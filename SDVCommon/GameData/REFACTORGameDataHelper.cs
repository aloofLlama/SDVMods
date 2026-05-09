using SDVCommon.GameData;
using SDVCommon.Models.Builders;
using SDVCommon.Models.Wrappers;
using StardewValley;
using StardewValley.GameData.Crops;

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

        public static readonly Dictionary<string, string> _harvestToSeed = new();

        public static void BuildHarvestToSeedMap()
        {
            // Crops
            foreach (var (seedId, cropData) in Game1.cropData)
            {
                string harvestId = cropData.HarvestItemId;
                if (!string.IsNullOrEmpty(harvestId))
                    _harvestToSeed[harvestId] = seedId;
            }

            // Fruit trees
            foreach (var (saplingId, treeData) in Game1.fruitTreeData)
            {
                var fruit = treeData.Fruit.FirstOrDefault();
                if (fruit != null && !string.IsNullOrEmpty(fruit.ItemId))
                    _harvestToSeed[fruit.ItemId] = saplingId;
            }

            //TODO CUSTOM BUSH
            // Custom Bushes
            //var api = CustomBushCompat.Api;
            //if (api != null)
            //{
            //    foreach (string bushId in api.GetAllBushIds())
            //    {
            //        // bushId is the SEED ID
            //        if (api.TryGetDrops(bushId, out var drops) && drops.Count > 0)
            //        {
            //            string harvestId = drops[0].ItemId; // HARVEST ID
            //            _harvestToSeed[harvestId] = bushId;
            //        }
            //    }
            //}
            //}
        }

        //---------------
        //Have harvest Id, get access to seed data in plantinfo
        //---------------
        public static CropData? GetSeedDataForHarvest(string harvestId)
        {
            if (_harvestToSeed.TryGetValue(harvestId, out string? seedId))
            {
                if (Game1.cropData.TryGetValue(seedId, out var seedData))
                    return seedData;
            }

            return null;
        }

    }
}
