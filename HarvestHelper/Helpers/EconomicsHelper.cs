using SDVData;
using StardewValley.Locations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HarvestHelper.Helpers
{
    //TODO: PART OF DETERMINING IF SEED MAKER IS ECONOMICAL
    public static class EconomicsHelper
    {
        public static int? GetMinSeedPriceFromMainVendors(PlantInfoData plant)
        {
            if (plant == null || plant.PurchaseOptions == null)
                return null;

            // Allowed mainstream vendors
            var allowed = new HashSet<VendorType>
            {
                VendorType.Pierre,
                VendorType.Oasis,
                VendorType.Marnie,
                //VendorType.Ari, //Sunberry
                //VendorType.Jumana //Sunberry
            };


            // Filter to allowed vendors with a gold price
            var prices = plant.PurchaseOptions
                .Where(p => allowed.Contains(p.Type) && p.GoldPrice.HasValue)
                .Select(p => p.GoldPrice!.Value);

            // Return min or null if none
            return prices.Any() ? prices.Min() : (int?)null;
        }

    }
}
