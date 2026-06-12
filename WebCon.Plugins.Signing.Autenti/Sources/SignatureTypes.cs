using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using WebCon.Plugins.Signing.Autenti.Api.Models;
using WebCon.WorkFlow.SDK.Common.Model;
using WebCon.WorkFlow.SDK.DataSourcePlugins;
using WebCon.WorkFlow.SDK.DataSourcePlugins.Model;

namespace WebCon.Plugins.Signing.Autenti.Sources;

public class SignatureTypes : CustomDataSource<SignatureTypesConfig>
{
    private static readonly (string Id, string Value, string TranslationPL, string TranslationDE, Func<SignaturesEnabledConfig, bool> IsEnabled)[] Types =
    [
        (AutentiSignatureTypes.Basic, "Autenti E-signature", "E-podpis Autenti", "Autenti E-Signatur", s => s.IsESignatureEnabled),
        (AutentiSignatureTypes.Qualified, "Qualified signature", "Podpis kwalifikowany", "Qualifizierte Signatur", s => s.IsQualifiedSignatureEnabled),
        (AutentiSignatureTypes.Advanced, "Advanced Autenti e-signature", "Zaawansowany e-podpis Autenti", "Fortgeschrittene Autenti E-Signatur", s => s.IsAdvancedESignatureEnabled),
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

        foreach (var (id, value, translationPL, translataionDE, isEnabled) in Types)
            if (isEnabled(Configuration.AvailableSignature))
                table.Rows.Add(id, value, translationPL, translataionDE);

        return Task.FromResult(table);
    }
}