using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebCon.Plugins.Signing.Autenti.Api;
using WebCon.Plugins.Signing.Autenti.Api.Models;
using WebCon.Plugins.Signing.Autenti.Common;
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
            var recipients = RecipientPartyMapper.MapToRecipientStatuses(documentDetails.Parties);

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

    private async Task UpdateItemListAsync(ItemsList list, List<RecipientStatus> recipients)
    {
        var mapper = Configuration.Response.RecipientsListMapper;
        var matcher = new RecipientRowMatcher(mapper.Email, mapper.Role, mapper.AutentiId);

        foreach (var recipient in recipients)
        {
            var existingRow = matcher.FindMatch(list, recipient);

            if (existingRow != null)
            {
                await ApplyStatusAsync(existingRow, recipient);
            }
            else if (recipient.Role == AutentiRoles.Signer || recipient.Role == AutentiRoles.Approver)
            {
                var newRow = await list.Rows.AddNewRowAsync();
                await ApplyIdentityAsync(newRow, recipient);
                await ApplyStatusAsync(newRow, recipient);
            }
        }
    }

    private async Task ApplyStatusAsync(ItemRowData row, RecipientStatus recipient)
    {
        var mapper = Configuration.Response.RecipientsListMapper;

        await row.SetCellValueAsync(mapper.SignatureStatus, recipient.ParticipationStatus);
        await row.SetCellValueAsync(mapper.RejectionReason, recipient.RejectionReason);

        if (mapper.AutentiId.HasValue)
            await row.SetCellValueAsync(mapper.AutentiId.Value, recipient.AutentiId);
    }

    private async Task ApplyIdentityAsync(ItemRowData row, RecipientStatus recipient)
    {
        var mapper = Configuration.Response.RecipientsListMapper;

        await row.SetCellValueAsync(mapper.Email, recipient.Email);
        await row.SetCellValueAsync(mapper.Role, recipient.Role);

        await SetIfMappedAsync(row, mapper.FirstName, recipient.FirstName);
        await SetIfMappedAsync(row, mapper.LastName, recipient.LastName);
        await SetIfMappedAsync(row, mapper.SignatureType, recipient.SignatureType);
        await SetIfMappedAsync(row, mapper.PhoneNumber, recipient.PhoneNumber);
        await SetIfMappedAsync(row, mapper.SmsAuthorization, recipient.SmsAuthorization);
        await SetIfMappedAsync(row, mapper.SmsUnlock, recipient.SmsUnlock);
        await SetIfMappedAsync(row, mapper.Representative, recipient.IsRepresentative);
        await SetIfMappedAsync(row, mapper.OrganisationName, recipient.OrganisationName);
        await SetIfMappedAsync(row, mapper.VAT, recipient.Vat);
        await SetIfMappedAsync(row, mapper.Position, recipient.Position);
    }

    private static async Task SetIfMappedAsync(ItemRowData row, int? columnId, object value)
    {
        if (columnId.HasValue)
            await row.SetCellValueAsync(columnId.Value, value);
    }
}
