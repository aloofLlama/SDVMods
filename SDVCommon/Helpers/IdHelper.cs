
using SDVCommon.GameData;
using StardewValley;
using StardewValley.GameData.Crops;
using StardewValley.GameData.Objects;

namespace SDVCommon.Helpers
{
    public static class IdHelper
    {
        //Seed and Harvest Ids are stored raw from game data:
        //e.g. 890, CarrotSeeds, Cornucopia_BasilSeeds, 889, Carrot, Cornucopia_Basil, (O)638 [for fruit tree fruit]
        //Shop data lists seeds as (O)890, Carrot Seeds, Cornucopia_BasilSeeds
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



        //---------------
        //Have harvest Id, get access to seed data in plantinfo
        //---------------
        public static CropData? GetSeedDataForHarvest(string harvestId)
        {
            if (GameObjectInfoHelper._harvestToSeed.TryGetValue(harvestId, out string? seedId))
            {
                if (Game1.cropData.TryGetValue(seedId, out var seedData))
                    return seedData;
            }

            return null;
        }

    }


}
