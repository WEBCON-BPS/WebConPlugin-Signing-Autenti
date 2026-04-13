using WebCon.Plugins.Signing.Autenti.Common;
using WebCon.Plugins.Signing.Autenti.SignLink;
using WebCon.WorkFlow.SDK.Common;
using WebCon.WorkFlow.SDK.ConfigAttributes;

namespace WebCon.Plugins.Signing.Autenti.SendReminder
{
    public class SendReminderConfig : PluginConfiguration
    {
        [ConfigStudioTranslation("Uwierzytelnienie", TranslationCulture.plPL)]
        [ConfigGroupBox(DisplayName = "Authorization", Order = 0)]
        public AuthorizationConfig Authorization { get; set; }

        [ConfigStudioTranslation("Dane wejściowe", TranslationCulture.plPL)]
        [ConfigGroupBox(DisplayName = "Input", Order = 1)]
        public ReminderInput Input { get; set; }
    }

    public class ReminderInput
    {

        [ConfigEditableFormFieldID("Autenti ID", true)]
        public int AutentiIDField { get; set; }
    }
}