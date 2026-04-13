using WebCon.WorkFlow.SDK.Common;
using WebCon.WorkFlow.SDK.ConfigAttributes;

namespace WebCon.Plugins.Signing.Autenti.Sources;

public class SignatureTypesConfig : PluginConfiguration
{
    [ConfigStudioTranslation("Konfiguracji dostępności typów podpisów", TranslationCulture.plPL)]
    [ConfigGroupBox(DisplayName = "Available signature types configuration")]
    public SignaturesEnabledConfig AvailableSignature { get; set; }
}

public class SignaturesEnabledConfig
{
    [ConfigStudioTranslation("E-podpis Autenti", TranslationCulture.plPL)]
    [ConfigEditableBool("Autenti E-signature", DefaultValue = true)]
    public bool IsESignatureEnabled { get; set; }

    [ConfigStudioTranslation("Podpis kwalifikowany", TranslationCulture.plPL)]
    [ConfigEditableBool("Qualified signature", DefaultValue = true)]
    public bool IsQualifiedSignatureEnabled { get; set; }

    [ConfigStudioTranslation("Zaawansowany e-podpis Autenti", TranslationCulture.plPL)]
    [ConfigEditableBool("Advanced Autenti e-signature", DefaultValue = true)]
    public bool IsAdvancedESignatureEnabled { get; set; }
}