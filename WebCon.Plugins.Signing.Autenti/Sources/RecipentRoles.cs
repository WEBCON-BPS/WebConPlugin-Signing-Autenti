using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using WebCon.Plugins.Signing.Autenti.Api.Models;
using WebCon.WorkFlow.SDK.Common.Model;
using WebCon.WorkFlow.SDK.DataSourcePlugins;
using WebCon.WorkFlow.SDK.DataSourcePlugins.Model;

namespace WebCon.Plugins.Signing.Autenti.Sources;

public class RecipentRoles : CustomDataSource<RecipentRolesConfig>
{
    private static readonly (string Id, string Value, string Translation, Func<RoleEnabledConfig, bool> IsEnabled)[] Roles =
    [
        (AutentiRoles.Signer, "Signer", "Podpisujący", r => r.IsSignerEnabled),
        (AutentiRoles.Approver, "Approver", "Parafujący", r => r.IsApproverEnabled),
        (AutentiRoles.Reviewer, "Reviewer", "Opiniujący", r => r.IsReviewerEnabled),
        (AutentiRoles.Viewer, "Viewer", "Do wglądu", r => r.IsViewerEnabled),
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

        foreach (var (id, value, translation, isEnabled) in Roles)
            if (isEnabled(Configuration.AvailableRoles))
                table.Rows.Add(id, value, translation);

        return Task.FromResult(table);
    }
}