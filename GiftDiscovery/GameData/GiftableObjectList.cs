using GiftDiscovery.Models;
using GiftDiscovery.Services;
using SDVCommon;
using SDVCommon.GameData;
using SDVCommon.Services;
using StardewModdingAPI;
using StardewValley;
using SObject = StardewValley.Object;


namespace GiftDiscovery.GameData
{
    public static class GiftableObjectList
    {
        private static List<SObject>? _giftableObjects;
        private static HashSet<string>? _giftableIds;

        public static List<SObject> GetAllGiftableObjects()
        {
            if (_giftableObjects == null)
                BuildGiftableObjectList();

            return _giftableObjects!;
        }

        public static HashSet<string> GetAllGiftableIds()
        {
            if (_giftableIds == null)
                BuildGiftableObjectList();

            return _giftableIds!;
        }

        public static bool IsGiftableObject(string qId)
        {
            if (_giftableIds == null)
                BuildGiftableObjectList();

            return _giftableIds!.Contains(qId);
        }

        public static void Reset()
        {
            _giftableObjects = null;
            _giftableIds = null;
        }

        public static void Initialize()
        {
            GetAllGiftableObjects();
            GetAllGiftableIds();
        }

        private static void BuildGiftableObjectList()
        {
            string timer = "Build Giftable Object List";
            LogLevel logLevel = LogHelper.DebugOrTrace;
            SDVCommonServices.PerfBegin(timer);

            var giftableNPCs = GiftableNPCList.GetAllGiftableNPCs();

            _giftableObjects = new List<SObject>();
            _giftableIds = new HashSet<string>();

            int cnt = 0;

            foreach (var (unqualifiedId, objData) in Game1.objectData) // Game1 returns unqualified Id
            {
                cnt++;

                var obj = GameObject.GetObjectInstance(unqualifiedId);
                //var obj = ItemRegistry.Create(unqualifiedId) as SObject;

                if (obj == null || !obj.canBeGivenAsGift())
                    continue;

                if (unqualifiedId == "434") // Stardrop exclusion
                    continue;

                bool hasTaste = false;
                bool hasLoveOrLike = false;

                foreach (var npc in giftableNPCs)
                {
                    GiftTaste t;

                    try
                    {
                        t = (GiftTaste)npc.getGiftTasteForThisItem(obj);
                    }
                    catch
                    {
                        continue;
                    }

                    hasTaste = true;

                    if (t == GiftTaste.Love || t == GiftTaste.Like)
                    {
                        hasLoveOrLike = true;
                        break; // early exit
                    }
                }

                if (!hasTaste || !hasLoveOrLike)
                    continue;

                _giftableObjects.Add(obj);
                _giftableIds.Add(obj.QualifiedItemId);
            }

            SDVCommonServices.PerfEnd(timer, $"Giftable items: {_giftableIds.Count} / {cnt}", 0, logLevel);

        }


        private static HashSet<string> BuildUniversalLoves()
        {
            string timer = "Build Universal Loves";
            LogLevel logLevel = LogHelper.DebugOrTrace;
            SDVCommonServices.PerfBegin(timer);

            var result = new HashSet<string>();

            var giftableNPCs = GiftableNPCList.
                GetAllGiftableNPCs();
            var giftableQIds = GetAllGiftableIds();

            int totalGiftable = giftableNPCs.Count;
            int threshold = (int)(0.85 * totalGiftable);
            int notLoveThreshold = totalGiftable - threshold;

            int cnt = 0;

            foreach (string qId in giftableQIds)
            {
                int loveCount = 0;
                int notLoveCount = 0;

                foreach (var npc in giftableNPCs)
                {
                    var taste = TasteMap.GetTasteForNPCItemPair(qId, npc);

                    if (taste == GiftTaste.Love)
                        loveCount++;
                    else
                        notLoveCount++;

                    if (notLoveCount > notLoveThreshold)
                        break;
                }

                if (loveCount >= threshold)
                {
                    cnt++;
                    result.Add(qId);
                }
            }

            SDVCommonServices.PerfEnd(timer, $"Universal Loves: {cnt}", 0, logLevel);

            return result;
        }


    }

}

