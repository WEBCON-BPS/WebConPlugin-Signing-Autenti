using WebCon.WorkFlow.SDK.ConfigAttributes;

namespace WebCon.Plugins.Signing.Autenti.Common;

public class AuthorizationConfig
{
    [ConfigStudioTranslation("Połączenie", TranslationCulture.plPL)]
    [ConfigEditableConnectionID("Connection", true, ConnectionsType = DataConnectionType.WebServiceREST)]
    public int ConnectionId { get; set; }

    [ConfigStudioTranslation("Połączenie z kluczem PEM", TranslationCulture.plPL)]
    [ConfigEditableConnectionID("Connection with PEM key", true, ConnectionsType = DataConnectionType.WebServiceREST)]
    public int PemConnectionId { get; set; }

    [ConfigStudioTranslation("Email użytkownika w którego kontekście działa akcja", TranslationCulture.plPL)]
    [ConfigEditableText("User email in whose context the action is performed", true)]
    public string UserEmail { get; set; }

    [ConfigStudioTranslation("Unikalny identyfikator organizacji", TranslationCulture.plPL, Description = "Atrybut ten powinien być obecny i zawierać unikalny identyfikator organizacji wywołującej (w przyszłości może stać się obowiązkowy).")]
    [ConfigEditableText("Organization unique identifier", true, Description = "Attribute should be present, and shall contain unique identification of the calling organization (may become mandatory in the future).")]
    public string Issuer { get; set; }

    [ConfigStudioTranslation("Unikalny identyfikator użytkownika", TranslationCulture.plPL, Description = "Atrybut powinien zawierać stały, unikalny identyfikator użytkownika w systemie wywołującym (identyfikator użytkownika w systemie łączącym się za pośrednictwem API). Pole to powinno być obecne i jest zarezerwowane do wykorzystania w przyszłości.")]
    [ConfigEditableText("User unique identifier", true, Description = "Field should contain the permanent unique identifier of the user in the calling system (the identifier of the user in the system that connects via the API). The field should be present, and is reserved for future use.")]
    public string Subject { get; set; }

    [ConfigStudioTranslation("Identyfikator klucza (kid)", TranslationCulture.plPL, Description = "Identyfikator klucza prywatnego. Musi odpowiadać wartości 'kid' przypisanej do klucza publicznego w konfiguracji aplikacji Autenti. Jeżeli klucz publiczny nie ma ustawionej wartości kid pole należy zostawić puste")]
    [ConfigEditableText("Key ID (kid)", Description = "The identifier of the private key. Must match the 'kid' value assigned to the public key in your Autenti application configuration. If the public key does not have a kid value set, leave this field empty.")]
    public string KeyId { get; set; }

    [ConfigStudioTranslation("Użyj proxy", TranslationCulture.plPL)]
    [ConfigEditableBool("Use proxy")]
    public bool UseProxy { get; set; }


}