using System.Text.Json.Serialization;

namespace WebCon.Plugins.Signing.Autenti.Api.Models;

public class ReminderChallengeResponse
{
    [JsonPropertyName("classifiers")]
    public string[] Classifiers { get; set; }

    [JsonPropertyName("attributes")]
    public ReminderChallengeAttributes Attributes { get; set; }
}

public class ReminderChallengeAttributes
{
    [JsonPropertyName("mode")]
    public string Mode { get; set; }

    [JsonPropertyName("options")]
    public Option[] Options { get; set; }

    [JsonPropertyName("errors")]
    public ReminderChallengeError[] Errors { get; set; }
}

public class ReminderChallengeError
{
    [JsonPropertyName("description")]
    public string Description { get; set; }

    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; }
}

public class Option
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; }

    [JsonPropertyName("availability")]
    public string Availability { get; set; }

    [JsonPropertyName("meta")]
    public object Meta { get; set; }
}