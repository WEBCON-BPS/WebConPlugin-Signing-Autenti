using System;
using System.Threading.Tasks;
using WebCon.Plugins.Signing.Autenti.Api;
using WebCon.WorkFlow.SDK.ActionPlugins;
using WebCon.WorkFlow.SDK.ActionPlugins.Model;
using WebCon.WorkFlow.SDK.Exceptions;
using WebCon.WorkFlow.SDK.Tools.Data;

namespace WebCon.Plugins.Signing.Autenti.Withdrawal
{
    public class WithdrawalAction : CustomAction<WithdrawalConfig>
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

                await httpClient.DocumentWithdrawalAsync(documentId);
            }
            catch (Exception ex)
            {
                args.Context.PluginLogger.AppendInfo($"Error during document withdrawal: {ex}");
                args.HasErrors = true;
                args.Message = ex.Message;
            }
        }
    }
}