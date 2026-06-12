using System.Collections.Generic;
using WebCon.Plugins.Signing.Autenti.Api.Models;

namespace WebCon.Plugins.Signing.Autenti.SendForSignature.Providers.Enrichers;

public class CoreDataEnricher : IPartyEnricher
{
    private const string ContactType = "CONTACT-TYPE:EMAIL";

    public void Enrich(Party party, IItemListRowContext row)
    {
        var participant = new Participant();

        participant.FirstName = row.FirstName;
        participant.LastName = row.LastName;
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
}