using WebCon.Plugins.Signing.Autenti.Api.Models;

namespace WebCon.Plugins.Signing.Autenti.SendForSignature.Providers.Enrichers;

public class SigningOrderEnricher(bool includeSigningOrder) : IPartyEnricher
{
    public void Enrich(Party party, IItemListRowContext row)
    {
        if (!includeSigningOrder || !row.SigningOrder.HasValue)
            return;

        if (party.Role != AutentiRoles.Signer && party.Role != AutentiRoles.Approver)
            return;

        party.Constraints.Add(new Constraint()
        {
            Classifiers = [AutentiConstraints.ParticipationPriority],
            Attributes = new ConstraintAttributes() { Priority = row.SigningOrder.Value }
        });
    }
}