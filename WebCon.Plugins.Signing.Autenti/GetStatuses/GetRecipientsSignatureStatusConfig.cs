using WebCon.Plugins.Signing.Autenti.Common;
using WebCon.WorkFlow.SDK.Common;
using WebCon.WorkFlow.SDK.ConfigAttributes;

namespace WebCon.Plugins.Signing.Autenti.GetStatuses;

public class GetRecipientsSignatureStatusConfig : PluginConfiguration
{
    [ConfigStudioTranslation("Uwierzytelnienie", TranslationCulture.plPL)]
    [ConfigGroupBox(DisplayName = "Authorization", Order = 0)]
    public AuthorizationConfig Authorization { get; set; }

    [ConfigStudioTranslation("Dane wejściowe", TranslationCulture.plPL)]
    [ConfigGroupBox(DisplayName = "Input", Order = 1)]
    public DocumentStatusInput Input { get; set; }

    [ConfigStudioTranslation("Dane wyjściowe", TranslationCulture.plPL)]
    [ConfigGroupBox(DisplayName = "Response", Order = 2)]
    public RecipientsStatusResponse Response { get; set; }
}

public class RecipientsStatusResponse
{
    [ConfigStudioTranslation("Lista odbiorców", TranslationCulture.plPL)]
    [ConfigEditableItemList("Recipients list")]
    public RecipientsList RecipientsListMapper { get; set; }
}

public class RecipientsList : IConfigEditableItemList
{
    public int ItemListId { get; set; }

    [ConfigEditableItemListColumnID("Email", true)]
    public int Email { get; set; }

    [ConfigStudioTranslation("Rola", TranslationCulture.plPL)]
    [ConfigEditableItemListColumnID("Role", true)]
    public int Role { get; set; }

    [ConfigStudioTranslation("Status podpisu", TranslationCulture.plPL)]
    [ConfigEditableItemListColumnID("Signature Status", true)]
    public int SignatureStatus { get; set; }

    [ConfigStudioTranslation("Powód odrzucenia", TranslationCulture.plPL)]
    [ConfigEditableItemListColumnID("Rejection reason", true)]
    public int RejectionReason { get; set; }

    [ConfigStudioTranslation("Autenti ID", TranslationCulture.plPL)]
    [ConfigEditableItemListColumnID("Autenti ID")]
    public int? AutentiId { get; set; }

    [ConfigStudioTranslation("Imię", TranslationCulture.plPL)]
    [ConfigEditableItemListColumnID("First name")]
    public int? FirstName { get; set; }

    [ConfigStudioTranslation("Nazwisko", TranslationCulture.plPL)]
    [ConfigEditableItemListColumnID("Last name")]
    public int? LastName { get; set; }

    [ConfigStudioTranslation("Rodzaj podpisu", TranslationCulture.plPL)]
    [ConfigEditableItemListColumnID("Signature Type")]
    public int? SignatureType { get; set; }

    [ConfigStudioTranslation("Numer telefonu", TranslationCulture.plPL)]
    [ConfigEditableItemListColumnID("Phone Number")]
    public int? PhoneNumber { get; set; }

    [ConfigStudioTranslation("Autoryzacja odbiorcy kodem SMS", TranslationCulture.plPL)]
    [ConfigEditableItemListColumnID("Recipient SMS authorization", ItemListColumnTypes = ItemListColumnTypes.Boolean)]
    public int? SmsAuthorization { get; set; }

    [ConfigStudioTranslation("Zabezpieczenie dokumentu kodem SMS", TranslationCulture.plPL)]
    [ConfigEditableItemListColumnID("Unlock document by SMS", ItemListColumnTypes = ItemListColumnTypes.Boolean)]
    public int? SmsUnlock { get; set; }

    [ConfigStudioTranslation("Rodzaj odbiorcy lub Reprezentant organizacji", TranslationCulture.plPL)]
    [ConfigEditableItemListColumnID("Representative of the organisation", ItemListColumnTypes = ItemListColumnTypes.Boolean)]
    public int? Representative { get; set; }

    [ConfigStudioTranslation("NIP", TranslationCulture.plPL)]
    [ConfigEditableItemListColumnID("VAT")]
    public int? VAT { get; set; }

    [ConfigStudioTranslation("Nazwa organizacji", TranslationCulture.plPL)]
    [ConfigEditableItemListColumnID("Name of organisation")]
    public int? OrganisationName { get; set; }

    [ConfigStudioTranslation("Stanowisko w organizacji", TranslationCulture.plPL)]
    [ConfigEditableItemListColumnID("Position in the organisation")]
    public int? Position { get; set; }
}