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

    public override async Task RunAsync(RunCustomActionParams args)
    {
        try
        {
            var httpClient = await CreateHttpClientAsync(args.Context);
            var documentId = GetRequiredDocumentId(args.Context);
            await DownloadAndSaveFilesAsync(httpClient, documentId, args.Context);
        }
        catch (Exception ex)
        {
            args.Context.PluginLogger.AppendInfo($"Error during sending document for signature: {ex}");
            args.HasErrors = true;
            args.Message = ex.Message;
        }
    }

    private async Task DownloadAndSaveFilesAsync(AutentiHttpClient httpClient, string documentId, ActionContextInfo context)
    {
        var documentDetails = await httpClient.GetDocumentDetailsAsync(documentId);

        var signedFile = FindFile(documentDetails, documentId, AutentiFilePurposes.SignedContentFile);
        var fileContent = await httpClient.DownloadFileAsync(documentId, signedFile.Id);
        await SaveSignedFileToAttachmentsAsync(context, signedFile.Filename, fileContent);

        if(!Configuration.Response.DownloadSignatureCard)
            return;

        var signatureCardFile = TryFindFile(documentDetails, documentId, AutentiFilePurposes.SignatureCard);
        if (signatureCardFile == null)
        {
            context.PluginLogger.AppendInfo($"No signature card file found in document '{documentId}'.");
            return;
        }
        var signCardFileContent = await httpClient.DownloadFileAsync(documentId, signatureCardFile.Id);
        await SaveSigCardFileToAttachmentsAsync(context, signatureCardFile.Filename, signCardFileContent);
    }

    private async Task<AutentiHttpClient> CreateHttpClientAsync(ActionContextInfo context)
    {
        var clientProvider = new AutentiClientProvider(new ConnectionsHelper(context), Configuration.Authorization, context.PluginLogger);
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

    private static DocumentFileInfo FindFile(DocumentDetailsResponse documentDetails, string documentId, string purpose)
    {
        return TryFindFile(documentDetails, documentId, purpose) ??
            throw new SDKArgumentException($"No {purpose} file found in document '{documentId}'. The document has not been signed yet.");
    }

    private static DocumentFileInfo? TryFindFile(DocumentDetailsResponse documentDetails, string documentId, string purpose)
    {
        if (documentDetails.Files is not { Count: > 0 })
            throw new SDKArgumentException($"No files found in document '{documentId}'. The document may not have been processed yet.");

        return documentDetails.Files.FirstOrDefault(f => f.FilePurpose == purpose);
    }

    private async Task SaveSignedFileToAttachmentsAsync(ActionContextInfo context, string defaultFileName, byte[] content)
    {
        await SaveAttachmentAsync(context, defaultFileName, content, Configuration.Response.FileName);
    }

    private async Task SaveSigCardFileToAttachmentsAsync(ActionContextInfo context, string defaultFileName, byte[] content)
    {
        await SaveAttachmentAsync(context, defaultFileName, content, Configuration.Response.FileNameSignCard);
    }

    private async Task SaveAttachmentAsync(ActionContextInfo context, string defaultFileName, byte[] content, string configuredFileName)
    {
        var config = Configuration.Response;
        var fileName = !string.IsNullOrEmpty(configuredFileName) ? configuredFileName : defaultFileName;

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