using WebCon.Plugins.Signing.Autenti.Common;
using WebCon.WorkFlow.SDK.Common;
using WebCon.WorkFlow.SDK.ConfigAttributes;

namespace WebCon.Plugins.Signing.Autenti.Withdrawal
{
    public class WithdrawalConfig : PluginConfiguration
    {
        [ConfigStudioTranslation("Uwierzytelnienie", TranslationCulture.plPL)]
        [ConfigGroupBox(DisplayName = "Authorization", Order = 0)]
        public AuthorizationConfig Authorization { get; set; }

        [ConfigStudioTranslation("Dane wejściowe", TranslationCulture.plPL)]
        [ConfigGroupBox(DisplayName = "Input", Order = 1)]
        public WithdrawalInput Input { get; set; }
    }

    public class WithdrawalInput
    {

        [ConfigEditableFormFieldID("Autenti ID", true)]
        public int AutentiIDField { get; set; }
    }
}