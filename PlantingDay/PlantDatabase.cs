using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PlantingDay.Compatibility;
using PlantingDay.Helpers;
using PlantingDay.Models;
using StardewModdingAPI;
using StardewValley;
using StardewValley.GameData;
using StardewValley.GameData.Buffs;
using StardewValley.GameData.Crops;
using StardewValley.GameData.FruitTrees;
using StardewValley.GameData.Objects;
using StardewValley.GameData.Shops;
using StardewValley.Locations;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Xml.Linq;
using static StardewValley.Minigames.MineCart;
using SObject = StardewValley.Object;

namespace PlantingDay
{

    public static class PlantDatabase
    {
        private static bool _isInitialized;

        private static readonly Dictionary<string, PlantInfo> _plants = new();
        public static IEnumerable<PlantInfo> AllPlants => _plants.Values;
        public static IEnumerable<string> AllKeys() => _plants.Keys;


        private static Dictionary<string, List<(int itemId, float chance)>> _monsterDropTable = new();


        public static void Initialize()
        {
            if (_isInitialized)
                return;


            LoadCrops();
            LoadFruitTrees();
            //LoadBushes();

            _isInitialized = true;

            ModEntry.Instance.Monitor.Log(
                $"[{DateTime.Now:HH:mm:ss}] Plant Database Initialized",
                LogLevel.Alert);

            // KEEP Debug to output desired database variable
            /*
            foreach (var plant in PlantDatabase.AllPlants)
            {
                ModEntry.Instance.Monitor.Log($"Seed: {plant.SeedId}", LogLevel.Warn);
            }
            */

        }
        public static void Reset()
        {
            _isInitialized = false;
        }

        // Lookup by seed item ID (e.g. "O:472" for Parsnip Seeds)
        public static PlantInfo? LookupFromKey(string key)
        {
            return _plants.TryGetValue(key, out var info) ? info : null;
        }

        private static ItemInfo? FromObject(string objectId)
        {

            // 1) Try raw key first (works for modded IDs and numeric vanilla)
            if (!Game1.objectData.TryGetValue(objectId, out var obj))
            {
                // 2) Fallback: extract numeric ID for "(O)638" / "O:638" / "638"
                string numeric = new string(objectId.Where(char.IsDigit).ToArray());

                if (string.IsNullOrEmpty(numeric) ||
                    !Game1.objectData.TryGetValue(numeric, out obj))
                {
                    return null;
                }
            }



            return new ItemInfo
            {
                Id = objectId,
                Name = obj.DisplayName,
                Description = obj.Description,
                Price = obj.Price,
                Category = obj.Category,
                Edibility = obj.Edibility,
                Type = obj.Type
            };
        }



        //-------
        // Crops
        //-------

        private static void LoadCrops()
        {
            foreach (var (seedId, cropData) in Game1.cropData)
            {
                var plant = FromCrop(seedId, cropData);
                if (plant == null)
                    continue;

                _plants[plant.SeedId] = plant;
            }
        }

        public static PlantInfo FromCrop(string seedId, CropData crop)
        {
            var seedInfo = FromObject(seedId);

            var harvestId = crop.HarvestItemId ?? "";
            var harvestInfo = FromObject(harvestId);


            return new PlantInfo
            {
                SeedId = seedId,
                HarvestId = harvestId,
                PlantType = PlantType.Crop,

                Seasons = crop.Seasons?
                    .Select(s => Enum.Parse<SeasonId>(s.ToString(), ignoreCase: true))
                    .ToList()
                    ?? new List<SeasonId>(),

                DaysToProduce = crop.DaysInPhase?.Sum() ?? 0,
                RegrowDays = crop.RegrowDays > 0 ? crop.RegrowDays : null,

                Trellis = crop.IsRaised,
                Paddy = crop.IsPaddyCrop,
                MultiSprite = crop.TintColors?.Count ?? 0,
                NeedsWatering = crop.NeedsWatering,
                Scythe = crop.HarvestMethod,

                Seed = seedInfo,
                Harvest = harvestInfo,
                HarvestPrice = harvestInfo?.Price ?? 0,

                //Drops = new List<DropInfo>
                //    {
                //    new DropInfo
                //    {
                //        ItemId = harvestId,
                //        MinStack = crop.HarvestMinStack,
                //        MaxStack = crop.HarvestMaxStack,
                //        ExtraHarvestChance = crop.ExtraHarvestChance
                //    }
                //}.AsReadOnly(),

            };
        }

        //-----
        //Fruit Trees
        //-------
        private static void LoadFruitTrees()
        {
            foreach (var (saplingId, treeData) in Game1.fruitTreeData)
            {
                var plant = FromFruitTree(saplingId, treeData);
                if (plant == null)
                    continue;

                //ModEntry.Instance.Monitor.Log(
                //    $"LOAD FRUITTREE raw seed: {saplingId} -> plant seed: {plant.Seed?.Id}",
                //    LogLevel.Info);

                _plants[plant.SeedId] = plant;
            }
        }

        private static PlantInfo FromFruitTree(string saplingId, FruitTreeData data)
        {
            // Fruit tree always has exactly one fruit entry
            var fruitEntry = data.Fruit.FirstOrDefault();

            string fruitId = fruitEntry?.ItemId ?? "";

            var seedInfo = FromObject(saplingId);
            var harvestInfo = FromObject(fruitId);

            // Patch for Uncle Iroh Approved Tea which mismatches the (O) prefix
            if (harvestInfo == null && fruitId.StartsWith("(O)"))
            {
                string cleanId = fruitId.Substring(3); // remove "(O)"
                harvestInfo = FromObject(cleanId);
            }

            return new PlantInfo
            {
                SeedId = saplingId,
                HarvestId = fruitId,
                PlantType = PlantType.FruitTree,

                Seed = seedInfo,
                Harvest = harvestInfo,
                HarvestPrice = harvestInfo?.Price ?? 0,

                Seasons = data.Seasons?
                    .Select(s => Enum.Parse<SeasonId>(s.ToString(), ignoreCase: true))
                    .ToList()
                    ?? new List<SeasonId>(),

                // Fruit trees always take 28 days to mature
                DaysToProduce = 28,

                // After maturity, they produce daily
                RegrowDays = 1,

                //Drops = new List<DropInfo>
                //    {
                //    new DropInfo
                //    {
                //        ItemId = fruitId,
                //        MinStack = fruitEntry?.MinStack ?? 1,
                //        MaxStack = fruitEntry?.MaxStack ?? 1,
                //        ExtraHarvestChance = 0f
                //    }

                //}.AsReadOnly(),

            };

        }


    }
}


