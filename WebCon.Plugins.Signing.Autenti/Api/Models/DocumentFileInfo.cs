using System.Text.Json.Serialization;

namespace WebCon.Plugins.Signing.Autenti.Api.Models;

public class DocumentFileInfo
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("filename")]
    public string Filename { get; set; }

    [JsonPropertyName("filePurpose")]
    public string FilePurpose { get; set; }

    [JsonPropertyName("mimeType")]
    public string MimeType { get; set; }
}