using WebCon.WorkFlow.SDK.Documents.Model;
using WebCon.WorkFlow.SDK.Documents.Model.ItemLists;

namespace WebCon.Plugins.Signing.Autenti.SendForSignature.Providers.Enrichers;

public class ItemListRowContext(ItemRowData row, RecipentsListConfig columns) : IItemListRowContext
{
    private string GetValue(int? columnId)
    {
        if (!columnId.HasValue || columnId <= 0)
            return null;

        return row.GetCellValue(columnId.Value, EntityValueFormat.PairID)?.ToString();
    }

    private int? GetIntValue(int? columnId)
    {
        var value = GetValue(columnId);
        return int.TryParse(value, out var result) ? result : null;
    }

    private bool? GetBoolValue(int? columnId)
        => columnId.HasValue ? row.BooleanCells.GetByID(columnId.Value).Value : null;

    public string FirstName => GetValue(columns.FirstName);
    public string LastName => GetValue(columns.LastName);
    public string Email => GetValue(columns.Email);
    public string Role => GetValue(columns.Role);
    public string SignatureType => GetValue(columns.SignatureType);
    public string PhoneNumber => GetValue(columns.PhoneNumber);
    public bool? SmsAuthorization => GetBoolValue(columns.SmsAuthorization);
    public bool? SmsUnlock => GetBoolValue(columns.SmsUnlock);
    public int? SigningOrder => GetIntValue(columns.SigningOrder);
    public bool? Representative => GetBoolValue(columns.Representative);
    public string VAT => GetValue(columns.VAT);
    public string OrganisationName => GetValue(columns.OrganisationName);
    public string Position => GetValue(columns.Position);
}