using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace WebCon.Plugins.Signing.Autenti.Api.Models;

public class DocumentProcessRequest
{
    [JsonPropertyName("title")]
    public string Title { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; }

    [JsonPropertyName("processLanguage")]
    public string ProcessLanguage { get; set; }

    [JsonPropertyName("parties")]
    public List<Party> Parties { get; set; } = [];

    [JsonPropertyName("tags")]
    public List<Tag> Tags { get; set; } = [];

    [JsonPropertyName("constraints")]
    public List<Constraint> Constraints { get; set; } = [];
}

public class Party
{
    [JsonPropertyName("party")]
    public Participant Participant { get; set; }

    [JsonPropertyName("role")]
    public string Role { get; set; }

    [JsonPropertyName("constraints")]
    public List<Constraint> Constraints { get; set; } = [];
}

public class Participant
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("firstName")]
    public string FirstName { get; set; }

    [JsonPropertyName("lastName")]
    public string LastName { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("contacts")]
    public List<Contact> Contacts { get; set; }


    [JsonPropertyName("extIds")]
    public List<ExternalId> ExtIds { get; set; }

    [JsonPropertyName("relationships")]
    public List<Relationship> Relationships { get; set; } = [];
}

public class Relationship
{
    [JsonPropertyName("type")]
    public string Type { get; set; }

    [JsonPropertyName("party")]
    public Participant Party { get; set; }

    [JsonPropertyName("attributes")]
    public RelationshipAttributes Attributes { get; set; }
}

public class RelationshipAttributes
{
    [JsonPropertyName("relationshipDescription")]
    public string RelationshipDescription { get; set; }
}

public class ExternalId
{
    [JsonPropertyName("identificationSpace")]
    public string IdentificationSpace { get; set; }

    [JsonPropertyName("identifier")]
    public string Identifier { get; set; }
}

public class Contact
{
    [JsonPropertyName("type")]
    public string Type { get; set; }

    [JsonPropertyName("attributes")]
    public ContactAttributes Attributes { get; set; }
}

public class ContactAttributes
{
    [JsonPropertyName("email")]
    public string Email { get; set; }
}

public class Constraint
{
    [JsonPropertyName("constrainedActions")]
    public string[] ConstrainedActions { get; set; }

    [JsonPropertyName("classifiers")]
    public string[] Classifiers { get; set; }

    [JsonPropertyName("attributes")]
    public ConstraintAttributes Attributes { get; set; }
}

public class ConstraintAttributes
{
    [JsonPropertyName("priority")]
    public int? Priority { get; set; }

    [JsonPropertyName("requiredClassifiers")]
    public string[] RequiredClassifiers { get; set; }

    [JsonPropertyName("phoneNumber")]
    public string PhoneNumber { get; set; }

    [JsonPropertyName("visualisationId")]
    public string VisualisationId { get; set; }

    [JsonPropertyName("identificationProfiles")]
    public List<IdentificationProfile> IdentificationProfiles { get; set; }
}

public class Tag
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; }
}