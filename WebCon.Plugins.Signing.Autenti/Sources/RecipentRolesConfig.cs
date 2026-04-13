using WebCon.WorkFlow.SDK.Common;
using WebCon.WorkFlow.SDK.ConfigAttributes;

namespace WebCon.Plugins.Signing.Autenti.Sources;

public class RecipentRolesConfig : PluginConfiguration
{
    [ConfigStudioTranslation("Konfiguracji dostępności ról", TranslationCulture.plPL)]
    [ConfigGroupBox(DisplayName = "Available roles configuration")]
    public RoleEnabledConfig AvailableRoles { get; set; }
}

public class RoleEnabledConfig
{
    [ConfigStudioTranslation("Podpisujący", TranslationCulture.plPL)]
    [ConfigEditableBool("Signer", DefaultValue = true)]
    public bool IsSignerEnabled { get; set; }

    [ConfigStudioTranslation("Parafujący", TranslationCulture.plPL)]
    [ConfigEditableBool("Approver", DefaultValue = true)]
    public bool IsApproverEnabled { get; set; }

    [ConfigStudioTranslation("Opiniujący", TranslationCulture.plPL)]
    [ConfigEditableBool("Reviewer", DefaultValue = true)]
    public bool IsReviewerEnabled { get; set; }

    [ConfigStudioTranslation("Do wglądu", TranslationCulture.plPL)]
    [ConfigEditableBool("Viewer", DefaultValue = true)]
    public bool IsViewerEnabled { get; set; }
}