using System.Collections.Generic;

namespace WebCon.Plugins.Signing.Autenti.Api.Models
{
    public class ParticipantsDetailsResponse
    {
        public PartyInfo Party { get; set; }
    }

    public class PartyInfo
    {
        public string Id { get; set; }   
        public List<Contact> Contacts { get; set; }
    }
}
