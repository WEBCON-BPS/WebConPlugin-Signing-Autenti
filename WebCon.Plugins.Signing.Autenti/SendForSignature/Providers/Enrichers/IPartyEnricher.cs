using WebCon.Plugins.Signing.Autenti.Api.Models;

namespace WebCon.Plugins.Signing.Autenti.SendForSignature.Providers.Enrichers;

public interface IPartyEnricher
{
    void Enrich(Party party, IItemListRowContext row);
}