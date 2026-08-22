using GiftDiscovery.GameData;
using GiftDiscovery.Models;
using SDVCommon;
using SDVCommon.Services;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GiftDiscovery.Helpers
{
    public class GiftType
    {
        private static HashSet<string>? _universalLoveIds;

        public static HashSet<string> GetUniversalLoveIds()
        {
            if (_universalLoveIds == null)
                _universalLoveIds = BuildUniversalLoves();

            return _universalLoveIds!;
        }

        public static void Reset()
        {
            _universalLoveIds = null;
        }

        public static void Initialize()
        {
            GetUniversalLoveIds();
        }


        private static HashSet<string> BuildUniversalLoves()
        {
            string timer = "Build Universal Loves";
            LogLevel logLevel = LogHelper.DebugOrTrace;
            SDVCommonServices.PerfBegin(timer);

            var result = new HashSet<string>();

            var giftableNPCs = GiftableNPC.GetAllGiftableNPCs();
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
                    result.Add(qualifiedItemId);
                }
            }

            SDVCommonServices.PerfEnd(timer, $"Universal Loves: {cnt}", 0, logLevel);

            return result;
        }


    }
}
