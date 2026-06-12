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
    private static readonly (string Id, string Value, string TranslationPL, string TranslationDE, Func<RoleEnabledConfig, bool> IsEnabled)[] Roles =
    [
        (AutentiRoles.Signer, "Signer", "Podpisujący", "Signierender", r => r.IsSignerEnabled),
        (AutentiRoles.Approver, "Approver", "Parafujący", "Paraphierender", r => r.IsApproverEnabled),
        (AutentiRoles.Reviewer, "Reviewer", "Opiniujący", "Beurteilender", r => r.IsReviewerEnabled),
        (AutentiRoles.Viewer, "Viewer", "Do wglądu", "Zur Einsicht", r => r.IsViewerEnabled),
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

        foreach (var (id, value, translationPL, translationDE, isEnabled) in Roles)
            if (isEnabled(Configuration.AvailableRoles))
                table.Rows.Add(id, value, translationPL, translationDE);

        return Task.FromResult(table);
    }
}