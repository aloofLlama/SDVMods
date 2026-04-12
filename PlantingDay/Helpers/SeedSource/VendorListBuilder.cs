using PlantingDay.Models.Runtime;
using PlantingDay.Models.Wrappers;

namespace PlantingDay.Helpers.SeedSource
{
    public static class VendorListBuilder
    {

        private static readonly HashSet<string> IgnoredVendors = new()
        {
           "JojaMart",
           "Joja"
        };

        public static List<PurchaseInfoRuntime> Build(PlantInfo plant)
        {
            // 1. Filter ignored vendors
            var filtered = plant.PurchaseOptions
                .Where(p => !IgnoredVendors.Any(ignore =>
                    p.Data.VendorId.StartsWith(ignore, StringComparison.OrdinalIgnoreCase)))
                .ToList();

            // 2. Collapse duplicates using VendorKey
            var collapsed = filtered
                .GroupBy(v => VendorHelper.VendorKey(v))
                .Select(g => g.First())
                .ToList();

            // 3. Sort vendors using SortKey + VendorName
            var sorted = collapsed
                .OrderBy(v => VendorHelper.SortKey(v))
                .ThenBy(v => v.Data.VendorName)
                .ToList();

            //ModEntry.Instance.Monitor.Log("=== SORTED VENDORS ===", LogLevel.Warn);
            //foreach (var v in sortedVendors)
            //{
            //    ModEntry.Instance.Monitor.Log(
            //        $"Vendor: {v.VendorName}, Price={v.GoldPrice}, Trade={v.TradeAmount}, IsNightMarket={VendorHelper.IsNightMarket(v)}",
            //        LogLevel.Warn
            //        );
            //}

            return sorted;
        }
    }


    }
