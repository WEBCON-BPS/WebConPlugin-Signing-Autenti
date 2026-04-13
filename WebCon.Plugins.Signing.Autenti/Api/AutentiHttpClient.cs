using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Web;
using WebCon.Plugins.Signing.Autenti.Api.Models;
using WebCon.WorkFlow.SDK.Common;

namespace WebCon.Plugins.Signing.Autenti.Api;

public class AutentiHttpClient(HttpClient httpClient, PluginLogger logger) : IAutentiHttpClient, IDisposable
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    private static readonly string SendDocumentAssertion = BuildSendDocumentAssertion();
    private static readonly string SendReminderAssertion = BuildSendReminderAssertion();

    private static string BuildSendDocumentAssertion()
    {
        var payload = new AssertionPayload
        {
            Classifiers = [AutentiClassifiers.ActionSelection],
            Attributes = new AssertionAttributes
            {
                SelectedIds = [AutentiClassifiers.DocumentSent]
            }
        };

        var json = JsonSerializer.Serialize(payload, JsonOptions);
        return Convert.ToBase64String(Encoding.UTF8.GetBytes(json));
    }

    private static string BuildSendReminderAssertion()
    {
        var payload = new AssertionPayload
        {
            Classifiers = [AutentiClassifiers.ActionSelection],
            Attributes = new AssertionAttributes
            {
                SelectedIds = [AutentiClassifiers.RemindersSent]
            }
        };

        var json = JsonSerializer.Serialize(payload, JsonOptions);
        return Convert.ToBase64String(Encoding.UTF8.GetBytes(json));
    }

    public async Task<DocumentProcessResponse> CreateDocumentProcessAsync(DocumentProcessRequest request)
    {
        var json = JsonSerializer.Serialize(request, JsonOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        logger.AppendDebug($"Create document request: {json}");

        var response = await httpClient.PostAsync("/api/v2/document-processes", content);
        await response.EnsureSuccessOrThrowWithBodyAsync(logger);

        var responseJson = await response.Content.ReadAsStringAsync();
        logger.AppendDebug($"Create document response: {responseJson}");
        return JsonSerializer.Deserialize<DocumentProcessResponse>(responseJson, JsonOptions);
    }

    public async Task AddFilesToDocumentAsync(List<FileData> files, string documentId)
    {
        if (files.Count == 0)
            throw new ArgumentException("Empty attachments list. Please attach at least one file.");

        foreach (var file in files)
        {
            var imageContent = new ByteArrayContent(file.Content);
            imageContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/octet-stream");

            var multiForm = new MultipartFormDataContent
            {
                { imageContent, "file", file.Name }
            };

            var request = new HttpRequestMessage(HttpMethod.Post, $"/api/v2/document-processes/{documentId}/files")
            {
                Content = multiForm
            };
            request.Headers.Add("Accept", "application/json");

            var response = await httpClient.SendAsync(request);
            var result = await response.Content.ReadAsStringAsync();
            logger.AppendDebug($"Add file response: {result}");
            await response.EnsureSuccessOrThrowWithBodyAsync(logger);
        }
    }

    public async Task SendDocumentAsync(string documentId)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"/api/v2/document-processes/{documentId}/actions")
        {
            Content = new StringContent("{}", Encoding.UTF8, "application/json")
        };
        request.Headers.Add("Accept", "application/json");
        request.Headers.Add("X-ASSERTION", SendDocumentAssertion);

        var response = await httpClient.SendAsync(request);
        var result = await response.Content.ReadAsStringAsync();
        logger.AppendDebug($"Send document response: {result}");
        await response.EnsureSuccessOrThrowWithBodyAsync(logger);
    }

    public async Task<DocumentDetailsResponse> GetDocumentDetailsAsync(string documentId)
    {
        var response = await httpClient.GetAsync($"/api/v2/document-processes/{documentId}");
        await response.EnsureSuccessOrThrowWithBodyAsync(logger);
        var responseJson = await response.Content.ReadAsStringAsync();
        logger.AppendDebug($"Get document details response: {responseJson}");
        return JsonSerializer.Deserialize<DocumentDetailsResponse>(responseJson, JsonOptions);
    }

    public async Task<List<ApiLinkResponse>> GetApiLinksAsync(string documentId, string email)
    {
        var content = new StringContent("");
        content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        var response = await httpClient.PostAsync($"/api/v2/document-processes/{documentId}/parties/PARTY-EMAIL:{email}/action-links", content);
        await response.EnsureSuccessOrThrowWithBodyAsync(logger);
        var responseJson = await response.Content.ReadAsStringAsync();
        logger.AppendDebug($"Get api links response: {responseJson}");
        return JsonSerializer.Deserialize<List<ApiLinkResponse>>(responseJson, JsonOptions);
    }

    public async Task<ReminderChallengeResponse> GetAvailableRecipientsToSendReminderAsync(string documentId)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"/api/v2/document-processes/{documentId}/actions")
        {
            Content = new StringContent("{}", Encoding.UTF8, "application/json")
        };
        request.Headers.Add("Accept", "application/json");
        request.Headers.Add("X-ASSERTION", SendReminderAssertion);

        var response = await httpClient.SendAsync(request);
        var result = HttpResponseMessageExtensions.TryDecodeChallengeHeader(response);
        logger.AppendDebug("GetAvailableRecipientsToSendReminderAsync Response: " + result);
        var challenge = JsonSerializer.Deserialize<ReminderChallengeResponse>(result, JsonOptions);    
        return challenge;
    }

    public async Task SendReminderAsync(string documentId, string selectedRecipientsAsseration)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"/api/v2/document-processes/{documentId}/actions")
        {
            Content = new StringContent("{}", Encoding.UTF8, "application/json")
        };
        request.Headers.Add("Accept", "application/json");
        request.Headers.Add("X-ASSERTION", SendReminderAssertion);
        request.Headers.Add("X-ASSERTION", selectedRecipientsAsseration);

        var response = await httpClient.SendAsync(request);
        await response.EnsureSuccessOrThrowWithBodyAsync(logger);
    }

    /// <summary>
    /// Retrieves file metadata for a given document process.
    /// <para>
    /// <b>Known issue:</b> The Autenti API returns concatenated JSON objects (e.g., <c>{...}{...}{...}</c>)
    /// instead of a valid JSON array or single object. This format is not valid JSON and cannot be parsed
    /// by <see cref="JsonDocument"/> or <see cref="JsonSerializer"/>, resulting in a <see cref="JsonException"/>.
    /// </para>
    /// </summary>
    /// <param name="documentId">The identifier of the document process.</param>
    /// <returns>A list of <see cref="DocumentFileInfo"/> objects.</returns>
    [Obsolete("Disabled due to Autenti API returning invalid JSON (concatenated objects instead of an array). Awaiting a fix from Autenti.")]
    public async Task<List<DocumentFileInfo>> GetDocumentFilesAsync(string documentId)
    {
        var response = await httpClient.GetAsync($"/api/v2/document-processes/{documentId}/files");
        await response.EnsureSuccessOrThrowWithBodyAsync(logger);
        var responseJson = await response.Content.ReadAsStringAsync();
        logger.AppendDebug($"Get document files response: {responseJson}");

        using var document = JsonDocument.Parse(responseJson);
        if (document.RootElement.ValueKind == JsonValueKind.Array)
            return JsonSerializer.Deserialize<List<DocumentFileInfo>>(responseJson, JsonOptions);

        var single = JsonSerializer.Deserialize<DocumentFileInfo>(responseJson, JsonOptions);
        return [single];
    }

    public async Task<byte[]> DownloadFileAsync(string documentId, string fileId)
    {
        var response = await httpClient.GetAsync($"/api/v2/document-processes/{documentId}/files/{HttpUtility.UrlEncode(fileId)}/content");
        await response.EnsureSuccessOrThrowWithBodyAsync(logger);
        return await response.Content.ReadAsByteArrayAsync();
    }

    public void Dispose()
    {
        httpClient.Dispose();
    }
}