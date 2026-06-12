using System.Threading.Tasks;
using WebCon.Plugins.Signing.Autenti.Api.Models;
using WebCon.Plugins.Signing.Autenti.SendForSignature;

namespace WebCon.Plugins.Signing.Autenti.Api;

public interface IDocumentProcessRequestBuilder
{
    IDocumentProcessRequestBuilder WithDocumentDetails(DocumentDetailsConfig config);
    IDocumentProcessRequestBuilder WithOrganizationSender(DocumentDetailsConfig config);
    IDocumentProcessRequestBuilder WithSignatureVisualisation(DocumentDetailsConfig config);
    IDocumentProcessRequestBuilder WithParticipantsAsync();
    IDocumentProcessRequestBuilder WithAttachmentsAsync();
    IDocumentProcessRequestBuilder WithTags();
    Task<RequestDto> BuildAsync();
}