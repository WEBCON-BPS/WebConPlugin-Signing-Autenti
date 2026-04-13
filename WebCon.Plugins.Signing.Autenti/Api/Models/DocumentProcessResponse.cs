using System.Text.Json.Serialization;

namespace WebCon.Plugins.Signing.Autenti.Api.Models;

public class DocumentProcessResponse
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("status")]
    public string Status { get; set; }

    [JsonPropertyName("documentLink")]
    public string DocumentLink { get; set; }
}