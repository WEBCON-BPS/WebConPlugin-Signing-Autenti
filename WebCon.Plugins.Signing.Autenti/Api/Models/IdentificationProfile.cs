using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace WebCon.Plugins.Signing.Autenti.Api.Models;

public class IdentificationProfile
{
    [JsonPropertyName("amr_values")]
    public string[] AmrValues { get; set; }

    [JsonPropertyName("reuse_policy")]
    public ReusePolicy ReusePolicy { get; set; }

    [JsonPropertyName("verified_claims")]
    public List<VerifiedClaim> VerifiedClaims { get; set; } = [];
}

public class ReusePolicy
{
    [JsonPropertyName("reuse_duration")]
    public string ReuseDuration { get; set; }
}

public class VerifiedClaim
{
    [JsonPropertyName("verification")]
    public ClaimVerification Verification { get; set; }

    [JsonPropertyName("claims")]
    public ClaimDetails Claims { get; set; }
}

public class ClaimVerification
{
    [JsonPropertyName("trust_framework")]
    public string TrustFramework { get; set; }

    [JsonPropertyName("assurance_level")]
    public string AssuranceLevel { get; set; }
}

public class ClaimDetails
{
    [JsonPropertyName("given_name")]
    public string GivenName { get; set; }

    [JsonPropertyName("family_name")]
    public string FamilyName { get; set; }
}