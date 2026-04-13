namespace WebCon.Plugins.Signing.Autenti.SendForSignature.Providers.Enrichers;

public interface IItemListRowContext
{
    string Name { get; }
    string Email { get; }
    string Role { get; }
    string SignatureType { get; }
    string PhoneNumber { get; }
    bool? SmsAuthorization { get; }
    bool? SmsUnlock { get; }
    int? SigningOrder { get; }
    bool? Representative { get; }
    string VAT { get; }
    string OrganisationName { get; }
    string Position { get; }
}