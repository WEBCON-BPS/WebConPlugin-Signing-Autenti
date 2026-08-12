using System.Threading.Tasks;
using WebCon.Plugins.Signing.Autenti.Api.Models;
using WebCon.Plugins.Signing.Autenti.SendForSignature;
using WebCon.WorkFlow.SDK.Common;

namespace WebCon.Plugins.Signing.Autenti.Api;

public class AutentiApiService : IAutentiApiService
{
    private readonly IDocumentProcessRequestBuilder _requestBuilder;
    private readonly IAutentiHttpClient _httpClient;
    private readonly PluginLogger _logger;

    public AutentiApiService(IDocumentProcessRequestBuilder requestBuilder, IAutentiHttpClient httpClient, PluginLogger logger)
    {
        _requestBuilder = requestBuilder;
        _httpClient = httpClient;
        _logger = logger;
    }
        
    public async Task<DocumentProcessResponse> SendForSignatureAsync(SendForSignatureConfig config)
    {
        _logger?.AppendDebug("Building document process request");
        var request = await _requestBuilder
            .WithDocumentDetails(config.DocumentDetails)
            .WithParticipantsAsync()
            .WithOrganizationSender(config.DocumentDetails)
            .WithSignatureVisualisation()
            .WithAttachmentsAsync()
            .WithTags()
            .BuildAsync();

        _logger?.AppendInfo($"Request built: {request.DocumentRequest.Parties?.Count ?? 0} parties, {request.Files?.Count ?? 0} files");

        var documentResult = await _httpClient.CreateDocumentProcessAsync(request.DocumentRequest);
        _logger?.AppendInfo($"Document process created with ID: {documentResult.Id}");

        await _httpClient.AddFilesToDocumentAsync(request.Files, documentResult.Id);
        _logger?.AppendInfo($"Added {request.Files.Count} file(s) to document {documentResult.Id}");

        await _httpClient.SendDocumentAsync(documentResult.Id);
        _logger?.AppendInfo($"Document {documentResult.Id} sent for signature");

        return documentResult;
    }
}