using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using WebCon.WorkFlow.SDK.Common;
using WebCon.WorkFlow.SDK.Common.Model;
using WebCon.WorkFlow.SDK.DataSourcePlugins;
using WebCon.WorkFlow.SDK.DataSourcePlugins.Model;

namespace WebCon.Plugins.Signing.Autenti.Sources;

public class AvailableLanguages : CustomDataSource<PluginConfiguration>
{
    private static readonly List<string> languages =
    [
        "cs", "da", "de", "el", "en", "es", "fi", "fr", "ga", "hr", "hu", "it", "lt", "lv", "mt", "nl", "no", "pl", "ro", "ru", "sk", "sl", "sv", "uk"
    ];

    public override Task<List<DataSourceColumn>> GetColumnsAsync()
    {
        return Task.FromResult(new List<DataSourceColumn>
        {
            new("Id"),
            new("LanguageCode")
        });
    }

    public override Task<DataTable> GetDataAsync(SearchConditions searchConditions)
    {
        var dt = new DataTable();
        dt.Columns.Add("Id");
        dt.Columns.Add("LanguageCode");

        for (int i = 0; i < languages.Count; i++)
            dt.Rows.Add(i + 1, languages[i]);

        return Task.FromResult(dt);
    }
}