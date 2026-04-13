using System;

namespace WebCon.Plugins.Signing.Autenti.Api;

public class DocumentLockException() : Exception("Dokument został zabezpieczony kodem SMS, nie można wykonać akcji.")
{

}