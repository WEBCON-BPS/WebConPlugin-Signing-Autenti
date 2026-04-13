using System;
using System.Collections.Generic;
using System.Linq;
using WebCon.Plugins.Signing.Autenti.Api.Models;

namespace WebCon.Plugins.Signing.Autenti.SendForSignature.Providers.Enrichers;

public class CoreDataEnricher : IPartyEnricher
{
    private const string ContactType = "CONTACT-TYPE:EMAIL";

    public void Enrich(Party party, IItemListRowContext row)
    {
        var participant = new Participant();

        var (firstName, lastName) = SplitName(row.Name);
        participant.FirstName = firstName;
        participant.LastName = lastName;
        participant.Name = row.Name;
        participant.Contacts = CreateContacts(row.Email);

        party.Participant = participant;
        party.Role = row.Role;
    }

    private List<Contact> CreateContacts(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return null;

        return
        [
            new() {
                Type = ContactType,
                Attributes = new ContactAttributes { Email = email.Trim() }
            }
        ];
    }

    private static (string firstName, string lastName) SplitName(string fullName)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            return (null, null);

        var parts = fullName.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        return (parts.First(), parts.Last());
    }
}