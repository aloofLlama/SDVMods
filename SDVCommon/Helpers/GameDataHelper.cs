using SDVCommon.Models.Wrappers;
using SDVCommon.Services;
using StardewValley;

namespace SDVCommon.Helpers
{
    internal class GameDataHelper
    {
        //---------------
        //Have seed Id, get access to harvest SDV object from game data
        //---------------
        public static StardewValley.Object? GetHarvestObjectFromSeedId(string seedId)
        {
            // Use your canonical accessor
            PlantInfo? plant = PlantInfoBuilder.LookupFromKey(seedId);
            if (plant == null)
                return null;

            // Get the harvest ID from your PlantInfoData
            string harvestId = plant.Data.HarvestId;
            if (string.IsNullOrEmpty(harvestId))
                return null;

            // Create the actual harvest item instance (UIS-style)
            Item item = ItemRegistry.Create(harvestId);
            return item as StardewValley.Object;
        }
    }
}
