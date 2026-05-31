using GiftDiscovery.GameData;
using GiftDiscovery.Models;
using GiftDiscovery.Models.Builders;
using SDVCommon.Helpers;
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
        public static bool IsUniversalLove(string itemId)
        {
            var item = ItemRegistry.Create(itemId);

            // Count all giftable NPCs
            int totalGiftable = GiftableNPC.GetAllGiftableNPCs().Count();

            if (totalGiftable == 0)
                return false;

            // Count how many love this item
            int loveCount = GetLovedBy(item).Count();

            // Universal love = 80% or more
            return loveCount >= (int)(0.8 * totalGiftable);
        }

        // ---------------------------------------------------------
        // ITEM → NPC (Loves)
        // ---------------------------------------------------------
        public static IEnumerable<NPC> GetLovedBy(Item item)
        {
            foreach (var npc in GiftableNPC.GetAllGiftableNPCs())
            {
                GiftTaste? taste = null;

                try
                {
                    taste = (GiftTaste)npc.getGiftTasteForThisItem(item);
                }
                catch
                {
                    SDVCommonLog.Log($"Missing Gift Info: {npc.displayName} | {item.DisplayName}",
                        LogLevel.Warn);
                    continue;
                }

                if (taste == GiftTaste.Love)
                    yield return npc;
            }
        }


    }
}
