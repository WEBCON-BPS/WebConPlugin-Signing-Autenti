using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebCon.Plugins.Signing.Autenti.Api.Models;
using WebCon.Plugins.Signing.Autenti.SendForSignature.Providers.Enrichers;
using WebCon.WorkFlow.SDK.ActionPlugins.Model;

namespace WebCon.Plugins.Signing.Autenti.SendForSignature.Providers;

public class PartiesProvider
    (IReadOnlyList<IPartyEnricher> enrichers, RecipentsConfig config, ActionContextInfo context) 
    : IPartiesProvider
{
    public Task<List<Party>> GetPartiesAsync()
    {
        var itemList = context.CurrentDocument.ItemsLists.GetByID(config.RecipentsList.ItemListId);
        context.PluginLogger.AppendDebug($"Loaded item list {config.RecipentsList.ItemListId} with {itemList.Rows.Count} row(s)");

        var rowContexts = itemList.Rows
            .Select(row => (IItemListRowContext)new ItemListRowContext(row, config.RecipentsList))
            .ToList();

        var validator = new PartiesValidator(config.IncludeSigningOrder);
        validator.Validate(rowContexts);

        var parties = new List<Party>();

        foreach (var rowContext in rowContexts)
        {
            var party = new Party();

            foreach (var enricher in enrichers)
                enricher.Enrich(party, rowContext);

            parties.Add(party);
        }

        context.PluginLogger.AppendDebug($"Built {parties.Count} party/ies: {string.Join(", ", parties.Select(p => p.Role))}");
        return Task.FromResult(parties);
    }
}