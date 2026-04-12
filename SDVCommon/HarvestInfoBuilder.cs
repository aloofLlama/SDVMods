using PlantingDay.Models.Wrappers;
using SDVData;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Text;
using StardewValley.GameData.Crops;
using StardewValley.GameData.FruitTrees;
using StardewModdingAPI;
using SDVCommon.GameData;



namespace SDVCommon
{
    internal class HarvestInfoBuilder
    {
        private static bool _isInitialized;


        private static readonly Dictionary<string, HarvestInfo> _harvests = new();
        public static IEnumerable<HarvestInfo> AllHarvests => _harvests.Values;
        public static IEnumerable<string> AllKeys() => _harvests.Keys;

        public static void Initialize()
        {
            if (_isInitialized)
                return;

            LoadFromCrops();
            LoadFromFruitTrees();
            //LoadFromCustomBushes(); // future

            // Deduplicate automatically because dictionary keyed by HarvestId
        }
        public static void Reset()
        {
            _isInitialized = false;
        }

        public static HarvestInfo? LookupFromKey(string key)
        {
            return _harvests.TryGetValue(key, out var info) ? info : null;
        }


        private static void LoadFromCrops()
        {
            foreach (var (_, cropData) in Game1.cropData)
            {
                string harvestId = cropData.HarvestItemId ?? "";
                AddHarvestIfMissing(harvestId);
            }
        }

        private static void LoadFromFruitTrees()
        {
            foreach (var (_, treeData) in Game1.fruitTreeData)
            {
                var fruitEntry = treeData.Fruit.FirstOrDefault();
                if (fruitEntry == null)
                    continue;

                string harvestId = fruitEntry.ItemId ?? "";
                AddHarvestIfMissing(harvestId);
            }
        }

        //Only need one copy if there are multiple sources, so only add if it has not already been added
        private static void AddHarvestIfMissing(string harvestId)
        {
            if (string.IsNullOrEmpty(harvestId))
                return;

            if (_harvests.ContainsKey(harvestId))
                return;

            var itemInfo = GameObjectInfoHelper.FromObject(harvestId);
            if (itemInfo == null)
                return;

            var data = new HarvestInfoData
            {
                HarvestId = harvestId,
                HarvestItem = itemInfo
            };

            _harvests[harvestId] = new HarvestInfo(data);
        }
    }
}

