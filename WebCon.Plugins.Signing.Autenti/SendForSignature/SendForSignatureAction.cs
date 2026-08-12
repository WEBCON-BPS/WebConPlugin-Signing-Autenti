using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Polly;
using Polly.Retry;
using WebCon.Plugins.Signing.Autenti.Api;
using WebCon.Plugins.Signing.Autenti.Api.Models;
using WebCon.Plugins.Signing.Autenti.Common;
using WebCon.Plugins.Signing.Autenti.SendForSignature.Providers;
using WebCon.Plugins.Signing.Autenti.SendForSignature.Providers.Enrichers;
using WebCon.WorkFlow.SDK.ActionPlugins;
using WebCon.WorkFlow.SDK.ActionPlugins.Model;
using WebCon.WorkFlow.SDK.Documents.Model.ItemLists;
using WebCon.WorkFlow.SDK.Tools.Data;

namespace WebCon.Plugins.Signing.Autenti.SendForSignature;

public class SendForSignatureAction : CustomAction<SendForSignatureConfig>
{
    public override async Task RunAsync(RunCustomActionParams args)
    {
        try
        {
            args.Context.PluginLogger.AppendInfo("Prepare autenti service");

            var clientProvider = new AutentiClientProvider(new ConnectionsHelper(args.Context), Configuration.Authorization, args.Context.PluginLogger);
            var authenticatedClient = await clientProvider.GetAuthenticatedClientAsync();
            using var httpClient = new AutentiHttpClient(authenticatedClient, args.Context.PluginLogger);

            var apiService = PrepareAutentiService(args.Context, httpClient);

            args.Context.PluginLogger.AppendInfo("Create and send document for signature");
            var response = await apiService.SendForSignatureAsync(Configuration);

            args.Context.PluginLogger.AppendInfo("Set response to fields");
            await args.Context.CurrentDocument.SetFieldValueAsync(Configuration.Response.AutentiId, response.Id);

            await SaveRecipientsAutentiIdsAsync(args.Context, httpClient, response.Id);
        }
        catch (Exception ex)
        {
            args.Context.PluginLogger.AppendInfo($"Error during sending document for signature: {ex}");
            args.HasErrors = true;
            args.Message = ex.Message;
        }
    }

    private async Task SaveRecipientsAutentiIdsAsync(ActionContextInfo context, IAutentiHttpClient httpClient, string documentId)
    {
        var listConfig = Configuration.Recipents.RecipentsList;
        if (!listConfig.AutentiId.HasValue)
            return;

        var list = context.CurrentDocument.ItemsLists.GetByID(listConfig.ItemListId);
        var matcher = new RecipientRowMatcher(listConfig.Email, listConfig.Role, listConfig.AutentiId);

        var recipients = await DownloadRecipientsWithAutentiIdsAsync(context, httpClient, documentId, matcher, list);

        context.PluginLogger.AppendInfo("Save Autenti IDs of recipients on the item list");
        foreach (var recipient in recipients)
        {
            var row = matcher.FindMatch(list, recipient);
            if (row != null)
                await row.SetCellValueAsync(listConfig.AutentiId.Value, recipient.AutentiId);
        }
    }

    private async Task<List<RecipientStatus>> DownloadRecipientsWithAutentiIdsAsync(
        ActionContextInfo context, IAutentiHttpClient httpClient, string documentId, RecipientRowMatcher matcher, ItemsList list)
    {
        var pipeline = new ResiliencePipelineBuilder<List<RecipientStatus>>()
            .AddRetry(new RetryStrategyOptions<List<RecipientStatus>>
            {
                ShouldHandle = new PredicateBuilder<List<RecipientStatus>>()
                    .HandleResult(recipients => !AllMatchedRecipientsHaveAutentiId(recipients, matcher, list)),
                MaxRetryAttempts = 4,
                BackoffType = DelayBackoffType.Exponential,
                Delay = TimeSpan.FromSeconds(2),
                OnRetry = args =>
                {
                    context.PluginLogger.AppendInfo($"Autenti IDs for recipients not ready yet; retrying in {args.RetryDelay.TotalSeconds}s (attempt {args.AttemptNumber + 1}).");
                    return default;
                }
            })
            .Build();


        await Task.Delay(TimeSpan.FromSeconds(2));

        var recipients = await pipeline.ExecuteAsync(async _ =>
        {
            var documentDetails = await httpClient.GetDocumentDetailsAsync(documentId);
            return RecipientPartyMapper.MapToRecipientStatuses(documentDetails.Parties);
        });

        if (!AllMatchedRecipientsHaveAutentiId(recipients, matcher, list))
            throw new InvalidOperationException("Autenti IDs for recipients were not ready after all attempts.");

        return recipients;
    }

    private static bool AllMatchedRecipientsHaveAutentiId(List<RecipientStatus> recipients, RecipientRowMatcher matcher, ItemsList list)
    {
        var matched = recipients.Where(r => matcher.FindMatch(list, r) != null).ToList();
        return matched.Count > 0 && matched.All(r => IsPublicSigningProcessId(r.AutentiId));
    }

    private static bool IsPublicSigningProcessId(string autentiId)
        => autentiId?.StartsWith(AutentiIds.PublicSigningProcessIdPrefix, StringComparison.Ordinal) == true;

    private IAutentiApiService PrepareAutentiService(ActionContextInfo context, IAutentiHttpClient httpClient)
    {
        var enrichers = CreateEnrichers();
        var participantsProvider = new PartiesProvider(enrichers, Configuration.Recipents, context);
        var attachmentsProvider = new AttachmentsProvider(Configuration.Attachments, context);
        var tagsProvider = new TagsProvider(Configuration.DocumentDetails, context);
        var signatureVisualisationProvider = new SignatureVisualisationProvider(Configuration.DocumentDetails, context);

        var requestBuilder = new DocumentProcessRequestBuilder(participantsProvider, attachmentsProvider, tagsProvider, signatureVisualisationProvider, context.PluginLogger);

        return new AutentiApiService(requestBuilder, httpClient, context.PluginLogger);
    }

    private IPartyEnricher[] CreateEnrichers() =>
    [
        new CoreDataEnricher(),
        new SmsEnricher(),
        new SignatureEnricher(),
        new SigningOrderEnricher(Configuration.Recipents.IncludeSigningOrder),
        new OrganizationRepresentativeEnricher()
    ];
}