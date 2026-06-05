using SDVCommon.GameData;
using SDVCommon.Helpers;
using SDVCommon.Models.Wrappers;
using SDVCommon.Compatibility;
using SDVData;
using StardewModdingAPI;
using StardewValley;
using StardewValley.GameData.Crops;
using StardewValley.GameData.FruitTrees;
using StardewValley.GameData.Objects;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Text;
using SObject = StardewValley.Object;



namespace SDVCommon.Models.Builders
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
            _harvests.Clear();
        }

        public static HarvestInfo? LookupFromKey(string key)
        {
            return _harvests.TryGetValue(key, out var info) ? info : null;
        }


        private static void LoadFromObjectData()
        {
            foreach (var (id, obj) in Game1.objectData)
            {
                // Explicit exclusions
                if (id == "434") // Stardrop is a game object but not a real item to get
                    continue;

                // Include by category
                if (HarvestCategories.IsDesiredCategory(obj))
                {
                    AddHarvestIfMissing(id, obj);
                    continue;
                }

            }
        }


        //Only need one copy if there are multiple sources, so only add if it has not already been added
        private static void AddHarvestIfMissing(string harvestId, ObjectData obj)
        {
            if (string.IsNullOrEmpty(harvestId))
                return;

            if (_harvests.ContainsKey(harvestId))
                return;

            var itemInfo = GameObject.FromObject(harvestId);
            if (itemInfo == null)
                return;

            GameDataHelper._harvestToSeed.TryGetValue(harvestId, out string? seedId);


            bool shipOne = IsShipOneCandidate(harvestId, obj);

            // Mono/Poly shipping achievements come from the seed data
            var seedData = GameDataHelper.GetSeedDataForHarvest(harvestId);
            bool shipMono = seedData?.CountForMonoculture == true;
            bool shipPoly = seedData?.CountForPolyculture == true;

            var data = new HarvestInfoData
            {
                HarvestId = harvestId,
                SeedId = seedId,
                Harvest = itemInfo,
                ShipOne = shipOne,
                ShipMonoCulture = shipMono,
                ShipPolyCulture = shipPoly,
                ModSource = ModSourceHelper.GetModSource(harvestId)
            };

            _harvests[harvestId] = new HarvestInfo(data);
        }

        public static bool IsShipOneCandidate(string itemId, ObjectData data)
        {
            // Exclusions
            if (data.Type == "Arch"
                || data.Type == "Fish"
                || data.Type == "Mineral"
                || data.Type == "Cooking")
                return false;

            // Only items the game considers "basic shipped"
            if (!SObject.isPotentialBasicShipped(itemId, data.Category, data.Type))
                return false;

            return true;
        }
    }
}

