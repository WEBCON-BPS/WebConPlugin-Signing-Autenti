using System.Collections.Generic;

namespace WebCon.Plugins.Signing.Autenti.Api.Models;

public class RequestDto
{
    public DocumentProcessRequest DocumentRequest { get; set; }
    public List<FileData> Files { get; set; }
}

public class FileData
{
    public string Name { get; set; }
    public byte[] Content { get; set; }
}