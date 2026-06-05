

namespace SDVData
{
    public class PurchaseInfoData
    {
        public string VendorId { get; set; } = "";
        public string VendorName { get; set; } = "";
        public VendorType Type { get; set; } = VendorType.Other;

        public int? GoldPrice { get; set; }
        public string? TradeItemId { get; set; }
        public int TradeAmount { get; set; }

        public string? Condition { get; set; } //e.g. "YEAR 2"

    }

    public enum VendorType
    {
        Pierre,
        Joja,
        Oasis,
        Marnie,
        NightMarket,
        TravelingCart,
        DesertFestival,
        ValleyFair,
        //JojaEmporium, //From SDV Expanded
        //Ari, //From Sunberry
        //Jumana, //From Sunberry
        Other
    }

}
