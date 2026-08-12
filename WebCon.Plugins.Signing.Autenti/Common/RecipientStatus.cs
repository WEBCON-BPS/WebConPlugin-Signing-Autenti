namespace WebCon.Plugins.Signing.Autenti.Common;

public class RecipientStatus
{
    public string Email { get; set; }
    public string AutentiId { get; set; }
    public string Role { get; set; }
    public string ParticipationStatus { get; set; }
    public string RejectionReason { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string SignatureType { get; set; }
    public string PhoneNumber { get; set; }
    public bool SmsAuthorization { get; set; }
    public bool SmsUnlock { get; set; }
    public bool IsRepresentative { get; set; }
    public string OrganisationName { get; set; }
    public string Vat { get; set; }
    public string Position { get; set; }
}
