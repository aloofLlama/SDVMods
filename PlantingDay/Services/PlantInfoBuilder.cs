using PlantingDay.Compatibility;
using PlantingDay.Helpers;
using PlantingDay.Helpers.SeedSource;
using PlantingDay.Models.Wrappers;
using SDVCommon.Compatibility;
using SDVCommon.GameData;
using SDVCommon.Helpers;
using SDVData;
using StardewModdingAPI;
using StardewValley;
using StardewValley.GameData.Crops;
using StardewValley.GameData.FruitTrees;


namespace PlantingDay.Services
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
            LoadCustomBushes();

            foreach (var plant in _plants.Values)
            {
                string seedId = plant.Data.SeedId;



            }

            _isInitialized = true;



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
                // Build raw data object
                var data = new PlantInfoData
                {
                    SeedId = seedId,
                    HarvestId = cropData.HarvestItemId ?? "",
                    PlantType = PlantType.Crop,

                    Seasons = cropData.Seasons?
                        .Select(s => Enum.Parse<SeasonId>(s.ToString(), ignoreCase: true))
                        .ToList()
                        ?? new List<SeasonId>(),

                    DaysToProduce = cropData.DaysInPhase?.Sum() ?? 0,
                    RegrowDays = cropData.RegrowDays > 0 ? cropData.RegrowDays : null,

                    Trellis = cropData.IsRaised,
                    Paddy = cropData.IsPaddyCrop,
                    MultiSprite = cropData.TintColors?.Count ?? 0,
                    NeedsWatering = cropData.NeedsWatering,
                    NeedsScythe = cropData.HarvestMethod == HarvestMethod.Scythe,

                    Seed = GameObjectInfoHelper.FromObject(seedId),
                };
                data.PurchaseOptions = PurchaseDataBuilder.GetPurchaseInfo(seedId);
                data.MonsterDrops = MonsterDropLoader.GetDropsForItem(seedId);

                //// Apply Monster Drop Overrides
                //if (SeedOverrides.MonsterDrops.TryGetValue(seedId, out var overrideList))
                //{
                //    data.MonsterDrops = overrideList
                //        .Select(m => new MonsterDropInfoData
                //        {
                //            MonsterName = m.Monster,
                //            DropChance = (float)m.Chance
                //        })
                //        .ToList();
                //}


                var plant = new PlantInfo(data);
                _plants[data.SeedId] = plant;
            }
        }

        //-----
        //Fruit Trees
        //-------
        private static void LoadFruitTrees()
        {
            foreach (var (saplingId, fruitTreeData) in Game1.fruitTreeData)
            {
                // Fruit tree always has exactly one fruit entry
                var fruitEntry = fruitTreeData.Fruit.FirstOrDefault();

                string fruitId = fruitEntry?.ItemId ?? "";

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

                    Seed = GameObjectInfoHelper.FromObject(saplingId),

                    Seasons = fruitTreeData.Seasons?
                        .Select(s => Enum.Parse<SeasonId>(s.ToString(), ignoreCase: true))
                        .ToList()
                        ?? new List<SeasonId>(),

                    // Fruit trees always take 28 days to mature
                    DaysToProduce = 28,

                    // After maturity, they produce daily
                    RegrowDays = 1,


                };
                data.PurchaseOptions = PurchaseDataBuilder.GetPurchaseInfo(saplingId);
                data.MonsterDrops = MonsterDropLoader.GetDropsForItem(saplingId);

                //// Apply Monster Drop Overrides
                //if (SeedOverrides.MonsterDrops.TryGetValue(saplingId, out var overrideList))
                //{
                //    data.MonsterDrops = overrideList
                //        .Select(m => new MonsterDropInfoData
                //        {
                //            MonsterName = m.Monster,
                //            DropChance = (float)m.Chance
                //        })
                //        .ToList();
                //}



                var plant = new PlantInfo(data);
                _plants[data.SeedId] = plant;
            }
        }

        private static void LoadCustomBushes()
        {
            var api = CustomBushCompat.Api;

            if (api == null)
            {
                //LoadVanillaTeaBush();
                return;
            }

            foreach (var (qualifiedId, objData) in Game1.objectData)
            {
                // Convert to CustomBush key: add (O) if missing
                string cbKey = IdHelper.ToQualifiedId(qualifiedId);

                if (!api.TryGetDrops(cbKey, out var drops) || drops.Count == 0)
                    continue;

                string harvestId = drops[0].ItemId;

                var data = new PlantInfoData
                {
                    SeedId = qualifiedId,
                    HarvestId = harvestId,
                    PlantType = PlantType.Bush,
                    Seed = GameObjectInfoHelper.FromObject(qualifiedId),
                    //Seasons = new() { SeasonId.Spring, SeasonId.Summer, SeasonId.Fall },
                    DaysToProduce = 20,
                    RegrowDays = 1
                };

                data.PurchaseOptions = PurchaseDataBuilder.GetPurchaseInfo(qualifiedId);
                data.MonsterDrops = MonsterDropLoader.GetDropsForItem(qualifiedId);

                _plants[data.SeedId] = new PlantInfo(data);
            }

            //LoadVanillaTeaBush();
        }


        //        private static void LoadCustomBush()
        //        {

        //            if (
        //              ApiManager.GetApi(ModCompat.CustomBush, out ICustomBushApi? customBushApi)
        //              && customBushApi.TryGetDrops(item.QualifiedItemId, out IList<ICustomBushDrop>? drops)
        //              && drops.Count > 0
        //            )
        //            {
        //              return ItemRegistry.Create<SObject>(drops[0].ItemId);
        //            }

        //            // Vanilla tea sapling fallback (no Custom Bush mod)
        //            if (item.QualifiedItemId == "(O)251")
        //            {
        //              return ItemRegistry.Create<SObject>("(O)614");
        //            }

        //return null;
        //          }




    }
}


