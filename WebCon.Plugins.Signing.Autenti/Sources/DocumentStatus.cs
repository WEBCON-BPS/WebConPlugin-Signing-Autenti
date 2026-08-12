using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using WebCon.WorkFlow.SDK.Common;
using WebCon.WorkFlow.SDK.Common.Model;
using WebCon.WorkFlow.SDK.DataSourcePlugins;
using WebCon.WorkFlow.SDK.DataSourcePlugins.Model;

namespace WebCon.Plugins.Signing.Autenti.Sources;

public class DocumentStatus : CustomDataSource<PluginConfiguration>
{
    private static readonly string[] Statuses =
    [
        "PROCESSING",
        "COMPLETED ",
        "WITHDRAWN",
        "ERROR",
        "REJECTED"
    ];

    public override Task<List<DataSourceColumn>> GetColumnsAsync()
        => Task.FromResult(new List<DataSourceColumn>
       {
           new("ID"),
           new("Name"),
       });

    public override Task<DataTable> GetDataAsync(SearchConditions searchConditions)
    {
        var table = new DataTable("result");
        table.Columns.Add("ID", typeof(string));
        table.Columns.Add("Name", typeof(string));

        foreach (var status in Statuses)
            table.Rows.Add(status, status);

        return Task.FromResult(table);
    }
}