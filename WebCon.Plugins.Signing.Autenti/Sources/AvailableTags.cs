using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using WebCon.Plugins.Signing.Autenti.Api;
using WebCon.Plugins.Signing.Autenti.Api.Models;
using WebCon.WorkFlow.SDK.Common.Model;
using WebCon.WorkFlow.SDK.DataSourcePlugins;
using WebCon.WorkFlow.SDK.DataSourcePlugins.Model;
using WebCon.WorkFlow.SDK.Tools.Data;

namespace WebCon.Plugins.Signing.Autenti.Sources
{
    public class AvailableTags : CustomDataSource<AvailableTagsConfig>
    {
        public override Task<List<DataSourceColumn>> GetColumnsAsync()
        {
            return Task.FromResult(new List<DataSourceColumn>() {
                new DataSourceColumn("Id"),
                new DataSourceColumn("Type"),
                new DataSourceColumn("Description"),
            });
        }

        public override async Task<DataTable> GetDataAsync(SearchConditions searchConditions)
        {
            var clientProvider = new AutentiClientProvider(new ConnectionsHelper(base.Context), Configuration.Authorization, base.Context.PluginLogger);
            var authenticatedClient = await clientProvider.GetAuthenticatedClientAsync();
            using var httpClient = new AutentiHttpClient(authenticatedClient, base.Context.PluginLogger);
            base.Context.PluginLogger.AppendInfo("Gettings tags");
            var tags = await httpClient.GetTagsAsync();
            return MapToDt(tags);
        }

        private DataTable MapToDt(List<TagsResponse> tags)
        {
            base.Context.PluginLogger.AppendInfo("Mapping tags to table");
            var dt = new DataTable();
            dt.Columns.Add("Id");
            dt.Columns.Add("Type");
            dt.Columns.Add("Description");
            foreach (var tag in tags.Where(x => x != null))
            {
                var row = dt.NewRow();
                row["Id"] = tag.Id;
                row["Type"] = tag.Type;
                row["Description"] = tag.Description;
                dt.Rows.Add(row);
            }
            return dt;
        }
    }
}