using System.Collections.Generic;
using System.Linq;
using PlantingDay.Models.Wrappers;

namespace PlantingDay.Helpers.SeedSource
{
    public static class SeedSourceAggregator
    {
        /// <summary>
        /// Returns a unified ordered list of vendor + monster drop sources.
        /// Order:
        ///   1. Non–Night Market vendors
        ///   2. Monster drops
        ///   3. Night Market vendors (always last)
        /// </summary>
        public static List<object> BuildFullSourceList(PlantInfo plant)
        {

            var list = new List<object>();

            // 1. Sorted vendor list (Pierre → Gold → Trade → Night Market)
            var vendors = VendorListBuilder.Build(plant);

            // 2. Split Night Market
            var nightMarket = vendors.Where(v => VendorHelper.IsNightMarket(v.Data.VendorId)).ToList();
            var nonNightVendors = vendors.Where(v => !VendorHelper.IsNightMarket(v.Data.VendorId)).ToList();

            // 3. Add in correct order
            list.AddRange(nonNightVendors);
            list.AddRange(plant.MonsterDrops);
            list.AddRange(nightMarket);

            return list;
        }
    }
}
