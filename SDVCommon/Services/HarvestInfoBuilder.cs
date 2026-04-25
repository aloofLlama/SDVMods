using SDVData;
using StardewValley;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Text;
using StardewValley.GameData.Crops;
using StardewValley.GameData.FruitTrees;
using StardewModdingAPI;
using SDVCommon.GameData;
using SDVCommon.Models.Wrappers;
using SDVCommon.Helpers;

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

            LoadFromObjectData();
        }
        public static void Reset()
        {
            _isInitialized = false;
        }

        public static HarvestInfo? LookupFromKey(string key)
        {
            return _harvests.TryGetValue(key, out var info) ? info : null;
        }


        private static void LoadFromObjectData()
        {
            foreach (var (id, obj) in Game1.objectData)
            {
                // Include by category
                if (HarvestCategories.IsDesiredCategory(obj.Category))
                {
                    AddHarvestIfMissing(id);
                    continue;
                }

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
                Harvest = itemInfo,
                Price = itemInfo.Price,
            };

            _harvests[harvestId] = new HarvestInfo(data);
        }
    }
}

