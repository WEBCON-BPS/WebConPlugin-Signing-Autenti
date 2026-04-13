using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace WebCon.Plugins.Signing.Autenti.Api.Models;

public class DocumentDetailsResponse
{
    [JsonPropertyName("status")]
    public string Status { get; set; }

    [JsonPropertyName("parties")]
    public List<DocumentParty> Parties { get; set; } = [];

    [JsonPropertyName("files")]
    public List<DocumentFileInfo> Files { get; set; } = [];
}

public class DocumentParty
{
    [JsonPropertyName("party")]
    public PartyIdentity Party { get; set; }

    [JsonPropertyName("role")]
    public string Role { get; set; }

    [JsonPropertyName("participationStatus")]
    public string ParticipationStatus { get; set; }
}

public class PartyIdentity
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("contacts")]
    public List<PartyContact> Contacts { get; set; } = [];
}

public class PartyContact
{
    [JsonPropertyName("attributes")]
    public PartyContactAttributes Attributes { get; set; }
}

public class PartyContactAttributes
{
    [JsonPropertyName("email")]
    public string Email { get; set; }
}