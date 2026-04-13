using System;
using WebCon.Plugins.Signing.Autenti.Api.Models;

namespace WebCon.Plugins.Signing.Autenti.SendForSignature.Providers.Enrichers;

public class OrganizationRepresentativeEnricher : IPartyEnricher
{
    private const string MemberRelationshipType = "PARTY_RELATIONSHIP-TYPE:MEMBER";

    public void Enrich(Party party, IItemListRowContext row)
    {
        if (row.Representative != true)
            return;

        ValidateOrganizationData(row);

        var (identificationSpace, identifier) = ParseVat(row.VAT.Trim());

        party.Participant.Relationships =
        [
            new Relationship
            {
                Type = MemberRelationshipType,
                Party = new Participant
                {
                    Name = row.OrganisationName.Trim(),
                    ExtIds =
                    [
                        new ExternalId
                        {
                            IdentificationSpace = identificationSpace,
                            Identifier = identifier
                        }
                    ]
                },
                Attributes = new RelationshipAttributes
                {
                    RelationshipDescription = row.Position
                }
            }
        ];
    }

    private static (string IdentificationSpace, string Identifier) ParseVat(string vat)
    {
        const int countryCodeLength = 2;

        if (vat.Length <= countryCodeLength)
            throw new InvalidOperationException(
                $"VAT value '{vat}' is too short. Expected format: country code (2 letters) followed by tax identifier, e.g. 'PL1234567890'.");

        var countryCode = vat[..countryCodeLength];

        if (!char.IsLetter(countryCode[0]) || !char.IsLetter(countryCode[1]))
            throw new InvalidOperationException(
                $"VAT value '{vat}' must start with a 2-letter country code (e.g. 'PL', 'DE'). Got '{countryCode}'.");

        countryCode = countryCode.ToUpperInvariant();
        var identifier = vat[countryCodeLength..];

        return ($"TAXID-{countryCode}-NIP", identifier);
    }

    private static void ValidateOrganizationData(IItemListRowContext row)
    {
        if (string.IsNullOrWhiteSpace(row.OrganisationName))
            throw new InvalidOperationException(
                "Organisation name is required when the recipient is a representative of an organisation.");

        if (string.IsNullOrWhiteSpace(row.VAT))
            throw new InvalidOperationException(
                "VAT (NIP) is required when the recipient is a representative of anorganisation.");

        if(string.IsNullOrWhiteSpace(row.Position))
            throw new InvalidOperationException(
                "Position is required when the recipient is a representative of an organisation.");
    }
}