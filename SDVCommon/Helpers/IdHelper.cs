
using StardewValley;
using System.Xml;

namespace SDVCommon.Helpers
{
    /*
    https://stardewvalleywiki.com/Modding:Items
    Every item is identified in the game data using a unique item ID. This has two forms:
    
    The unqualified item ID(item.ItemId) is a unique string ID for the item, like 128 (vanilla item)
    or Example.ModId_Watermelon(custom item). For legacy reasons, the unqualified ID for vanilla items
    may not be globally unique; for example, Pufferfish(object 128) and Mushroom Box(bigcraftable 128)
    both have item ID 128.
    
    The qualified item ID(item.QualifiedItemId) is a globally unique identifier which combines the
    item's type ID and unqualified item ID, like (O)128 for object ID 128.
    */

    //TODO Clean up IDs
    // Was Canonical -> change to ItemId
    // Was toGameId -> change to ItemId
    // Was toQualifiedId -> keep as is 

    /* Where possible, always use unqualified ID (do not have (O) prefix)
     * Call them: ItemId in general, or HarvestId, SeedId, etc according to context
     * 
     * If qualified ID is needed, always call it qualifiedId (or qualifiedItemId, qualifiedSeedId, etc)
     */


    public static class IdHelper
    {
        //Removes the (O) prefix from Ids
        public static string ToItemId(string? raw)
        {
            if (string.IsNullOrEmpty(raw))
                return string.Empty;

            // Vanilla object: (O)### → ###
            if (raw.StartsWith("(O)") && int.TryParse(raw.AsSpan(3), out int num))
                return num.ToString();

            // Modded object: (O)StringId → StringId
            if (raw.StartsWith("(O)"))
                return raw.Substring(3);

            return raw;
        }

        //Adds the (O) prefix to Ids
        public static string ToQualifiedId(string id)
        {
            if (string.IsNullOrEmpty(id))
                return id;

            if (id.StartsWith("(O)"))
                return id;

            return "(O)" + id;
        }

        public static string GetModPrefix(string itemId)
        {
            if (IdHelper.IsVanillaStardew(itemId))
                return "StardewValley";

            // Special case: Nature in the Valley uses dot notation
            if (itemId.StartsWith("NatInValley.", StringComparison.OrdinalIgnoreCase))
                return "NatInValley";

            // Extract mod prefix
            if (itemId.Contains('_'))
                return itemId.Split('_')[0];

            // Fallback: treat entire string as prefix
            return itemId;
        }

        public static string RemoveModPrefix(string itemId)
        {
            if (string.IsNullOrWhiteSpace(itemId))
                return itemId;

            // General nomenclature is modder.ModName_Name (e.g. skellady.SBVCP_SunberrySeeds or skellady.SBVCP_AriMarket)
            if (itemId.Contains('_'))
                return itemId[(itemId.IndexOf('_') + 1)..];

            // No prefix found
            return itemId;
        }



        //Checks if the ID is one that belongs to the basegame (aka not a mod)
        public static bool IsVanillaStardew(string itemId)
        {
            if (string.IsNullOrWhiteSpace(itemId))
                return false;

            string id = ToItemId(itemId);

            // Numeric IDs → always vanilla
            if (int.TryParse(id, out _))
                return true;

            // Verified vanilla string IDs
            if (VanillaStringIds.Contains(id))
                return true;


            return false;
        }

        private static readonly HashSet<string> VanillaStringIds = new(StringComparer.OrdinalIgnoreCase)
            {
            "FarAwayStone",
            "DeluxeBait",
            "Moss",
            "MossySeed",
            "SonarBobber",
            "SpecificBait",
            "TentKit",
            "MysticTreeSeed",
            "MysticSyrup",
            "Raisins",
            "DriedFruit",
            "DriedMushrooms",
            "StardropTea",
            "PrizeTicket",
            "GoldCoin",
            "TreasureTotem",
            "ChallengeBait",
            "Carrot",
            "SummerSquash",
            "Broccoli",
            "Powdermelon",
            "SmokedFish",
            "PurpleBook",
            "SkillBook_0",
            "SkillBook_1",
            "SkillBook_2",
            "SkillBook_3",
            "SkillBook_4",
            "SeaJelly",
            "CaveJelly",
            "RiverJelly",
            "Goby",
            "BlueGrassStarter",
            "MossSoup",

            //Books
            "Book_Trash",
            "Book_Crabbing",
            "Book_Bombs",
            "Book_Roe",
            "Book_WildSeeds",
            "Book_Woodcutting",
            "Book_Defense",
            "Book_Friendship",
            "Book_Void",
            "Book_Speed",
            "Book_Marlon",
            "Book_PriceCatalogue",
            "Book_QueenOfSauce",
            "Book_Diamonds",
            "Book_Mystery",
            "Book_AnimalCatalogue",
            "Book_Speed2",
            "Book_Artifact",
            "Book_Horse",
            "Book_Grass"

        };


    }
}
