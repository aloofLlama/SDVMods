using SDVCommon.Icons;
using SDVData;

namespace PlantingDay.Models.Runtime;

public class PurchaseInfoRuntime
{
    public PurchaseInfoData Data { get; }
    public Icon? CurrencyIcon { get; set; }

    public PurchaseInfoRuntime(PurchaseInfoData data)
    {
        Data = data;
    }
}
