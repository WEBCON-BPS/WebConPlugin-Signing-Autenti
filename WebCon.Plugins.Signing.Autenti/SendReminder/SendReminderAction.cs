using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using WebCon.Plugins.Signing.Autenti.Api;
using WebCon.Plugins.Signing.Autenti.Api.Models;
using WebCon.WorkFlow.SDK.ActionPlugins;
using WebCon.WorkFlow.SDK.ActionPlugins.Model;
using WebCon.WorkFlow.SDK.Exceptions;
using WebCon.WorkFlow.SDK.Tools.Data;

namespace WebCon.Plugins.Signing.Autenti.SendReminder
{
    public class SendReminderAction : CustomAction<SendReminderConfig>
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

               await SendReminderAsync(httpClient, documentId);
            }
            catch (Exception ex)
            {
                args.Context.PluginLogger.AppendInfo($"Error during sending reminders: {ex}");
                args.HasErrors = true;
                args.Message = ex.Message;
            }
        }


        private async Task SendReminderAsync(IAutentiHttpClient httpClient, string documentId)
        {
            var response = await httpClient.GetAvailableRecipientsToSendReminderAsync(documentId);
            var ids = GetRecipientsIds(response);
            var selectedRecipientsAsseration = CreateSelectedRecipientsToSendReminder(ids);
            await httpClient.SendReminderAsync(documentId, selectedRecipientsAsseration);
        }

        private List<string> GetRecipientsIds(ReminderChallengeResponse response)
        {
            if (response?.Attributes?.Errors != null)
            {
                var error = response?.Attributes?.Errors?.FirstOrDefault();
                throw new Exception(error?.Description);
            }

            return response?.Attributes?.Options?.Where(x => string.Equals(x.Availability, "AVAILABLE", StringComparison.OrdinalIgnoreCase)).Select(x => x.Id).ToList();
        }

        private string CreateSelectedRecipientsToSendReminder(List<string> availableRecipients)
        {
            var challengeRequest = new ReminderChallengeRequest
            {
                classifiers = new[] { AutentiClassifiers.RemindersPartySelection },
                attributes = new ReminderChallengeRequestAttributes
                {
                    selectedIds = availableRecipients.ToArray()
                }
            };
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(challengeRequest)));
        }
    }
}