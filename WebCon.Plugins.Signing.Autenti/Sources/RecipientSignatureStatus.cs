using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using WebCon.Plugins.Signing.Autenti.Api.Models;
using WebCon.WorkFlow.SDK.Common;
using WebCon.WorkFlow.SDK.Common.Model;
using WebCon.WorkFlow.SDK.DataSourcePlugins;
using WebCon.WorkFlow.SDK.DataSourcePlugins.Model;

namespace WebCon.Plugins.Signing.Autenti.Sources;

public class RecipientSignatureStatus : CustomDataSource<PluginConfiguration>
{
    private static readonly (string id, string value, string translationPL, string translationDE)[] Statuses =
    [
        (AutentiStatuses.None, "Not started", "Nierozpoczęty", "Nicht begonnen"),
        (AutentiStatuses.Pending, "Pending", "Oczekujący", "Ausstehend"),
        (AutentiStatuses.Completed, "Completed", "Zakończony", "Abgeschlossen"),
        (AutentiStatuses.Obsolete, "Obsolete", "Nieaktualny", "Veraltet"),
        (AutentiStatuses.Failed, "Failed", "Niepowodzenie", "Fehlgeschlagen"),
        (AutentiStatuses.Rejected, "Rejected", "Odrzucony", "Abgelehnt"),
    ];

    public override Task<List<DataSourceColumn>> GetColumnsAsync()
        => Task.FromResult(new List<DataSourceColumn>
       {
           new("ID"),
           new("Name"),
           new("Name_PL"),
           new("Name_DE"),
       });

    public override Task<DataTable> GetDataAsync(SearchConditions searchConditions)
    {
        var table = new DataTable("result");
        table.Columns.Add("ID", typeof(string));
        table.Columns.Add("Name", typeof(string));
        table.Columns.Add("Name_PL", typeof(string));
        table.Columns.Add("Name_DE", typeof(string));

        foreach (var (id, value, translationPL, translationDE) in Statuses)
            table.Rows.Add(id, value, translationPL, translationDE);

        return Task.FromResult(table);
    }
}