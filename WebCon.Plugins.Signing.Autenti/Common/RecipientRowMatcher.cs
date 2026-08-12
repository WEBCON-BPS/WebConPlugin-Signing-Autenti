using System.Linq;
using WebCon.WorkFlow.SDK.Documents.Model.ItemLists;

namespace WebCon.Plugins.Signing.Autenti.Common;

public class RecipientRowMatcher(int emailColumnId, int roleColumnId, int? autentiIdColumnId)
{
    public ItemRowData FindMatch(ItemsList list, RecipientStatus recipient)
        => list.Rows.FirstOrDefault(row => IsMatch(row, recipient));

    public bool IsMatch(ItemRowData row, RecipientStatus recipient)
    {
        var isIdMatch = autentiIdColumnId.HasValue && row.GetCellValue(autentiIdColumnId.Value)?.ToString() == recipient.AutentiId;

        return (row.GetCellValue(emailColumnId)?.ToString() == recipient.Email || isIdMatch) &&
               row.GetCellValue(roleColumnId)?.ToString()?.Split('#')?.FirstOrDefault() == recipient.Role;
    }
}
