using SDVCommon.Compatibility;
using SDVCommon.GameData;
using SDVCommon.Helpers;
using SDVCommon.Services;
using SDVData;
using StardewModdingAPI;
using StardewValley;
using StardewValley.GameData.Objects;
using SObject = StardewValley.Object;

namespace SDVCommon.GameData.Dictionaries
{
    internal class Harvest
    {
        private static bool _isInitialized;

        // Harvests keyed on qualified id (qId) for the harvest item. 
        private static readonly Dictionary<string, HarvestInfo> _harvests = new();
        //public static IEnumerable<HarvestInfo> AllHarvests => _harvests.Values;
        //public static IEnumerable<string> AllKeys() => _harvests.Keys;

        //-----
        // For a single harvest item
        //-----
        public static bool IsHarvestObject(string qId)
        {
            if (!_isInitialized)
                Initialize();

            return _harvests.ContainsKey(qId);
        }

        public static HarvestInfo? GetHarvestInfo(string qId)
        {
            if (!_isInitialized)
                Initialize();

            return _harvests.TryGetValue(qId, out var info) ? info : null;
        }

        //-----
        // The whole harvest list
        //-----
        public static IEnumerable<string> GetAllHarvestIds()
        {
            if (!_isInitialized)
                Initialize();

            return _harvests.Keys;
        }

        public static IEnumerable<HarvestInfo> GetAllHarvests
        {
            get
            {
                if (!_isInitialized)
                    Initialize();

                return _harvests.Values;
            }
        }


        public static void Initialize()
        {
            if (_isInitialized)
                return;

            //LoadFromObjectData();
            BuildAllHarvestObjects();
            _isInitialized = true;

        }

        public static void Reset()
        {
            _isInitialized = false;
            _harvests.Clear();
        }

        private static void BuildAllHarvestObjects()
        {
            string timer = "Build Harvest Object List";
            LogLevel logLevel = LogHelper.DebugOrTrace;
            SDVCommonServices.PerfBegin(timer);

            foreach (var (unqualifiedId, objData) in Game1.objectData)
            {
                // Explicit exclusions
                if (unqualifiedId == "434") // Stardrop is a game object but not a real item to get
                    continue;

                string qId = IdHelper.ToQualifiedId(unqualifiedId);

                // Include by category
                if (HarvestCategories.IsDesiredCategory(objData))
                {
                    AddHarvestIfMissing(qId, objData);
                    continue;
                }

            }
            SDVCommonServices.PerfEnd(timer, 0, logLevel);

        }





        //private static void LoadFromObjectData()
        //{
        //    foreach (var (unqualifiedId, data) in Game1.objectData)
        //    {
        //        // Explicit exclusions
        //        if (unqualifiedId == "434") // Stardrop is a game object but not a real item to get
        //            continue;

        //        // Include by category
        //        if (HarvestCategories.IsDesiredCategory(data))
        //        {
        //            AddHarvestIfMissing(unqualifiedId, data);
        //            continue;
        //        }

        //    }
        //}


        //Only need one copy if there are multiple sources, so only add if it has not already been added
        private static void AddHarvestIfMissing(string harvestQId, ObjectData objData)
        {
            if (string.IsNullOrEmpty(harvestQId))
                return;

            if (_harvests.ContainsKey(harvestQId))
                return;

            var objInfo = GameObject.GetObjectInfo(harvestQId);
            if (objInfo == null)
                return;

            string unqualifiedHarvestId = IdHelper.ToUnqualifiedItemId(harvestQId);
            //bool shipOne = IsShipOneCandidate(harvestId, objData);
            bool shipOne = SObject.isPotentialBasicShipped(unqualifiedHarvestId, objData.Category, objData.Type);
            bool shipMono = false;
            bool shipPoly = false;


            SeedHarvestMap.TryGetSeedId(harvestQId, out string? seedQId);
            if (seedQId != null)
            {
                string unqualifiedSeedId = IdHelper.ToUnqualifiedItemId(seedQId);
                Game1.cropData.TryGetValue(unqualifiedSeedId, out var seedData);

                // Mono/Poly shipping achievements come from the seed data
                shipMono = seedData?.CountForMonoculture == true;
                shipPoly = seedData?.CountForPolyculture == true;
            }

            _harvests[harvestQId] = new HarvestInfo
            {
                HarvestQId = harvestQId,
                //DisplayName = obj.DisplayName,
                DisplayName = objInfo.DisplayName,
                SeedQId = seedQId,
                Category = objInfo.Category,
                ModSource = ModSourceHelper.GetModSource(harvestQId),
                //Harvest = objInfo,
                ShipOne = shipOne,
                ShipMonoCulture = shipMono,
                ShipPolyCulture = shipPoly
            };

        }

        public static bool IsShipOneCandidate(string itemId, ObjectData data)
        {
            // Exclusions
            // I THINK POTENTIAL BASIC SHIPPED CATCHES THESE
            //if (data.Type == "Arch"
            //    || data.Type == "Fish"
            //    || data.Type == "Mineral"
            //    || data.Type == "Cooking")
            //    return false;

            // Only items the game considers "basic shipped"
            if (!SObject.isPotentialBasicShipped(itemId, data.Category, data.Type))
                return false;

            return true;
        }
    }
}

