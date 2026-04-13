using WebCon.Plugins.Signing.Autenti.Common;
using WebCon.WorkFlow.SDK.Common;
using WebCon.WorkFlow.SDK.ConfigAttributes;

namespace WebCon.Plugins.Signing.Autenti.SignLink
{
    public class GetSignLinkConfig : PluginConfiguration
    {
        [ConfigStudioTranslation("Uwierzytelnienie", TranslationCulture.plPL)]
        [ConfigGroupBox(DisplayName = "Authorization", Order = 0)]
        public AuthorizationConfig Authorization { get; set; }

        [ConfigStudioTranslation("Dane wejściowe", TranslationCulture.plPL)]
        [ConfigGroupBox(DisplayName = "Input", Order = 1)]
        public SignLinkInput Input { get; set; }

        [ConfigStudioTranslation("Dane wyjściowe", TranslationCulture.plPL)]
        [ConfigGroupBox(DisplayName = "Response", Order = 2)]
        public SignLinkResponse Response { get; set; }
    }

    public class SignLinkInput {

        [ConfigStudioTranslation("Tryb działania", TranslationCulture.plPL, Description = "Redirect - adres email jest pobierany z pola email i użytkownik zostaje automatycznie przekierowany na link, Field - adres email jest pobierany z pole email i link jest zapisywany do pola, List - adresy email są pobierane z listy pozycji i dla każdego link jest zapisywany na liście pozycji we wskazanej kolumnie")]
        [ConfigEditableEnum("Mode", Description = "Redirect - the email address is retrieved from the email field and the user is automatically redirected to the link, Field - the email address is retrieved from the email field and the link is saved to the field, List - email addresses are retrieved from the item list and for each one the link is saved in the specified column of the item list")]
        public SignLinkActionMode ActionMode { get; set; }

        [ConfigEditableFormFieldID("Autenti ID", true)]
        public int AutentiIDField { get; set; }

        [ConfigStudioTranslation("Email", TranslationCulture.plPL, Description = "Używany tylko w trybie działania akcji \"Redirect\" i \"Field\"")]
        [ConfigEditableText("Email", Description = "Used only in \"Redirect\" and \"Field\" action modes")]
        public string UserEmail { get; set; }
    }

    public class SignLinkResponse
    {
        [ConfigStudioTranslation("Pole na link", TranslationCulture.plPL)]
        [ConfigEditableFormFieldID("Field for link")]
        public int? LinkField { get; set; }

        [ConfigStudioTranslation("Lista odbiorców", TranslationCulture.plPL)]
        [ConfigEditableItemList("Recipients list")]
        public RecipientsList RecipientsListMapper { get; set; }
    }

    public enum SignLinkActionMode
    {
        Redirect = 0,
        Field = 1,
        List = 2
    }

    public class RecipientsList : IConfigEditableItemList
    {
        public int ItemListId { get; set; }

        [ConfigEditableItemListColumnID("Email")]
        public int Email { get; set; }

        [ConfigStudioTranslation("Link do podpisu", TranslationCulture.plPL)]
        [ConfigEditableItemListColumnID("Signature Link")]
        public int SignatureLink { get; set; }
    }

}