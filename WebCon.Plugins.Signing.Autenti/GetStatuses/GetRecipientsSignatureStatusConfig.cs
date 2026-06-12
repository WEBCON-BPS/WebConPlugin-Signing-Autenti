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
}