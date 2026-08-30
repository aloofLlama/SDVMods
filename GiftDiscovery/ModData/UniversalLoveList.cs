using GiftDiscovery.GameData;
using GiftDiscovery.Models;
using SDVCommon;
using SDVCommon.Services;
using StardewModdingAPI;

namespace GiftDiscovery.ModData
{
    internal static class UniversalLoveList
    {
        private static HashSet<string>? _universalLoveIds;
        private static bool _isInitialized;

        internal static HashSet<string> GetAllUniversalLoves()
        {
            EnsureInitialized();
            return _universalLoveIds!;
        }

        internal static bool IsUniversalLove(string qId)
        {
            EnsureInitialized();
            return _universalLoveIds!.Contains(qId);
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
            _universalLoveIds = null;
            _isInitialized = false;
        }

        //------------------------------------------------
        // Builder
        //------------------------------------------------
        private static void Build()
        {
            string timer = "Build Universal Loves";
            LogLevel logLevel = LogHelper.DebugOrTrace;
            SDVCommonServices.PerfBegin(timer);

            _universalLoveIds = new HashSet<string>();

            var giftableNPCs = NPCGiftStatus.GetAllGiftableNPCs();
            var giftableIds = GiftableObjectList.GetAllGiftableIds();

            int totalGiftable = giftableNPCs.Count;
            int threshold = (int)(0.85 * totalGiftable);
            int notLoveThreshold = totalGiftable - threshold;

            int cnt = 0;

            foreach (string qualifiedItemId in giftableIds)
            {
                int loveCount = 0;
                int notLoveCount = 0;

                foreach (var npc in giftableNPCs)
                {
                    var taste = TasteMap.GetTasteForNPCItemPair(qualifiedItemId, npc);

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
                    _universalLoveIds.Add(qualifiedItemId);
                }
            }

            SDVCommonServices.PerfEnd(timer, $"Universal Loves: {cnt}", 0, logLevel);

        }
    }
}

