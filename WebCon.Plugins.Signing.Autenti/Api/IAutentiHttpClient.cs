using System.Collections.Generic;
using System.Threading.Tasks;
using WebCon.Plugins.Signing.Autenti.Api.Models;

namespace WebCon.Plugins.Signing.Autenti.Api;

public interface IAutentiHttpClient
{
    Task<DocumentProcessResponse> CreateDocumentProcessAsync(DocumentProcessRequest request);
    Task AddFilesToDocumentAsync(List<FileData> files, string docGuid);
    Task SendDocumentAsync(string documentId);
    Task<ReminderChallengeResponse> GetAvailableRecipientsToSendReminderAsync(string documentId);
    Task SendReminderAsync(string documentId, string selectedRecipientsAsseration);
}