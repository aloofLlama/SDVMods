using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlantingDay.Helpers
{
    public record SeedOverride(string? VendorId, int? Price);

    public static class SeedDataOverrides
    {
        // Key: (seedId, vendorId)
        public static readonly Dictionary<(string? SeedId, string? VendorId), SeedOverride> Overrides
            = new()
            {
            { ("431", "SeedShop"), new SeedOverride(null, 200) }, // Sunflower Seeds

            //Grains over Hull CP seeds
            { ("slimerrain.grainsoverhullcp_Barley_Seeds", "SeedShop"), new SeedOverride(null, 10) }, // Barley_Seeds
            { ("slimerrain.grainsoverhullcp_Buckwheat_Seeds", "SeedShop"), new SeedOverride(null, 200) }, // Buckwheat_Seeds
            { ("slimerrain.grainsoverhullcp_Millet_Seeds", "SeedShop"), new SeedOverride(null, 10) }, // Millet_Seeds
            { ("slimerrain.grainsoverhullcp_Oat_Seeds", "SeedShop"), new SeedOverride(null, 16) }, // Oat_Seeds
            { ("slimerrain.grainsoverhullcp_Rye_Seeds", "SeedShop"), new SeedOverride(null, 20) }, // Rye_Seeds
            { ("slimerrain.grainsoverhullcp_Sorghum_Seeds", "SeedShop"), new SeedOverride(null, 10) }, // Sorghum_Seeds
                                                                              
            //Uncle Iroh Approved Tea seeds
            { ("slimerrain.uncleirohapprovedteacp_Chamomile_Seeds", "SeedShop"), new SeedOverride(null, 100) }, // Chamomile_Seeds
            { ("slimerrain.uncleirohapprovedteacp_Chrysanthemum_Seeds", "SeedShop"), new SeedOverride(null, 130) }, // Chrysanthemum_Seeds
            { ("slimerrain.uncleirohapprovedteacp_Goji_Berry_Seeds", "SeedShop"), new SeedOverride(null, 160) }, // Berry_Seeds
            { ("slimerrain.uncleirohapprovedteacp_Lavender_Seeds", "SeedShop"), new SeedOverride(null, 240) }, // Lavender_Seeds
            { ("slimerrain.uncleirohapprovedteacp_Lily_Bulb", "SeedShop"), new SeedOverride(null, 240) }, // Lily_Bulb
            { ("slimerrain.uncleirohapprovedteacp_Lotus_Seeds", "SeedShop"), new SeedOverride(null, 180) }, // Lotus_Seeds
            { ("slimerrain.uncleirohapprovedteacp_Sugarcane_Seeds", "SeedShop"), new SeedOverride(null, 350) }, // Sugarcane_Seeds

            //Cornucopia
            { ("Cornucopia_Soybeans", null), new SeedOverride("Traveler", 195) }, // Soybeans


            //Non Price and Non Trade - TBD what to do with these
            //{ ("499", ""),  }, // Ancient Seeds - crafting
            //{ ("885", ""),  }, // Fiber Seeds - crafting
            //{ ("890", ""),  }, // Qi Bean - lots
            //{ ("Cornucopia_SesameSeeds", ""),  }, // SesameSeeds - Artifact spots in desert
            //{ ("Cornucopia_LentilsSeeds", ""),  }, // LentilsSeeds - Artifact spots in desert
            //{ ("Cornucopia_MungBeans", ""),  }, // MungBeans - Fishing in Pelican town, Cindersap Forest, the Mountain
            //{ ("Cornucopia_UbeSeeds", ""),  }, // UbeSeeds - Artifact Spots in Winter

            };
    }
}
