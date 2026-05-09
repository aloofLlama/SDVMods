using SDVCommon.Compatibility;
using SDVCommon.GameData;
using SDVCommon.Helpers;
using SDVCommon.Models.Wrappers;
using SDVData;
using StardewModdingAPI;
using StardewValley;
using StardewValley.GameData.Crops;
using StardewValley.GameData.FruitTrees;
using StardewValley.GameData.Shops;


namespace SDVCommon.Models.Builders
{

    public static class PlantInfoBuilder
    {
        private static bool _isInitialized;

        private static readonly Dictionary<string, PlantInfo> _plants = new();
        public static IEnumerable<PlantInfo> AllPlants => _plants.Values;
        public static IEnumerable<string> AllKeys() => _plants.Keys;

        public static void Initialize()
        {
            if (_isInitialized)
                return;

            LoadCrops();
            LoadFruitTrees();
            LoadCustomBushes();

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
                    Location = GetLocation(cropData),

                    Seed = GameObjectInfo.FromObject(seedId),
                };
                data.PurchaseOptions = PurchaseDataBuilder.GetPurchaseInfo(seedId);
                data.MonsterDrops = MonsterDropBuilder.GetDropsForItem(seedId);

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

                //string fruitId = fruitEntry?.ItemId ?? "";
                string fruitId = IdHelper.CanonicalItemId(fruitEntry?.ItemId);


                var harvestInfo = GameObjectInfo.FromObject(fruitId);

                var data = new PlantInfoData
                {
                    SeedId = saplingId,
                    HarvestId = fruitId,
                    PlantType = PlantType.FruitTree,

                    Seed = GameObjectInfo.FromObject(saplingId),

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
                data.MonsterDrops = MonsterDropBuilder.GetDropsForItem(saplingId);

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
                    Seed = GameObjectInfo.FromObject(qualifiedId),
                    //Seasons = new() { SeasonId.Spring, SeasonId.Summer, SeasonId.Fall },
                    DaysToProduce = 20,
                    RegrowDays = 1
                };

                data.PurchaseOptions = PurchaseDataBuilder.GetPurchaseInfo(qualifiedId);
                data.MonsterDrops = MonsterDropBuilder.GetDropsForItem(qualifiedId);

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

        private static Location GetLocation(CropData cropData)
        {
            // Default
            var result = Location.Unrestricted;

            if (cropData.PlantableLocationRules != null)
            {
                foreach (var rule in cropData.PlantableLocationRules)
                {
                    if (rule.Id == "NotOutsideUnlessGingerIsland")
                    {
                        result = Location.Indoor;
                        break;
                    }

                    if (rule.Id == "NoGardenPots")
                    {
                        result = Location.NoGardenPot;
                        break;
                    }
                }
            }

            return result;
        }


    }



}


