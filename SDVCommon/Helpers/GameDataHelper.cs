using PlantingDay.Models.Wrappers;
using PlantingDay.Services;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlantingDay.Helpers
{
    internal class GameDataHelper
    {
        //---------------
        //Have seed Id, get access to harvest SDV object from game data
        //---------------
        public static StardewValley.Object? GetHarvestObjectFromSeedId(string seedId)
        {
            // Use your canonical accessor
            OBSPlantInfo? plant = OBSPlantInfoBuilder.LookupFromKey(seedId);
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
