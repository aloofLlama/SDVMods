using System;
using System.Collections.Generic;

namespace SDVCommon.Compatibility
{
    public static class DataOverrides
    {

        public static readonly Dictionary<string, string?> ModSource =
            new(StringComparer.OrdinalIgnoreCase)
            {
                    { "StardewValley", "Stardew Valley" }, //base game obviously does not have a mod name
                    { "EastScarp", "East Scarp" },
                    { "Lemurkat.EastScarp", "East Scarp" },
                    { "skellady.SBVCP", "Sunberry Village" },
                    { "Lumisteria.MtVapius", "Visit Mount Vapius" },


                    { "slimerrain.uncleirohapprovedteacp", "Uncle Iroh Approved Tea" },
                    { "slimerrain.grainsoverhullcp", "Grains Overhull" },
                    { "Cornucopia", "Cornucopia" },
                    { "MNF.MoreNewFish", "More New Fish" },
                    { "NatInValley", "Nature in the Valley" },
                    { "Wildflour.SASS", "Rose and the Alchemist"},

            };


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
// highlands monster drops from expanded



