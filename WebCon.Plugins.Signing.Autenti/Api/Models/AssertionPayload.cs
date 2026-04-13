using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace WebCon.Plugins.Signing.Autenti.Api.Models;

public class AssertionPayload
{
    [JsonPropertyName("classifiers")]
    public List<string> Classifiers { get; set; }

    [JsonPropertyName("attributes")]
    public AssertionAttributes Attributes { get; set; }
}

public class AssertionAttributes
{
    [JsonPropertyName("selectedIds")]
    public List<string> SelectedIds { get; set; }
}