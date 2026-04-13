using System;
using System.Collections.Generic;
using WebCon.Plugins.Signing.Autenti.Api.Models;

namespace WebCon.Plugins.Signing.Autenti.SendForSignature.Providers.Enrichers;

public class SmsEnricher : IPartyEnricher
{
    public void Enrich(Party party, IItemListRowContext row)
    {
        AddSmsUnlock(party.Constraints, row);
        AddSmsAuth(party.Constraints, row);
    }

    private void AddSmsUnlock(List<Constraint> constraints, IItemListRowContext row)
    {
        if (row.SmsUnlock != true)
            return;

        constraints.Add(new Constraint()
        {
            ConstrainedActions = [AutentiActions.ReadSensitiveMetadata],
            Classifiers = [AutentiConstraints.PhoneNumberVerification],
            Attributes = new ConstraintAttributes() { PhoneNumber = GetPhoneNumberWithValidation(row) }
        });
    }

    private void AddSmsAuth(List<Constraint> constraints, IItemListRowContext row)
    {
        if (row.SmsAuthorization != true)
            return;

        constraints.Add(new Constraint()
        {
            ConstrainedActions = [AutentiActions.SignatureApplication],
            Classifiers = [AutentiConstraints.PhoneNumberVerification],
            Attributes = new ConstraintAttributes() { PhoneNumber = GetPhoneNumberWithValidation(row) }
        });
    }

    private string GetPhoneNumberWithValidation(IItemListRowContext row)
        => string.IsNullOrWhiteSpace(row.PhoneNumber)
            ? throw new Exception("Phone number is empty. You have to provide value for required field.")
            : row.PhoneNumber;
}