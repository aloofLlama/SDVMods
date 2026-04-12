

namespace SDVData
{
    public class PurchaseInfoData
    {
        public string VendorId { get; set; } = "";
        public string VendorName { get; set; } = "";

        public int? GoldPrice { get; set; }
        public string? TradeItemId { get; set; }
        public IconRef? CurrencyIconRef { get; set; }
        public int TradeAmount { get; set; }

        public string? Condition { get; set; }
    }

}
