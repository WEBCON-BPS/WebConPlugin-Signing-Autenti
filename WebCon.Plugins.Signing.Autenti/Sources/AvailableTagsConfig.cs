using WebCon.Plugins.Signing.Autenti.Common;
using WebCon.WorkFlow.SDK.Common;
using WebCon.WorkFlow.SDK.ConfigAttributes;

namespace WebCon.Plugins.Signing.Autenti.Sources
{
    public class AvailableTagsConfig : PluginConfiguration
    {
        [ConfigStudioTranslation("Uwierzytelnienie", TranslationCulture.plPL)]
        [ConfigGroupBox(DisplayName = "Authorization", Order = 0)]
        public AuthorizationConfig Authorization { get; set; }
    }
}