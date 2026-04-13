namespace WebCon.Plugins.Signing.Autenti.Api.Models
{
    public class ReminderChallengeRequest
    {
        public string[] classifiers { get; set; }
        public ReminderChallengeRequestAttributes attributes { get; set; }
    }

    public class ReminderChallengeRequestAttributes
    {
        public string[] selectedIds { get; set; }
    }
}