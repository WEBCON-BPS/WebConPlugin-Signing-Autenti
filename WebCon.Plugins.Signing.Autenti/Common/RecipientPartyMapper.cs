using System.Collections.Generic;
using System.Linq;
using WebCon.Plugins.Signing.Autenti.Api.Models;

namespace WebCon.Plugins.Signing.Autenti.Common;

public static class RecipientPartyMapper
{
    public static List<RecipientStatus> MapToRecipientStatuses(List<DocumentParty> parties)
        => [.. parties.Select(MapToRecipientStatus)];

    public static RecipientStatus MapToRecipientStatus(DocumentParty p)
    {
        var rejectionReason = p.Events?.FirstOrDefault(x => x?.EventType == AutentiEvents.SIGNATURE_REJECTION)?.Attributes?.Comment;
        var relationship = p.Party?.Relationships?.FirstOrDefault(r => r?.Party != null);

        return new RecipientStatus
        {
            Email = p.Party?.Contacts?.FirstOrDefault()?.Attributes?.Email,
            AutentiId = p.Party?.Id,
            Role = p.Role,
            ParticipationStatus = rejectionReason != null ? AutentiStatuses.Rejected : p.ParticipationStatus,
            RejectionReason = rejectionReason,
            FirstName = p.Party?.FirstName,
            LastName = p.Party?.LastName,
            SignatureType = ResolveSignatureType(p.Constraints),
            PhoneNumber = ResolvePhoneNumber(p.Constraints),
            SmsAuthorization = HasPhoneVerification(p.Constraints, AutentiActions.SignatureApplication),
            SmsUnlock = HasPhoneVerification(p.Constraints, AutentiActions.ReadSensitiveMetadata),
            IsRepresentative = relationship != null,
            OrganisationName = relationship?.Party?.Name,
            Vat = ResolveVat(relationship),
            Position = relationship?.Attributes?.RelationshipDescription
        };
    }

    private static string ResolveSignatureType(List<Constraint> constraints)
    {
        var required = constraints?
            .FirstOrDefault(c => c.Classifiers?.Contains(AutentiConstraints.SignatureType) == true)?
            .Attributes?.RequiredClassifiers?.FirstOrDefault();

        return required?.Split(':').LastOrDefault();
    }

    private static bool HasPhoneVerification(List<Constraint> constraints, string action)
        => constraints?.Any(c =>
            c.Classifiers?.Contains(AutentiConstraints.PhoneNumberVerification) == true &&
            c.ConstrainedActions?.Contains(action) == true) == true;

    private static string ResolvePhoneNumber(List<Constraint> constraints)
        => constraints?
            .FirstOrDefault(c =>
                c.Classifiers?.Contains(AutentiConstraints.PhoneNumberVerification) == true &&
                !string.IsNullOrWhiteSpace(c.Attributes?.PhoneNumber))?
            .Attributes?.PhoneNumber;

    private static string ResolveVat(Relationship relationship)
    {
        var extId = relationship?.Party?.ExtIds?.FirstOrDefault();
        if (extId == null || string.IsNullOrWhiteSpace(extId.Identifier))
            return null;

        var space = extId.IdentificationSpace ?? string.Empty;
        var segments = space.Split('-');
        var countryCode = segments.Length >= 2 ? segments[1] : string.Empty;

        return $"{countryCode}{extId.Identifier}";
    }
}
