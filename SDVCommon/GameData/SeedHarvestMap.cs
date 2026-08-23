using SDVCommon.GameData;
using SDVCommon.Models.Builders;
using SDVCommon.Models.Wrappers;
using StardewValley;
using StardewValley.GameData.Crops;
using SObject = StardewValley.Object;


namespace SDVCommon.Helpers
{
    internal class SeedHarvestMap
    {
        private static bool _isInitialized;
        public static readonly Dictionary<string, string> _seedToHarvest = new();
        private static readonly Dictionary<string, string> _harvestToSeed = new();

        //---------------
        // Convert between seed/harvest Ids
        //---------------
        public static bool TryGetHarvestId(string seedQId, out string? harvestQId)
        {
            if (!_isInitialized)
                Initialize();

            return _seedToHarvest.TryGetValue(seedQId, out harvestQId);
        }

        public static bool TryGetSeedId(string harvestQId, out string? seedQId)
        {
            if (!_isInitialized)
                Initialize();

            return _harvestToSeed.TryGetValue(harvestQId, out seedQId);
        }


        public static void Initialize()
        {
            if (_isInitialized)
                return;

            BuildSeedAndHarvestMaps();
            _isInitialized = true;

        }

        public static void Reset()
        {
            _isInitialized = false;
            _seedToHarvest.Clear();
        }

        public static void BuildSeedAndHarvestMaps()
        {
            // Crops
            foreach (var (seedUnqualifiedId, cropData) in Game1.cropData)
            {
                string harvestUnqualifiedId = cropData.HarvestItemId;

                string seedQId = IdHelper.ToQualifiedId(seedUnqualifiedId);
                string harvestQId = IdHelper.ToQualifiedId(harvestUnqualifiedId);

                if (!string.IsNullOrEmpty(harvestQId))
                {
                    _seedToHarvest[seedQId] = harvestQId;
                    _harvestToSeed[harvestQId] = seedQId;
                }
            }

            // Fruit trees
            foreach (var (saplingUnqualifiedId, treeData) in Game1.fruitTreeData)
            {
                var fruitData = treeData.Fruit.FirstOrDefault();

                if (fruitData == null)
                    return;

                string seedQId = IdHelper.ToQualifiedId(saplingUnqualifiedId);
                string harvestQId = IdHelper.ToQualifiedId(fruitData.ItemId);

                if (!string.IsNullOrEmpty(harvestQId))
                {
                    _seedToHarvest[seedQId] = harvestQId;
                    _harvestToSeed[harvestQId] = seedQId;
                }
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
        //public static CropData? GetSeedDataForHarvest(string harvestId)
        //{
        //    if (_harvestToSeed.TryGetValue(harvestId, out string? seedId))
        //    {
        //        if (Game1.cropData.TryGetValue(seedId, out var seedData))
        //            return seedData;
        //    }

        //    return null;
        //}

    }
}
