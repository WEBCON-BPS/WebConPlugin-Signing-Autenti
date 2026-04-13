using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace WebCon.Plugins.Signing.Autenti.Api.Models;

public class ApiLinkResponse
{
    [JsonPropertyName("classifiers")]
    public List<string> Classifiers { get; set; } = [];

    [JsonPropertyName("attributes")]
    public ApiLinkAttributes Attributes { get; set; }
}

public class ApiLinkAttributes
{
    [JsonPropertyName("userInteraction")]
    public string UserInteraction { get; set; }

    [JsonPropertyName("httpRequest")]
    public ApiLinkHttpRequest HttpRequest { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; }

    [JsonPropertyName("actionIdentifier")]
    public string ActionIdentifier { get; set; }
}

public class ApiLinkHttpRequest
{
    [JsonPropertyName("uri")]
    public string Uri { get; set; }

    [JsonPropertyName("method")]
    public string Method { get; set; }
}