using System;
using System.Threading.Tasks;
using WebCon.Plugins.Signing.Autenti.Api;
using WebCon.Plugins.Signing.Autenti.SendForSignature.Providers;
using WebCon.Plugins.Signing.Autenti.SendForSignature.Providers.Enrichers;
using WebCon.WorkFlow.SDK.ActionPlugins;
using WebCon.WorkFlow.SDK.ActionPlugins.Model;
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
        }
        catch (Exception ex)
        {
            args.Context.PluginLogger.AppendInfo($"Error during sending document for signature: {ex}");
            args.HasErrors = true;
            args.Message = ex.Message;
        }
    }

    private IAutentiApiService PrepareAutentiService(ActionContextInfo context, IAutentiHttpClient httpClient)
    {
        var enrichers = CreateEnrichers();
        var participantsProvider = new PartiesProvider(enrichers, Configuration.Recipents, context);
        var attachmentsProvider = new AttachmentsProvider(Configuration.Attachments, context);
        var tagsProvider = new TagsProvider(Configuration.DocumentDetails, context);

        var requestBuilder = new DocumentProcessRequestBuilder(participantsProvider, attachmentsProvider, tagsProvider, context.PluginLogger);

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