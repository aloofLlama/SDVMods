using GiftDiscovery.Models;
using SDVCommon;
using SDVCommon.GameData;
using SDVCommon.Services;
using StardewModdingAPI;
using StardewValley;
using SObject = StardewValley.Object;


namespace GiftDiscovery.GameData.Static
{
    internal static class GiftableObjectList
    {
        private static List<SObject>? _giftableObjects;
        private static HashSet<string>? _giftableQIds;
        private static bool _isInitialized;

        internal static List<SObject> GetAllGiftableObjects()
        {
            EnsureInitialized();
            return _giftableObjects!;
        }

        internal static HashSet<string> GetAllGiftableIds()
        {
            EnsureInitialized();
            return _giftableQIds!;
        }

        internal static bool IsGiftableObject(string qId)
        {
            EnsureInitialized();
            return _giftableQIds!.Contains(qId);
        }

        //------------------------------------------------
        // Data lifecycle methods
        //------------------------------------------------
        private static void EnsureInitialized()
        {
            if (!_isInitialized)
                Initialize();
        }

        internal static void Initialize()
        {
            Build();
            _isInitialized = true;
        }
        internal static void Reset()
        {
            _giftableObjects = null;
            _giftableQIds = null;
            _isInitialized = false;
        }

        //------------------------------------------------
        // Builder
        //------------------------------------------------
        private static void Build()
        {
            string timer = "Build Giftable Object List";
            SDVCommonServices.PerfBegin(timer);

            var giftableNPCs = NPCGiftStatus.GetAllGiftableNPCs();

            _giftableObjects = new List<SObject>();
            _giftableQIds = new HashSet<string>();

            int cnt = 0;

            foreach (var (unqualifiedId, objData) in Game1.objectData) // Game1 returns unqualified Id
            {
                cnt++;

                var obj = GameObject.GetObjectInstance(unqualifiedId);

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
                        break;
                    }
                }

                if (!hasTaste || !hasLoveOrLike)
                    continue;

                _giftableObjects.Add(obj);
                _giftableQIds.Add(obj.QualifiedItemId);
            }

            SDVCommonServices.PerfEnd(timer, $"Giftable items: {_giftableQIds.Count} / {cnt}", 0);
        }
    }
}

