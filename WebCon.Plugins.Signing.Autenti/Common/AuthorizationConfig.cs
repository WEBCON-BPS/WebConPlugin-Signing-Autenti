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

    [ConfigStudioTranslation("Unikalny identyfikator organizacji", TranslationCulture.plPL)]
    [ConfigEditableText("Organization unique identifier", true)]
    public string Issuer { get; set; }

    [ConfigStudioTranslation("Unikalny identyfikator użytkownika", TranslationCulture.plPL)]
    [ConfigEditableText("User unique identifier", true)]
    public string Subject { get; set; }

    [ConfigStudioTranslation("Użyj proxy", TranslationCulture.plPL)]
    [ConfigEditableBool("Use proxy")]
    public bool UseProxy { get; set; }
}