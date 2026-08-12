using WebCon.Plugins.Signing.Autenti.Common;
using WebCon.WorkFlow.SDK.Common;
using WebCon.WorkFlow.SDK.ConfigAttributes;

namespace WebCon.Plugins.Signing.Autenti.SendForSignature;

public class SendForSignatureConfig : PluginConfiguration
{
    [ConfigStudioTranslation("Uwierzytelnienie", TranslationCulture.plPL)]
    [ConfigGroupBox(DisplayName = "Authorization", Order = 0)]
    public AuthorizationConfig Authorization { get; set; }

    [ConfigStudioTranslation("Właściwości dokumentu", TranslationCulture.plPL)]
    [ConfigGroupBox(DisplayName = "Document Details", Order = 1)]
    public DocumentDetailsConfig DocumentDetails { get; set; }

    [ConfigStudioTranslation("Załączniki", TranslationCulture.plPL)]
    [ConfigGroupBox(DisplayName = "Attachments", Order = 2)]
    public AttachmentsConfig Attachments { get; set; }

    [ConfigStudioTranslation("Odbiorcy", TranslationCulture.plPL)]
    [ConfigGroupBox(DisplayName = "Recipients", Order = 3)]
    public RecipentsConfig Recipents { get; set; }

    [ConfigStudioTranslation("Dane wyjściowe", TranslationCulture.plPL)]
    [ConfigGroupBox(DisplayName = "Response", Order = 4)]
    public ResponseConfig Response { get; set; }
}

public class DocumentDetailsConfig
{
    [ConfigStudioTranslation("Nazwa dokumentu", TranslationCulture.plPL)]
    [ConfigEditableText("Document Name", true)]
    public string DocumentName { get; set; }

    [ConfigStudioTranslation("Wiadomość do odbiorców", TranslationCulture.plPL)]
    [ConfigEditableText("Message to receipients")]
    public string MessageToReceipients { get; set; }

    [ConfigStudioTranslation("Język procesu", TranslationCulture.plPL, Description = "Dopuszczalne wartości: cs, da, de, el, en, es, fi, fr, ga, hr, hu, it, lt, lv, mt, nl, no, pl, ro, ru, sk, sl, sv, uk")]
    [ConfigEditableText("Process language", true, DefaultText = "pl", Description = "Allowed values: cs, da, de, el, en, es, fi, fr, ga, hr, hu, it, lt, lv, mt, nl, no, pl, ro, ru, sk, sl, sv, uk")]
    public string ProcessLanguage { get; set; }

    [ConfigStudioTranslation("Wyślij jako organizacja", TranslationCulture.plPL)]
    [ConfigEditableBool("Send document as organisation", DefaultValue = false)]
    public bool SendAsOrganization { get; set; }

    [ConfigStudioTranslation("Wizualizacja podpisu", TranslationCulture.plPL, Description = "W przypadku wyboru opcji 'Configuration_from_field', należy skonfigurować pole z wizualizacją podpisu")]
    [ConfigEditableEnum("Visual representation of the signature", Description = "When 'Configuration_from_field' is selected, the signature visualization field must be configured")]
    public SignatureRepresentation SignatureVisualRepresentation { get; set; }

    [ConfigStudioTranslation("Pole z wizualizacją podpisu", TranslationCulture.plPL, Description = "Opcjonalne pole, należy skonfigurować tylko kiedy w Wizualizacji podpisu wybrano opcję 'Configuration_from_field'")]
    [ConfigEditableFormFieldID("Signature visualization field", FormFieldTypes = FormFieldTypes.Choice, Description = "Optional field, should be configured only when 'Configuration_from_field' is selected in Visual representation of the signature")]
    public int? SignatureVisualizationFieldId { get; set; }

    [ConfigStudioTranslation("ID etykiety", TranslationCulture.plPL, Description = "Przy przekazaniu wielu etykiet wartości należy rozdzielić za pomocą średnika np. IdEtykiety1;IdEtykiety2")]
    [ConfigEditableText("Label ID", Description = "When passing multiple labels, values should be separated with a semicolon e.g. LabelId1;LabelId2")]
    public string TagsId { get; set; }

    [ConfigStudioTranslation("Pole z etykietami", TranslationCulture.plPL, Description = "Pole wyboru z etykietami, id wartości w źródle powinno odpowiadać id etykiety")]
    [ConfigEditableFormFieldID("Labels field", FormFieldTypes = FormFieldTypes.Choice, Description = "Choice field with labels, ID in the source should correspond to the label ID")]
    public int? TagsIdFieldId { get; set; }
}

public enum SignatureRepresentation
{
    Signature_on_additional_page_at_end_of_document = 0,
    Signature_in_any_place_chosen_by_the_recipient = 1,
    Configuration_from_field = 2,
}

