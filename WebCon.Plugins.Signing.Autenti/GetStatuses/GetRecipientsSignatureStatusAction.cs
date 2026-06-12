using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebCon.Plugins.Signing.Autenti.Api;
using WebCon.Plugins.Signing.Autenti.Api.Models;
using WebCon.WorkFlow.SDK.ActionPlugins;
using WebCon.WorkFlow.SDK.ActionPlugins.Model;
using WebCon.WorkFlow.SDK.Documents.Model.ItemLists;
using WebCon.WorkFlow.SDK.Exceptions;
using WebCon.WorkFlow.SDK.Tools.Data;

namespace WebCon.Plugins.Signing.Autenti.GetStatuses;

public class GetRecipientsSignatureStatusAction : CustomAction<GetRecipientsSignatureStatusConfig>
{
    public override async Task RunAsync(RunCustomActionParams args)
    {
        try
        {
            var clientProvider = new AutentiClientProvider(new ConnectionsHelper(args.Context), Configuration.Authorization, args.Context.PluginLogger);
            var authenticatedClient = await clientProvider.GetAuthenticatedClientAsync();
            using var httpClient = new AutentiHttpClient(authenticatedClient, args.Context.PluginLogger);

            var documentId = args.Context.CurrentDocument.GetFieldValue(Configuration.Input.AutentiIDField)?.ToString();
            if (string.IsNullOrEmpty(documentId))
                throw new SDKArgumentException("No document ID found in the specified field.");

            var documentDetails = await httpClient.GetDocumentDetailsAsync(documentId);
            var recipients = MapToRecipientStatuses(documentDetails.Parties);

            var list = args.Context.CurrentDocument.ItemsLists.GetByID(Configuration.Response.RecipientsListMapper.ItemListId);
            await UpdateItemListAsync(list, recipients);
        }
        catch (Exception ex)
        {
            args.Context.PluginLogger.AppendInfo($"Error during sending document for signature: {ex}");
            args.HasErrors = true;
            args.Message = ex.Message;
        }
    }

    private List<RecipientStatus> MapToRecipientStatuses(List<DocumentParty> parties)
        => [.. parties.Select(p => new RecipientStatus
        {
            Email = p.Party.Contacts.FirstOrDefault()?.Attributes.Email,
            Role = p.Role,
            ParticipationStatus = p.Events?.FirstOrDefault(x => x?.EventType == AutentiEvents.SIGNATURE_REJECTION)?.Attributes?.Comment != null ? AutentiStatuses.Rejected : p.ParticipationStatus,
            RejectionReason = p.Events?.FirstOrDefault(x => x?.EventType == AutentiEvents.SIGNATURE_REJECTION)?.Attributes?.Comment
        })];

    private async Task UpdateItemListAsync(ItemsList list, List<RecipientStatus> recipients)
    {
        var mapper = Configuration.Response.RecipientsListMapper;

        foreach (var recipient in recipients)
        {
            var existingRow = list.Rows.FirstOrDefault(r => IsMatchingRow(r, recipient));

            if (existingRow != null)
            {
                await existingRow.SetCellValueAsync(mapper.SignatureStatus, recipient.ParticipationStatus);
                await existingRow.SetCellValueAsync(mapper.RejectionReason, recipient.RejectionReason);
            }
                   
        }
    }

    private bool IsMatchingRow(ItemRowData r, RecipientStatus recipent)
        => r.GetCellValue(Configuration.Response.RecipientsListMapper.Email)?.ToString() == recipent.Email &&
           r.GetCellValue(Configuration.Response.RecipientsListMapper.Role)?.ToString()?.Split("#")?.FirstOrDefault() == recipent.Role;
}

internal class RecipientStatus
{
    public string Email { get; set; }
    public string Role { get; set; }
    public string ParticipationStatus { get; set; }
    public string RejectionReason { get; set; }
}