using WebCon.Plugins.Signing.Autenti.Common;
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

        [ConfigStudioTranslation("Lista odbiorców", TranslationCulture.plPL, Description = "Opcjonalna lista za której pomocą można zablokować wysyłanie przypomnień do wybranych odbiorców.")]
        [ConfigEditableItemList("Recipients list", Description = "Optional list that can be used to block sending reminders to selected recipients.")]
        public ReminderRecipientsList RecipientsListMapper { get; set; }

        [ConfigEditableFormFieldID("Autenti ID", true)]
        public int AutentiIDField { get; set; }
    }


    public class ReminderRecipientsList : IConfigEditableItemList
    {
        public int ItemListId { get; set; }

        [ConfigEditableItemListColumnID("Email")]
        public int? Email { get; set; }

        [ConfigStudioTranslation("Nie wysyłaj przypomnienia", TranslationCulture.plPL)]
        [ConfigEditableItemListColumnID("Dont send reminder", ItemListColumnTypes = ItemListColumnTypes.Boolean)]
        public int? BlockSend { get; set; }
    }
}