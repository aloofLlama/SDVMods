using System;
using System.Collections.Generic;

namespace PlantingDay.Helpers
{
    public static class SeedOverrides
    {
        //        // Key: (seedId, vendorId)
        //        public static readonly Dictionary<(string SeedId, string VendorId), int> Price
        //            = new()
        //            {
        //            { ("431", "SeedShop"), 200 }, // Sunflower Seeds

        //            //Grains over Hull CP seeds
        //            { ("slimerrain.grainsoverhullcp_Barley_Seeds", "SeedShop"), 10 }, // Barley_Seeds
        //            { ("slimerrain.grainsoverhullcp_Buckwheat_Seeds", "SeedShop"), 200 }, // Buckwheat_Seeds
        //            { ("slimerrain.grainsoverhullcp_Millet_Seeds", "SeedShop"), 10 }, // Millet_Seeds
        //            { ("slimerrain.grainsoverhullcp_Oat_Seeds", "SeedShop"), 16 }, // Oat_Seeds
        //            { ("slimerrain.grainsoverhullcp_Rye_Seeds", "SeedShop"), 20 }, // Rye_Seeds
        //            { ("slimerrain.grainsoverhullcp_Sorghum_Seeds", "SeedShop"), 10 }, // Sorghum_Seeds

        //           //Uncle Iroh Approved Tea seeds
        //            { ("slimerrain.uncleirohapprovedteacp_Chamomile_Seeds", "SeedShop"), 100 }, // Chamomile_Seeds
        //            { ("slimerrain.uncleirohapprovedteacp_Chrysanthemum_Seeds", "SeedShop"), 130 }, // Chrysanthemum_Seeds
        //            { ("slimerrain.uncleirohapprovedteacp_Goji_Berry_Seeds", "SeedShop"), 160 }, // Berry_Seeds
        //            { ("slimerrain.uncleirohapprovedteacp_Lavender_Seeds", "SeedShop"), 240 }, // Lavender_Seeds
        //            { ("slimerrain.uncleirohapprovedteacp_Lily_Bulb", "SeedShop"), 240 }, // Lily_Bulb
        //            { ("slimerrain.uncleirohapprovedteacp_Lotus_Seeds", "SeedShop"), 180 }, // Lotus_Seeds
        //            { ("slimerrain.uncleirohapprovedteacp_Sugarcane_Seeds", "SeedShop"), 350 }, // Sugarcane_Seeds

        //           //Cornucopia
        //            { ("Cornucopia_Soybeans", "Traveler"), 195 }, // Soybeans


        //            };

        //        // Key: (seedId, vendorId)
        //        // Value: new vendorId
        //        public static readonly Dictionary<(string SeedId, string VendorId), string> Vendor
        //            = new()
        //            {
        //                    // Example: Soybeans sold by Traveler instead of default
        //                    { ("Cornucopia_Soybeans", ""), "Traveler" },
        //             };


        // Key: seedId
        // Value: list of (monsterName, dropChance)
        public static readonly Dictionary<string, List<(string Monster, double Chance)>> MonsterDrops
            = new()
            {
                    // Red Cabbage Seeds dropped by Mummies & Serpents
                    { "485", new()
                        {
                            ("Mummy", 0.002),
                            ("Serpent", 0.002),
                            ("Purple Slime", 0.001)
                        }
                    },

                //dont know if this data is accurate yet


                //// Ancient Seeds dropped by Magma Sprite & Magma Duggy
                //{ "AncientSeeds", new()
                //    {
                //        ("Magma Sprite", 0.01),
                //        ("Magma Duggy", 0.01),
                //    }
                //},

                // Add more as needed
            };
    }
}










//    //Non Price and Non Trade - TBD what to do with these
//    //{ ("499", ""),  }, // Ancient Seeds - crafting
//    //{ ("885", ""),  }, // Fiber Seeds - crafting
//    //{ ("890", ""),  }, // Qi Bean - lots
//    //{ ("Cornucopia_SesameSeeds", ""),  }, // SesameSeeds - Artifact spots in desert
//    //{ ("Cornucopia_LentilsSeeds", ""),  }, // LentilsSeeds - Artifact spots in desert
//    //{ ("Cornucopia_MungBeans", ""),  }, // MungBeans - Fishing in Pelican town, Cindersap Forest, the Mountain
//    //{ ("Cornucopia_UbeSeeds", ""),  }, // UbeSeeds - Artifact Spots in Winter



