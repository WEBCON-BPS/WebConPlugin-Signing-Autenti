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

    [JsonPropertyName("events")]
    public List<DocumentPartyEvent> Events { get; set; } = [];

    [JsonPropertyName("constraints")]
    public List<Constraint> Constraints { get; set; } = [];
}

public class DocumentPartyEvent
{
    [JsonPropertyName("eventType")]
    public string EventType { get; set; }

    [JsonPropertyName("attributes")]
    public DocumentPartyEventAttributes Attributes { get; set; }
}

public class DocumentPartyEventAttributes
{
    [JsonPropertyName("comment")]
    public string Comment { get; set; }
}

public class PartyIdentity
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("firstName")]
    public string FirstName { get; set; }

    [JsonPropertyName("lastName")]
    public string LastName { get; set; }

    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("contacts")]
    public List<PartyContact> Contacts { get; set; } = [];

    [JsonPropertyName("relationships")]
    public List<Relationship> Relationships { get; set; } = [];
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