using SDVData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDVData_Tests.Helpers
{
    internal class TestHelpers
    {
        // Filter to vanilla seeds and fruit tree saplings
        public static bool IsVanillaSeedId(string seedId) => int.TryParse(seedId, out _);

        public static bool IsNightMarket(string vendorId) =>
            vendorId.Contains("NightMarket", StringComparison.OrdinalIgnoreCase);

        public static string TrimSeedName(string raw)
        {
            var parts = raw.Split('_');
            if (parts.Length < 2)
                return raw;

            //return $"{parts[^2]}_{parts[^1]}";
            return $"{parts[^1]}";
        }

        public static int GetMinYearForVendor(PurchaseInfoData vendor)
        {
            int minYear = int.MaxValue;

            var cond = vendor.Condition;
            if (string.IsNullOrWhiteSpace(cond))
                return 1;

            var tokens = cond.Split(',', ' ', '\t');

            for (int i = 0; i < tokens.Length - 1; i++)
            {
                if (tokens[i] == "YEAR" && int.TryParse(tokens[i + 1], out int y))
                    minYear = Math.Min(minYear, y);
            }

            return minYear == int.MaxValue ? 1 : minYear;
        }

        public static bool IsRegularShopVendor(PurchaseInfoData v)
        {
            var id = v.VendorId;

            // regular shops, including night market
            if (id == "SeedShop") return true;
            if (id == "Joja") return true;
            if (id == "Sandy") return true;
            if (id == "AnimalShop") return true;
            if (id == "skellady.SBVCP_AriMarket") return true;
            if (id == "skellady.SBVCP_JumanaShop") return true;
            if (TestHelpers.IsNightMarket(id)) return true;

            // everything else is ignored for this test (festivals, traveling cart, trade vendors, etc)
            return false;
        }



    }
}
