using System.Threading.Tasks;
using WebCon.Plugins.Signing.Autenti.Api.Models;
using WebCon.Plugins.Signing.Autenti.SendForSignature;

namespace WebCon.Plugins.Signing.Autenti.Api;

public interface IAutentiApiService
{
    Task<DocumentProcessResponse> SendForSignatureAsync(SendForSignatureConfig config);
}