
using SDVCommon.Services;
using StardewValley;
using System.Xml;
using System.Xml.Linq;

namespace SDVCommon.Helpers
{
    /*
    https://stardewvalleywiki.com/Modding:Items
    Every item is identified in the game data using a unique item ID. This has two forms:
    
    The unqualified item ID(item.ItemId) is a string ID for the item, like 128 (vanilla item)
    or Example.ModId_Watermelon(custom item). For legacy reasons, the unqualified ID for vanilla items
    may not be globally unique; for example, Pufferfish(object 128) and Mushroom Box(bigcraftable 128)
    both have item ID 128.
    
    The qualified item ID(item.QualifiedItemId) is a globally unique identifier which combines the
    item's type ID and unqualified item ID, like (O)128 for object ID 128.
    */

    /* Where possible, always use qualified ID. Call it qId (or HarvestQId, SeedQId, etc).
     * If unqualified must be used (e.g. interacting with gamedata) use unqualifiedId.
     * 
     * TODO the entire codebase is a mess of naming and which ID is used. Switching to qId for the entire
     * refactor to fix the mess. Pretty sure id and ItemId are actually the same thing.
     */


    public static class IdHelper
    {
        // Removes the (O) or (BC) prefix from Ids
        // Accepts both qualified and unqualified IDs, as ItemRegistry can resolve either
        public static string ToUnqualifiedItemId(string? id)
        {
            if (string.IsNullOrEmpty(id))
                return string.Empty;

            // (O)StringId → StringId
            if (id.StartsWith("(O)"))
                return id.Substring(3);

            // (BC)StringId → StringId
            if (id.StartsWith("(BC)"))
                return id.Substring(4);

            return id;
        }

        // Adds the correct prefix to Ids (usually (O))
        // Accepts both qualified and unqualified IDs
        public static string ToQualifiedId(string id)
        {
            if (string.IsNullOrEmpty(id))
                return id;

            // Already qualified
            if (id.StartsWith("("))
                return id;

            // Resolve via ItemRegistry
            var data = ItemRegistry.GetData(id);
            //SDVCommonLog.TempLog($"ToQualifiedId: {id} → {data?.QualifiedItemId}", LogHelper.DebugInfo);
            return data?.QualifiedItemId ?? id;
        }

        // Returns the mod prefix for an item (e.g. "skellady.SBVCP" for "skellady.SBVCP_SunberrySeeds")
        public static string GetModPrefix(string qId)
        {
            if (IdHelper.IsVanillaStardew(qId))
                return "StardewValley";

            // TODO check that nature in the valley works with qId
            // Special case: Nature in the Valley uses dot notation
            if (qId.StartsWith("NatInValley.", StringComparison.OrdinalIgnoreCase))
                return "NatInValley";

            // Extract mod prefix
            if (qId.Contains('_'))
                return qId.Split('_')[0];

            // Fallback: treat entire string as prefix
            return qId;
        }

        // Removes the mod prefix for an item (e.g. returns AsterShop for Lumisteria.MtVapius_AsterShop)
        // Works with any type of id that has the mod prefix qualified, unqualified, shop name, etc.
        public static string RemoveModPrefix(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return id;

            // General nomenclature is modder.ModName_Name (e.g. skellady.SBVCP_SunberrySeeds or skellady.SBVCP_AriMarket)
            if (id.Contains('_'))
                return id[(id.IndexOf('_') + 1)..];

            // No prefix found
            return id;
        }



        //Checks if the ID is one that belongs to the basegame (aka not a mod)
        private static bool IsVanillaStardew(string qId)
        {
            if (string.IsNullOrWhiteSpace(qId))
                return false;

            string unqualifiedId = ToUnqualifiedItemId(qId);

            // Numeric IDs → always vanilla
            if (int.TryParse(unqualifiedId, out _))
                return true;

            // Check against list of non-numberic vanilla IDs (e.g. "FarAwayStone", "DeluxeBait", etc.)
            if (VanillaStringIds.Contains(unqualifiedId))
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
