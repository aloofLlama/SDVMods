using PlantingDay.Helpers.Icons;
using PlantingDay.Models;

namespace PlantingDay.RuntimeModels;

public class PurchaseInfoRuntime
{
    public PurchaseInfoData Data { get; }
    public Icon? CurrencyIcon { get; set; }

    public PurchaseInfoRuntime(PurchaseInfoData data)
    {
        Data = data;
    }
}