public class AttachmentsConfig
{
    [ConfigStudioTranslation("Tryb wyboru", TranslationCulture.plPL)]
    [ConfigEditableEnum("Selection mode")]
    public AttachmentsChoosingOptions AttachmentsChoosingOption { get; set; }

    [ConfigStudioTranslation("Tryb kategorii", TranslationCulture.plPL)]
    [ConfigEditableEnum("Category mode")]
    public CategorySelectionOptions CategorySelectionOptions { get; set; }

    [ConfigStudioTranslation("Id kategorii", TranslationCulture.plPL)]
    [ConfigEditableText("Category ID")]
    public string GroupID { get; set; }

    [ConfigStudioTranslation("Wyrażenie regularne", TranslationCulture.plPL)]
    [ConfigEditableText("Regular expression")]
    public string AttRegularExpression { get; set; }

    [ConfigStudioTranslation("Zapytanie SQL", TranslationCulture.plPL)]
    [ConfigEditableText("SQL query", Multiline = true, TagEvaluationMode = EvaluationMode.SQL)]
    public string AttQuery { get; set; }
}

public enum AttachmentsChoosingOptions
{
    Category = 0,
    SQL = 1
}

public enum CategorySelectionOptions
{
    ID = 0,
    All = 1,
    None = 2
}

public class RecipentsConfig
{
    [ConfigStudioTranslation("Lista odbiorców", TranslationCulture.plPL)]
    [ConfigEditableItemList("Recipients list")]
    public RecipentsListConfig RecipentsList { get; set; }

    [ConfigStudioTranslation("Uwzględnij kolejność podpisów", TranslationCulture.plPL, Description = "Pole powinno zawierać tekst \"False\" lub \"True\"")]
    [ConfigEditableText("Include Signing Order", IsRequired = true, DefaultText = "False", Description = "The field should contain the text \"False\" or \"True\"")]
    public bool IncludeSigningOrder { get; set; }
}

public class RecipentsListConfig : IConfigEditableItemList
{
    public int ItemListId { get; set; }

    [ConfigStudioTranslation("Imię", TranslationCulture.plPL)]
    [ConfigEditableItemListColumnID("First name", true)]
    public int FirstName { get; set; }

    [ConfigStudioTranslation("Nazwisko", TranslationCulture.plPL)]
    [ConfigEditableItemListColumnID("Last name", true)]
    public int LastName { get; set; }

    [ConfigStudioTranslation("Email", TranslationCulture.plPL)]
    [ConfigEditableItemListColumnID("Email", true)]
    public int Email { get; set; }

    [ConfigStudioTranslation("Rola", TranslationCulture.plPL)]
    [ConfigEditableItemListColumnID("Role", true)]
    public int Role { get; set; }

    [ConfigStudioTranslation("Rodzaj podpisu", TranslationCulture.plPL)]
    [ConfigEditableItemListColumnID("Signature Type", true)]
    public int SignatureType { get; set; }

    [ConfigStudioTranslation("Numer telefonu", TranslationCulture.plPL)]
    [ConfigEditableItemListColumnID("Phone Number")]
    public int? PhoneNumber { get; set; }

    [ConfigStudioTranslation("Autoryzacja odbiorcy kodem SMS", TranslationCulture.plPL)]
    [ConfigEditableItemListColumnID("Recipient SMS authorization", ItemListColumnTypes = ItemListColumnTypes.Boolean)]
    public int? SmsAuthorization { get; set; }

    [ConfigStudioTranslation("Zabezpieczenie dokumentu kodem SMS", TranslationCulture.plPL)]
    [ConfigEditableItemListColumnID("Unlock document by SMS", ItemListColumnTypes = ItemListColumnTypes.Boolean)]
    public int? SmsUnlock { get; set; }

    [ConfigStudioTranslation("Kolejność podpisu", TranslationCulture.plPL)]
    [ConfigEditableItemListColumnID("Signing Order", ItemListColumnTypes = ItemListColumnTypes.Decimal | ItemListColumnTypes.CalculatedDecimal)]
    public int? SigningOrder { get; set; }

    [ConfigStudioTranslation("Status podpisu", TranslationCulture.plPL)]
    [ConfigEditableItemListColumnID("Signature Status")]
    public int? SignatureStatus { get; set; }

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

    [ConfigStudioTranslation("Autenti ID", TranslationCulture.plPL)]
    [ConfigEditableItemListColumnID("Autenti ID")]
    public int? AutentiId { get; set; }
}

public class ResponseConfig
{
    [ConfigEditableFormFieldID("Autenti ID")]
    public int AutentiId { get; set; }
}