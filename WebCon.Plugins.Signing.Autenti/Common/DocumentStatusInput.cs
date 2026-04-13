using WebCon.WorkFlow.SDK.ConfigAttributes;

namespace WebCon.Plugins.Signing.Autenti.Common;

public class DocumentStatusInput
{
    [ConfigEditableFormFieldID("Autenti ID", true)]
    public int AutentiIDField { get; set; }
}