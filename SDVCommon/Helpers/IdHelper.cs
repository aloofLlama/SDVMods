
namespace SDVCommon.Helpers
{
    public static class IdHelper
    {
        //Removes the (O) prefix from seed/harvest Ids
        public static string CanonicalItemId(string? raw)
        {
            if (string.IsNullOrEmpty(raw))
                return string.Empty;

            // Vanilla object: (O)### → ###
            if (raw.StartsWith("(O)") && int.TryParse(raw.AsSpan(3), out int num))
                return num.ToString();

            // Modded object: (O)StringId → StringId
            if (raw.StartsWith("(O)"))
                return raw.Substring(3);

            // Already canonical (modded seeds, JA, DGA, etc.)
            return raw;
        }
        public static string ToGameId(string id)
        {
            // Modded IDs contain no digits → return raw
            if (!id.Any(char.IsDigit))
                return id;

            // Vanilla IDs → return numeric form
                return new string(id.Where(char.IsDigit).ToArray());
            
        }

        public static string ToQualifiedId(string id)
        {
            if (string.IsNullOrEmpty(id))
                return id;

            if (id.StartsWith("(O)"))
                return id;

            return "(O)" + id;
        }

        //Checks if the ID is one that belongs to the basegame (aka not a mod)
        public static bool IsVanillaStardew(string canonicalId)
        {
            if (string.IsNullOrWhiteSpace(canonicalId))
                return false;

            // Normalize to canonical form
            string id = CanonicalItemId(canonicalId);

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
