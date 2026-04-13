namespace WebCon.Plugins.Signing.Autenti.Api.Models;

public static class AutentiActions
{
    public const string ReadSensitiveMetadata = "ACTION:READ_SENSITIVE_METADATA";
    public const string SignatureApplication = "ACTION:SIGNATURE_APPLICATION";
}

public static class AutentiClassifiers
{
    public const string ActionSelection = "CHALLENGE_CLASSIFIER-UNIQUE_TYPE:ACTION_SELECTION";
    public const string DocumentSent = "EVENT_CLASSIFIER-UNIQUE_TYPE:DOCUMENT_SENT";
    public const string RemindersSent = "EVENT_CLASSIFIER-UNIQUE_TYPE:DOCUMENT_REMINDER_SENT";
    public const string RemindersPartySelection = "CHALLENGE_CLASSIFIER-UNIQUE_TYPE:DOCUMENT_REMINDER_PARTY_SELECTION";
}

public static class AutentiConstraints
{
    public const string PhoneNumberVerification = "CONSTRAINT-UNIQUE_TYPE:PHONE_NUMBER_VERIFICATION_REQUIRED";
    public const string SignatureType = "CONSTRAINT-UNIQUE_TYPE:SIGNATURE_TYPE";
    public const string ParticipationPriority = "CONSTRAINT-UNIQUE_TYPE:PARTICIPATION_PRIORITY";
    public const string Visualisation = "CONSTRAINT-UNIQUE_TYPE:VISUALISATION";
    public const string IdentityVerification = "CONSTRAINT-UNIQUE_TYPE:IDENTITY_VERIFICATION";
}

public static class AutentiVisualisations
{
    public const string AutentiSignatureCard = "VISUALISATION:AUTENTI_SIGNATURE_CARD";
    public const string Manual = "VISUALISATION:MANUAL";
}

public static class AutentiRoles
{
    public const string Signer = "SIGNER";
    public const string Approver = "APPROVER";
    public const string Sender = "SENDER";
    public const string Reviewer = "REVIEWER";
    public const string Viewer = "VIEWER";
}

public static class AutentiSignatureTypes
{
    public const string Qualified = "QUALIFIED";
    public const string Basic = "BASIC";
    public const string Advanced = "ADVANCED";
}

public static class AutentiIdentification
{
    public const string ArmValue = "autenti-v3";
    public const string ReuseDuration = "PT24H";
    public const string TrustFramework = "eidas";
    public const string AssuranceLevel = "substantial";
}