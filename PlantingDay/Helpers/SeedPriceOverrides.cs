using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlantingDay.Helpers
{
    public static class SeedPriceOverrides
    {
        // Key: (seedId, vendorId)
        public static readonly Dictionary<(string SeedId, string VendorId), int> Overrides
            = new()
            {
            { ("431", "SeedShop"), 200 }, // Sunflower Seeds

            //Grains over Hull CP seeds
            { ("slimerrain.grainsoverhullcp_Barley_Seeds", "SeedShop"), 10 }, // Barley_Seeds
            { ("slimerrain.grainsoverhullcp_Buckwheat_Seeds", "SeedShop"), 200 }, // Buckwheat_Seeds
            // ("slimerrain.grainsoverhullcp_Millet_Seeds", "SeedShop"), 5 }, // Millet_Seeds
            { ("slimerrain.grainsoverhullcp_Oat_Seeds", "SeedShop"), 16 }, // Oat_Seeds
            { ("slimerrain.grainsoverhullcp_Rye_Seeds", "SeedShop"), 20 }, // Rye_Seeds
            { ("slimerrain.grainsoverhullcp_Sorghum_Seeds", "SeedShop"), 10 }, // Sorghum_Seeds
                                                                              
            //Uncle Iroh Approved Tea seeds
            //{ ("slimerrain.uncleirohapprovedteacp_Chamomile_Seeds", "SeedShop"), 50 }, // Chamomile_Seeds
            { ("slimerrain.uncleirohapprovedteacp_Chrysanthemum_Seeds", "SeedShop"), 130 }, // Chrysanthemum_Seeds
            { ("slimerrain.uncleirohapprovedteacp_Goji_Berry_Seeds", "SeedShop"), 160 }, // Berry_Seeds
            { ("slimerrain.uncleirohapprovedteacp_Lavender_Seeds", "SeedShop"), 240 }, // Lavender_Seeds
            //{ ("slimerrain.uncleirohapprovedteacp_Lily_Bulb", "SeedShop"), 120 }, // Lily_Bulb
            { ("slimerrain.uncleirohapprovedteacp_Lotus_Seeds", "SeedShop"), 180 }, // Lotus_Seeds
            { ("slimerrain.uncleirohapprovedteacp_Sugarcane_Seeds", "SeedShop"), 350 }, // Sugarcane_Seeds
            //{ ("slimerrain.uncleirohapprovedteacp_Cassia_Sapling", "Sandy"), 2400 }, // Cassia_Sapling

            //Sunberry Jumana's Shop
            //{ ("skellady.SBVCP_ArfajSeeds", "skellady.SBVCP_JumanaShop"), 50 }, // ArfajSeeds
            //{ ("skellady.SBVCP_CallaLilySeeds", "skellady.SBVCP_JumanaShop"), 20 }, // CallaLilySeeds
            //{ ("skellady.SBVCP_CoffeeBlossomSeeds", "skellady.SBVCP_JumanaShop"), 20 }, // CoffeeBlossomSeeds
            //{ ("skellady.SBVCP_CyclamenSeeds", "skellady.SBVCP_JumanaShop"), 30 }, // CyclamenSeeds
            //{ ("skellady.SBVCP_HyacinthSeeds", "skellady.SBVCP_JumanaShop"), 30 }, // HyacinthSeeds
            //{ ("skellady.SBVCP_RoyalAnemoneSeeds", "skellady.SBVCP_JumanaShop"), 40 }, // RoyalAnemoneSeeds
            { ("skellady.SBVCP_SpeedwellSeeds", "skellady.SBVCP_JumanaShop"), 60 }, // SpeedwellSeeds
            //{ ("skellady.SBVCP_WildMustardSeeds", "skellady.SBVCP_JumanaShop"), 40 }, // WildMustardSeeds


            };
    }
}
