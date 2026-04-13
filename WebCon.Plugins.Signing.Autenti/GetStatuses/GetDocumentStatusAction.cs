using System;
using System.Threading.Tasks;
using WebCon.Plugins.Signing.Autenti.Api;
using WebCon.WorkFlow.SDK.ActionPlugins;
using WebCon.WorkFlow.SDK.ActionPlugins.Model;
using WebCon.WorkFlow.SDK.Exceptions;
using WebCon.WorkFlow.SDK.Tools.Data;

namespace WebCon.Plugins.Signing.Autenti.GetStatuses;

public class GetDocumentStatusAction : CustomAction<GetDocumentStatusConfig>
{
    public override async Task RunAsync(RunCustomActionParams args)
    {
        try
        {
            var clientProvider = new AutentiClientProvider(new ConnectionsHelper(args.Context), Configuration.Authorization);
            var authenticatedClient = await clientProvider.GetAuthenticatedClientAsync();
            using var httpClient = new AutentiHttpClient(authenticatedClient, args.Context.PluginLogger);

            var documentId = args.Context.CurrentDocument.GetFieldValue(Configuration.Input.AutentiIDField)?.ToString();
            if (string.IsNullOrEmpty(documentId))
                throw new SDKArgumentException("No document ID found in the specified field.");

            var status = (await httpClient.GetDocumentDetailsAsync(documentId)).Status;
            await args.Context.CurrentDocument.SetFieldValueAsync(Configuration.Response.AutentiStatusField, status);
        }
        catch (Exception ex)
        {
            args.Context.PluginLogger.AppendInfo($"Error during sending document for signature: {ex}");
            args.HasErrors = true;
            args.Message = ex.Message;
        }
    }
}