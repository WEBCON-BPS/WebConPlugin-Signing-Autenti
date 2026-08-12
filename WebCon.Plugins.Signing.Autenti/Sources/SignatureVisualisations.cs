using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using WebCon.Plugins.Signing.Autenti.SendForSignature;
using WebCon.WorkFlow.SDK.Common;
using WebCon.WorkFlow.SDK.Common.Model;
using WebCon.WorkFlow.SDK.DataSourcePlugins;
using WebCon.WorkFlow.SDK.DataSourcePlugins.Model;

namespace WebCon.Plugins.Signing.Autenti.Sources
{
    public class SignatureVisualisations : CustomDataSource<PluginConfiguration>
    {

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


            table.Rows.Add((int)SignatureRepresentation.Signature_on_additional_page_at_end_of_document,
                "Signature on additional page at end of document", "Podpis na dodatkowej stronie na końcu dokumentu", "Unterschrift auf einer zusätzlichen Seite am Ende des Dokuments");
            table.Rows.Add((int)SignatureRepresentation.Signature_in_any_place_chosen_by_the_recipient,
                "Signature in any place chosen by the recipient", "Podpis w dowolnym miejscu wybranym przez odbiorcę", "Unterschrift an einer beliebigen vom Empfänger gewählten Stelle");

            return Task.FromResult(table);
        }
    }
}