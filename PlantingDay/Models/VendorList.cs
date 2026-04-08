using PlantingDay.Helpers;
using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static StardewValley.Menus.CharacterCustomization;

namespace PlantingDay.Models
{
    public static class VendorListBuilder
    {

        private static readonly HashSet<string> IgnoredVendors = new()
        {
           "JojaMart",
           "Joja"
        };

        //public static List<PurchaseInfo> Build(PlantInfo plant)
        //{
        //    // Filter and collapse duplicates
        //    return plant.PurchaseOptions
        //        .Where(p => !IgnoredVendors.Any(ignore =>
        //            p.VendorId.StartsWith(ignore, StringComparison.OrdinalIgnoreCase)))
        //        .GroupBy(v => VendorHelper.VendorKey(v))
        //        .Select(g => g.First())
        //        .ToList();
        //}


        //public static class VendorListBuilder
        //{
        //private static readonly HashSet<string> IgnoredVendors = new()
        //    {
        //        "JojaMart",
        //    };
        public static List<PurchaseInfo> Build(PlantInfo plant)
        {
            // Filter and collapse duplicates
            var vendors = plant.PurchaseOptions
                        .Where(p => !IgnoredVendors.Any(ignore =>
                            p.VendorId.StartsWith(ignore, StringComparison.OrdinalIgnoreCase)))

                .GroupBy(v => VendorHelper.VendorKey(v))
                .Select(g => g.First())
                .ToList();

            //ModEntry.Instance.Monitor.Log("=== VENDOR DETAILS ===", LogLevel.Warn);
            //foreach (var v in vendors)
            //{
            //    ModEntry.Instance.Monitor.Log(
            //        $"Vendor: {v.VendorName}, Price={v.GoldPrice}, Trade={v.TradeAmount}, IsNightMarket={VendorHelper.IsNightMarket(v)}",
            //        LogLevel.Warn
            //    );
            //}

            // sort vendors
            var sortedVendors = vendors
                .OrderBy(v => VendorHelper.SortKey(v))
                .ThenBy(v => v.VendorName)
                .ToList();

            //ModEntry.Instance.Monitor.Log("=== SORTED VENDORS ===", LogLevel.Warn);
            //foreach (var v in sortedVendors)
            //{
            //    ModEntry.Instance.Monitor.Log(
            //        $"Vendor: {v.VendorName}, Price={v.GoldPrice}, Trade={v.TradeAmount}, IsNightMarket={VendorHelper.IsNightMarket(v)}",
            //        LogLevel.Warn
            //        );
            //}

            return sortedVendors;
        }
    }

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

    }
