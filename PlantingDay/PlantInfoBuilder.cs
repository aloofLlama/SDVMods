using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PlantingDay.Compatibility;
using PlantingDay.Helpers;
using PlantingDay.Helpers.SeedSource;
using PlantingDay.Models.Wrappers;
using SDVData;
using SDVCommon.GameData;
using StardewModdingAPI;
using StardewValley;
using StardewValley.GameData;
using StardewValley.GameData.Buffs;
using StardewValley.GameData.Crops;
using StardewValley.GameData.FruitTrees;


namespace PlantingDay
{

    public static class PlantInfoBuilder
    {
        private static bool _isInitialized;

        private static readonly Dictionary<string, PlantInfo> _plants = new();
        public static IEnumerable<PlantInfo> AllPlants => _plants.Values;
        public static IEnumerable<string> AllKeys() => _plants.Keys;


        //private static Dictionary<string, List<(int itemId, float chance)>> _monsterDropTable = new();


        public static void Initialize()
        {
            if (_isInitialized)
                return;

            LoadCrops();
            LoadFruitTrees();
            //LoadBushes(); // if you add this later

            // Populate purchase info, monster drops
            foreach (var plant in _plants.Values)
            {
                string seedId = plant.Data.SeedId;

                plant.Data.PurchaseOptions =
                    PurchaseDataBuilder.GetPurchaseInfo(seedId);

                plant.Data.MonsterDrops =
                    MonsterDropLoader.GetDropsForItem(seedId);

            }

            _isInitialized = true;

            ModEntry.Instance.Monitor.Log(
                "Plant Database Initialized",
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

                _plants[plant.Data.SeedId] = plant;
            }
        }

        public static PlantInfo FromCrop(string seedId, CropData crop)
        {
            var seedInfo = GameObjectInfoHelper.FromObject(seedId);

            var harvestId = crop.HarvestItemId ?? "";

            var data = new SDVData.PlantInfoData
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
                NeedsScythe = crop.HarvestMethod == HarvestMethod.Scythe,

                Seed = seedInfo,

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
            return new PlantInfo(data);
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

                _plants[plant.Data.SeedId] = plant;
            }
        }

        private static PlantInfo FromFruitTree(string saplingId, FruitTreeData fruitTree)
        {
            // Fruit tree always has exactly one fruit entry
            var fruitEntry = fruitTree.Fruit.FirstOrDefault();

            string fruitId = fruitEntry?.ItemId ?? "";

            var seedInfo = GameObjectInfoHelper.FromObject(saplingId);
            var harvestInfo = GameObjectInfoHelper.FromObject(fruitId);

            // Patch for Uncle Iroh Approved Tea which mismatches the (O) prefix
            if (harvestInfo == null && fruitId.StartsWith("(O)"))
            {
                string cleanId = fruitId[3..]; // remove "(O)"
                harvestInfo = GameObjectInfoHelper.FromObject(cleanId);
            }

            var data = new PlantInfoData
            {
                SeedId = saplingId,
                HarvestId = fruitId,
                PlantType = PlantType.FruitTree,

                Seed = seedInfo,

                Seasons = fruitTree.Seasons?
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
            return new PlantInfo(data);

        }

        //Bushes. Try this code for accessing custom bush drops from UI Info Suite Alt 2 -> Infrastructure -> Tools:
        // Custom Bush saplings and vanilla tea sapling
        //    if (
        //      ApiManager.GetApi(ModCompat.CustomBush, out ICustomBushApi? customBushApi)
        //      && customBushApi.TryGetDrops(item.QualifiedItemId, out IList<ICustomBushDrop>? drops)
        //      && drops.Count > 0
        //    )
        //    {
        //      return ItemRegistry.Create<SObject>(drops[0].ItemId);
        //    }

        //    // Vanilla tea sapling fallback (no Custom Bush mod)
        //    if (item.QualifiedItemId == "(O)251")
        //    {
        //      return ItemRegistry.Create<SObject>("(O)614");
        //    }

        //return null;
        //  }




    }
}


