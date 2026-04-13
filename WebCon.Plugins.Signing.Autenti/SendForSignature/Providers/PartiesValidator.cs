using System;
using System.Collections.Generic;
using System.Linq;
using WebCon.Plugins.Signing.Autenti.Api.Models;
using WebCon.Plugins.Signing.Autenti.SendForSignature.Providers.Enrichers;

namespace WebCon.Plugins.Signing.Autenti.SendForSignature.Providers;

public class PartiesValidator(bool includeSigningOrder)
{
    public void Validate(IReadOnlyList<IItemListRowContext> rows)
    {
        ValidateRequiredFields(rows);
        ValidateUniqueEmails(rows);
        ValidateSigningOrderRequired(rows);
        ValidateUniqueSigningOrders(rows);
        ValidateApproverRules(rows);
        ValidateAdvancedSignatureRequirements(rows);
        ValidateQualifiedSignatureRequirements(rows);
    }

    private static void ValidateRequiredFields(IReadOnlyList<IItemListRowContext> rows)
    {
        foreach (var row in rows)
        {
            if (string.IsNullOrWhiteSpace(row.Name))
                throw new InvalidOperationException(
                    "Recipient name is required. Ensure all recipients have a name specified.");

            if (string.IsNullOrWhiteSpace(row.Email))
                throw new InvalidOperationException(
                    "Recipient email is required. Ensure all recipients have an email address specified.");
        }
    }

    private static void ValidateUniqueEmails(IReadOnlyList<IItemListRowContext> rows)
    {
        var duplicates = rows
            .GroupBy(r => r.Email, StringComparer.OrdinalIgnoreCase)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToList();

        if (duplicates.Count > 0)
            throw new InvalidOperationException(
                $"Email addresses must be unique. Duplicates found: {string.Join(", ", duplicates)}.");
    }

    private static void ValidateUniqueSigningOrders(IReadOnlyList<IItemListRowContext> rows)
    {
        var duplicates = rows
            .Where(r => r.SigningOrder.HasValue)
            .GroupBy(r => r.SigningOrder!.Value)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToList();

        if (duplicates.Count > 0)
            throw new InvalidOperationException(
                $"Signing order must be unique. Duplicate order values: {string.Join(", ", duplicates)}.");
    }

    private void ValidateApproverRules(IReadOnlyList<IItemListRowContext> rows)
    {
        var approvers = rows
            .Where(r => string.Equals(r.Role, AutentiRoles.Approver, StringComparison.OrdinalIgnoreCase))
            .ToList();

        if (approvers.Count == 0)
            return;

        if (!includeSigningOrder)
            throw new InvalidOperationException(
                "When an approver is added, signing order must be enabled.");

        var maxOrder = rows
            .Where(r => r.SigningOrder.HasValue)
            .Max(r => r.SigningOrder!.Value);

        var approversWithMaxOrder = approvers
            .Where(a => a.SigningOrder.HasValue && a.SigningOrder.Value == maxOrder)
            .ToList();

        if (approversWithMaxOrder.Count > 0)
            throw new InvalidOperationException(
                "An approver cannot have the highest signing order. The approver must sign before the last signer.");
    }

    private void ValidateSigningOrderRequired(IReadOnlyList<IItemListRowContext> rows)
    {
        if (!includeSigningOrder)
            return;

        var missingOrder = rows
            .Where(r => string.Equals(r.Role, AutentiRoles.Signer, StringComparison.OrdinalIgnoreCase)
                     || string.Equals(r.Role, AutentiRoles.Approver, StringComparison.OrdinalIgnoreCase))
            .Where(r => !r.SigningOrder.HasValue)
            .Select(r => r.Email)
            .ToList();

        if (missingOrder.Count > 0)
            throw new InvalidOperationException(
                $"Signing order number is required for Signer and Approver roles. Missing for: {string.Join(", ", missingOrder)}.");
    }

    private static void ValidateAdvancedSignatureRequirements(IReadOnlyList<IItemListRowContext> rows)
    {
        var invalidRows = rows
            .Where(r => string.Equals(r.SignatureType, AutentiSignatureTypes.Advanced, StringComparison.OrdinalIgnoreCase))
            .Where(r => r.SmsAuthorization != true)
            .Select(r => r.Email)
            .ToList();

        if (invalidRows.Count > 0)
            throw new InvalidOperationException(
                $"Advanced signature requires SMS authorization to be enabled. Missing for: {string.Join(", ", invalidRows)}.");
    }

    private static void ValidateQualifiedSignatureRequirements(IReadOnlyList<IItemListRowContext> rows)
    {
        var invalidRows = rows
            .Where(r => string.Equals(r.SignatureType, AutentiSignatureTypes.Qualified, StringComparison.OrdinalIgnoreCase))
            .Where(r => r.SmsAuthorization == true)
            .Select(r => r.Email)
            .ToList();

        if (invalidRows.Count > 0)
            throw new InvalidOperationException(
                $"Qualified signature does not support SMS authorization. Only SMS document unlock is allowed as additional security. Disable SMS authorization for: {string.Join(", ", invalidRows)}.");
    }
}