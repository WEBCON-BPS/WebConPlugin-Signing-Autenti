using System;
using System.Linq;
using System.Threading.Tasks;
using WebCon.Plugins.Signing.Autenti.Api;
using WebCon.Plugins.Signing.Autenti.Api.Models;
using WebCon.WorkFlow.SDK.ActionPlugins;
using WebCon.WorkFlow.SDK.ActionPlugins.Model;
using WebCon.WorkFlow.SDK.Exceptions;
using WebCon.WorkFlow.SDK.Tools.Data;

namespace WebCon.Plugins.Signing.Autenti.GetDocument;

public class GetDocumentAction : CustomAction<GetDocumentConfig>
{
    private const string SignedContentFilePurpose = "SIGNED_CONTENT_FILE";

    public override async Task RunAsync(RunCustomActionParams args)
    {
        try
        {
            var httpClient = await CreateHttpClientAsync(args.Context);
            var documentId = GetRequiredDocumentId(args.Context);

            var signedFile = await FindSignedFileAsync(httpClient, documentId);
            var fileContent = await httpClient.DownloadFileAsync(documentId, signedFile.Id);

            await SaveAttachmentAsync(args.Context, signedFile.Filename, fileContent);
        }
        catch (Exception ex)
        {
            args.Context.PluginLogger.AppendInfo($"Error during sending document for signature: {ex}");
            args.HasErrors = true;
            args.Message = ex.Message;
        }
    }

    private async Task<AutentiHttpClient> CreateHttpClientAsync(ActionContextInfo context)
    {
        var clientProvider = new AutentiClientProvider(new ConnectionsHelper(context), Configuration.Authorization);
        var authenticatedClient = await clientProvider.GetAuthenticatedClientAsync();
        return new AutentiHttpClient(authenticatedClient, context.PluginLogger);
    }

    private string GetRequiredDocumentId(ActionContextInfo context)
    {
        var documentId = context.CurrentDocument.GetFieldValue(Configuration.Input.AutentiIDField)?.ToString();
        if (string.IsNullOrEmpty(documentId))
            throw new SDKArgumentException("No document ID found in the specified field.");
        return documentId;
    }

    private static async Task<DocumentFileInfo> FindSignedFileAsync(AutentiHttpClient httpClient, string documentId)
    {
        var documentDetails = await httpClient.GetDocumentDetailsAsync(documentId);

        if (documentDetails.Files is not { Count: > 0 })
            throw new SDKArgumentException($"No files found in document '{documentId}'. The document may not have been processed yet.");

        return documentDetails.Files.FirstOrDefault(f => f.FilePurpose == SignedContentFilePurpose)
            ?? throw new SDKArgumentException($"No signed file found in document '{documentId}'. The document has not been signed yet.");
    }

    private async Task SaveAttachmentAsync(ActionContextInfo context, string defaultFileName, byte[] content)
    {
        var config = Configuration.Response;
        var fileName = !string.IsNullOrEmpty(config.FileName) ? config.FileName : defaultFileName;

        if (!fileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
            fileName += ".pdf";

        if (config.OverwriteAttachment)
        {
            var existing = context.CurrentDocument.Attachments.FirstOrDefault(a => a.FileName == fileName);

            if (existing != null)
            {
                existing.SetContent(content);
                return;
            }
        }

        var attachment = await context.CurrentDocument.Attachments.AddNewAsync(fileName, content);

        if (config.CategoryMode == Category.Dynamic && !string.IsNullOrEmpty(config.DynamicCategory))
            await attachment.SetFileGroupAsync(config.DynamicCategory);
    }
}