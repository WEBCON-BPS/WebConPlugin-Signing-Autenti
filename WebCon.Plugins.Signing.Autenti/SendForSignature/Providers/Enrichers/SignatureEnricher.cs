using System.Collections.Generic;
using WebCon.Plugins.Signing.Autenti.Api.Models;

namespace WebCon.Plugins.Signing.Autenti.SendForSignature.Providers.Enrichers;

public class SignatureEnricher : IPartyEnricher
{
    private const string SignatureProviderType = "SIGNATURE_PROVIDER-SIGNATURE_TYPE:{0}";
    private const string SignatureApplicationAction = "ACTION:SIGNATURE_APPLICATION";

    public void Enrich(Party party, IItemListRowContext row)
    {
        if (row.Role != AutentiRoles.Signer)
            return;

        AddSignConstraints(party.Constraints, row);

        if (row.SignatureType == AutentiSignatureTypes.Advanced)
            AddIdentityVerificationConstraint(party);
    }

    private void AddSignConstraints(List<Constraint> constraints, IItemListRowContext row)
    {
        constraints.Add(new Constraint()
        {
            ConstrainedActions = [SignatureApplicationAction],
            Classifiers = [AutentiConstraints.SignatureType],
            Attributes = new ConstraintAttributes()
            {
                RequiredClassifiers = [string.Format(SignatureProviderType, row.SignatureType)]
            }
        });
    }

    private void AddIdentityVerificationConstraint(Party party)
    {
        party.Constraints.Add(new Constraint()
        {
            ConstrainedActions = [SignatureApplicationAction],
            Classifiers = [AutentiConstraints.IdentityVerification],
            Attributes = new ConstraintAttributes()
            {
                IdentificationProfiles =
                [
                    new IdentificationProfile
                    {
                        AmrValues = [AutentiIdentification.ArmValue],
                        ReusePolicy = new ReusePolicy
                        {
                            ReuseDuration = AutentiIdentification.ReuseDuration
                        },
                        VerifiedClaims =
                        [
                            new VerifiedClaim
                            {
                                Verification = new ClaimVerification
                                {
                                    TrustFramework = AutentiIdentification.TrustFramework,
                                    AssuranceLevel = AutentiIdentification.AssuranceLevel
                                },
                                Claims = new ClaimDetails
                                {
                                    GivenName = party.Participant?.FirstName,
                                    FamilyName = party.Participant?.LastName
                                }
                            }
                        ]
                    }
                ]
            }
        });
    }
}