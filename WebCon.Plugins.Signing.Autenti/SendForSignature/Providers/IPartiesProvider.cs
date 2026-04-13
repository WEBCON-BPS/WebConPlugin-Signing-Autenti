using System.Collections.Generic;
using System.Threading.Tasks;
using WebCon.Plugins.Signing.Autenti.Api.Models;

namespace WebCon.Plugins.Signing.Autenti.SendForSignature.Providers;

public interface IPartiesProvider
{
    Task<List<Party>> GetPartiesAsync();
}