using System;
using System.Linq;
using System.Threading.Tasks;
using WebCon.Plugins.Signing.Autenti.Api;
using WebCon.WorkFlow.SDK.ActionPlugins;
using WebCon.WorkFlow.SDK.ActionPlugins.Model;
using WebCon.WorkFlow.SDK.Exceptions;
using WebCon.WorkFlow.SDK.Tools.Data;

namespace WebCon.Plugins.Signing.Autenti.SignLink;

public class GetSignLink : CustomAction<GetSignLinkConfig>
{
    public override async Task RunAsync(RunCustomActionParams args)
    {
        try
        {
            var clientProvider = new AutentiClientProvider(new ConnectionsHelper(args.Context), Configuration.Authorization);
            var authenticatedClient = await clientProvider.GetAuthenticatedClientAsync();
            using var httpClient = new AutentiHttpClient(authenticatedClient, args.Context.PluginLogger);

            var documentId = args.Context.CurrentDocument.GetFieldValue(Configuration.Input.AutentiIDField)?.ToString();
            Validate(documentId);

            if(Configuration.Input.ActionMode == SignLinkActionMode.List)
            {
                await GetLinksForListAsync(httpClient, documentId, args.Context);
            }
            else
            {
                await GetLinksForFieldAsync(httpClient, documentId, args);
            }           
        }
        catch (Exception ex)
        {
            args.Context.PluginLogger.AppendInfo($"Error during getting api link: {ex}");
            args.HasErrors = true;
            args.Message = ex.Message;
        }
    }

    private async  Task GetLinksForListAsync(AutentiHttpClient httpClient, string documentId, ActionContextInfo context)
    {
        var list = context.CurrentDocument.ItemsLists.GetByID(Configuration.Response.RecipientsListMapper.ItemListId);
        context.PluginLogger.AppendDebug($"Updating item list {Configuration.Response.RecipientsListMapper.ItemListId}");

        foreach (var row in list.Rows)
        {
            var email = row.GetCellValue(Configuration.Response.RecipientsListMapper.Email)?.ToString();
            if (string.IsNullOrEmpty(email))
            {
                context.PluginLogger.AppendInfo($"No email found in row {row.ID}, skipping");
                continue;
            }

            var link = await GetLinkAsync(httpClient, documentId, email);
            await row.SetCellValueAsync(Configuration.Response.RecipientsListMapper.SignatureLink, link);
        }
    }

    private async Task GetLinksForFieldAsync(AutentiHttpClient httpClient, string documentId, RunCustomActionParams args)
    {
       var link = await GetLinkAsync(httpClient, documentId, Configuration.Input.UserEmail);

        if (Configuration.Input.ActionMode == SignLinkActionMode.Field)
            await args.Context.CurrentDocument.SetFieldValueAsync(Configuration.Response.LinkField.Value, link);
        else if (Configuration.Input.ActionMode == SignLinkActionMode.Redirect)
        {
            base.CustomJavascript  = $"window.open('{link}', '_blank').focus();";
        }       
    }

    private async Task<string> GetLinkAsync(AutentiHttpClient httpClient, string documentId, string email)
    {
        var apiLinks = await httpClient.GetApiLinksAsync(documentId, email);
        return apiLinks?.FirstOrDefault()?.Attributes?.HttpRequest?.Uri;
    }

    private void Validate(string documentId)
    {
        
        if (string.IsNullOrEmpty(documentId))
            throw new SDKArgumentException("No document ID found in the specified field.");

        if (string.IsNullOrEmpty(Configuration.Input.UserEmail) && Configuration.Input.ActionMode != SignLinkActionMode.List)
            throw new SDKArgumentException("No email specified.");
    }
}