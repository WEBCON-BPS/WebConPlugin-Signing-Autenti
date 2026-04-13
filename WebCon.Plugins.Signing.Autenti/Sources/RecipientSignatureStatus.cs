using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using WebCon.WorkFlow.SDK.Common;
using WebCon.WorkFlow.SDK.Common.Model;
using WebCon.WorkFlow.SDK.DataSourcePlugins;
using WebCon.WorkFlow.SDK.DataSourcePlugins.Model;

namespace WebCon.Plugins.Signing.Autenti.Sources;

public class RecipientSignatureStatus : CustomDataSource<PluginConfiguration>
{
    private static readonly (string id, string value, string translation)[] Statuses =
    [
        ("NONE", "Not started", "Nierozpoczęty"),
        ("PENDING", "Pending", "Oczekujący"),
        ("COMPLETED", "Completed", "Zakończony"),
        ("OBSOLETE", "Obsolete", "Nieaktualny"),
        ("FAILED", "Failed", "Niepowodzenie"),
    ];

    public override Task<List<DataSourceColumn>> GetColumnsAsync()
        => Task.FromResult(new List<DataSourceColumn>
       {
           new("ID"),
           new("Name"),
           new("Name_PL"),
       });

    public override Task<DataTable> GetDataAsync(SearchConditions searchConditions)
    {
        var table = new DataTable("result");
        table.Columns.Add("ID", typeof(string));
        table.Columns.Add("Name", typeof(string));
        table.Columns.Add("Name_PL", typeof(string));

        foreach (var (id, value, translation) in Statuses)
            table.Rows.Add(id, value, translation);

        return Task.FromResult(table);
    }
}