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
                var clientProvider = new AutentiClientProvider(new ConnectionsHelper(args.Context), Configuration.Authorization, args.Context.PluginLogger);
                var authenticatedClient = await clientProvider.GetAuthenticatedClientAsync();
                using var httpClient = new AutentiHttpClient(authenticatedClient, args.Context.PluginLogger);
                var documentId = args.Context.CurrentDocument.GetFieldValue(Configuration.Input.AutentiIDField)?.ToString();

                if (string.IsNullOrEmpty(documentId))
                    throw new SDKArgumentException("No document ID found in the specified field.");

               await SendReminderAsync(httpClient, documentId, args.Context);
            }
            catch (Exception ex)
            {
                args.Context.PluginLogger.AppendInfo($"Error during sending reminders: {ex}");
                args.HasErrors = true;
                args.Message = ex.Message;
            }
        }


        private async Task SendReminderAsync(IAutentiHttpClient httpClient, string documentId, ActionContextInfo context)
        {
            var availableRecipients = await httpClient.GetAvailableRecipientsToSendReminderAsync(documentId);
            var ids = await GetRecipientsIdsToSendReminderAsync(availableRecipients, documentId, httpClient, context);   
            
            if(ids == null || !ids.Any())
                throw new SDKArgumentException("No recipients available to send reminders.");
            

            var selectedRecipientsAsseration = CreateSelectedRecipientsToSendReminder(ids);
            await httpClient.SendReminderAsync(documentId, selectedRecipientsAsseration);
        }

        private async Task<List<string>> GetRecipientsIdsToSendReminderAsync(ReminderChallengeResponse response, string documentId, IAutentiHttpClient httpClient, ActionContextInfo context)
        {
            if (response?.Attributes?.Errors != null)
            {
                var error = response?.Attributes?.Errors?.FirstOrDefault();
                throw new Exception(error?.Description);
            }

            var availableIds = response?.Attributes?.Options?.Where(x => string.Equals(x.Availability, AutentiActionsStatus.Available, StringComparison.OrdinalIgnoreCase)).Select(x => x.Id).ToList();
            return await RemoveMarkedRecipientsAsync(availableIds, documentId, httpClient, context);
        }

        private async Task<List<string>> RemoveMarkedRecipientsAsync(List<string> availableIds, string documentId, IAutentiHttpClient httpClient, ActionContextInfo context)
        {
            if(!Configuration.Input.RecipientsListMapper.BlockSend.HasValue || !Configuration.Input.RecipientsListMapper.Email.HasValue)
                return availableIds;

            var processIds = await httpClient.GetDetailsOfParticipantsAsync(documentId);
            var list = context.CurrentDocument.ItemsLists.GetByID(Configuration.Input.RecipientsListMapper.ItemListId);

            foreach (var row in list.Rows)
                if (row.BooleanCells.GetByID(Configuration.Input.RecipientsListMapper.BlockSend.Value).Value ?? false)
                {
                    var email = row.GetCellValue(Configuration.Input.RecipientsListMapper.Email.Value)?.ToString();
                    var processId = GetProcessIdByEmail(processIds, email);

                    if (!string.IsNullOrEmpty(processId))
                    {
                        context.PluginLogger.AppendInfo($"Blocking reminder for recipient with email: {email}");
                        availableIds.Remove(processId);
                    }
                }
            return availableIds;
        }

        private string? GetProcessIdByEmail(List<ParticipantsDetailsResponse> processIds, string emailToMatch)
        {
            foreach (var processId in processIds)
            {
                var email = processId.Party.Contacts.FirstOrDefault(x => string.Equals(x.Type, "CONTACT-TYPE:EMAIL", StringComparison.OrdinalIgnoreCase))?.Attributes?.Email;
                if(email != null && string.Equals(email, emailToMatch, StringComparison.OrdinalIgnoreCase))
                    return processId.Party.Id;
            }
            return null;
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