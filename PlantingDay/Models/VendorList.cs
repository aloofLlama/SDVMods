using PlantingDay.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlantingDay.Models
{
    public static class VendorListBuilder
    {
        private static readonly HashSet<string> IgnoredVendors = new()
        {
           "JojaMart",
        };

        public static List<PurchaseInfo> Build(PlantInfo plant)
        {
            // Filter and collapse duplicates
            return plant.PurchaseOptions
                .Where(p => !IgnoredVendors.Contains(p.VendorId))
                .GroupBy(v => VendorKey(v))
                .Select(g => g.First())
                .ToList();
        }

        private static string VendorKey(PurchaseInfo v)
        {
            if (VendorHelper.IsPierre(v))
                return "SeedShop";

            if (VendorHelper.IsNightMarket(v))
                return "NightMarket";

            if (VendorHelper.IsDesertFestival(v))
                return "DesertFestival";

            return v.VendorId;
        }
    }
    //public static class VendorListBuilder
    //{
    //    private static readonly HashSet<string> IgnoredVendors = new()
    //        {
    //            "JojaMart",
    //        };
    //    public static List<PurchaseInfo> Build(PlantInfo plant)
    //    {
    //        // Filter and collapse duplicates
    //        var vendors = plant.PurchaseOptions
    //            .Where(p => !IgnoredVendors.Contains(p.VendorId))
    //            .GroupBy(v => VendorKey(v))
    //            .Select(g => g.First())
    //            .ToList();

    //        // sort vendors
    //        var sortedVendors = vendors
    //            .OrderBy(v => SortKey(v))
    //            .ThenBy(v => v.VendorName)
    //            .ToList();

    //        return sortedVendors;
    //    }

    //    //-------------
    //    // Grouping - Collapse multiple entries to one
    //    //-------------

    //    private static string VendorKey(PurchaseInfo v)
    //    {
    //        if (Helpers.VendorHelper.IsPierre(v))
    //            return "SeedShop";      

    //        if (Helpers.VendorHelper.IsNightMarket(v))
    //            return "NightMarket";   

    //        if (Helpers.VendorHelper.IsDesertFestival(v))
    //            return "DesertFestival";   

    //        return v.VendorId;          
    //    }

    //    //-------------
    //    // Sorting
    //    //-------------
    //    private static int SortKey(PurchaseInfo v)
    //    {
    //        // 0 — Pierre always first
    //        if (Helpers.VendorHelper.IsPierre(v))
    //            return 0;

    //        // 4 — Night Market always last
    //        if (Helpers.VendorHelper.IsNightMarket(v))
    //            return 4;

    //        // 1 — Gold vendors (Joja, Traveling Cart, etc.)
    //        if (v.GoldPrice.HasValue)
    //            return 1;

    //        // 3 — Trade vendors (Desert Trader, Island Trader, Qi trade shops)
    //        if (v.TradeAmount > 0)
    //            return 3;

    //        // 2 — Everything else (icon-only vendors, special cases)
    //        return 2;
    //    }

    //}

}
