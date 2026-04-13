using WebCon.Plugins.Signing.Autenti.Common;
using WebCon.WorkFlow.SDK.Common;
using WebCon.WorkFlow.SDK.ConfigAttributes;

namespace WebCon.Plugins.Signing.Autenti.GetStatuses;

public class GetDocumentStatusConfig : PluginConfiguration
{
    [ConfigStudioTranslation("Uwierzytelnienie", TranslationCulture.plPL)]
    [ConfigGroupBox(DisplayName = "Authorization", Order = 0)]
    public AuthorizationConfig Authorization { get; set; }

    [ConfigStudioTranslation("Dane wejściowe", TranslationCulture.plPL)]
    [ConfigGroupBox(DisplayName = "Input", Order = 1)]
    public DocumentStatusInput Input { get; set; }

    [ConfigStudioTranslation("Dane wyjściowe", TranslationCulture.plPL)]
    [ConfigGroupBox(DisplayName = "Response", Order = 2)]
    public DocumentStatusResponse Response { get; set; }
}

public class DocumentStatusResponse
{
    [ConfigStudioTranslation("Status w Autenti", TranslationCulture.plPL)]
    [ConfigEditableFormFieldID("Autenti Status", true)]
    public int AutentiStatusField { get; set; }
}