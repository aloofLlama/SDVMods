

namespace SDVData
{
    public class PurchaseInfoData
    {
        public string VendorId { get; set; } = "";
        public string VendorName { get; set; } = "";
        public VendorType Type { get; set; } = VendorType.Other;
        public enum VendorType
        {
            Pierre,
            Joja,
            NightMarket,
            TravelingCart,
            DesertFestival,
            ValleyFair,
            Other
        }


        public int? GoldPrice { get; set; }
        public string? TradeItemId { get; set; }
        public int TradeAmount { get; set; }

        public string? Condition { get; set; }


    }

}
