using GiftDiscovery.GameData;
using GiftDiscovery.Models;
using GiftDiscovery.Services;
using GiftDiscovery.Tooltip;
using SDVCommon.Helpers;
using SDVCommon.Icons;
using SDVCommon.Models.Builders;
using StardewModdingAPI;
using StardewValley;
using System.Diagnostics;
using static SDVCommon.TooltipRenderer;

namespace GiftDiscovery.Helpers
{
    public static class Initializer
    {
        public static void InitializeAll(IModHelper helper)
        {
            ModEntry.Instance.Monitor.Log($"[{DateTime.Now:HH:mm:ss}]", LogLevel.Warn);

            TooltipIcons.Initialize();
            HarvestInfoBuilder.Initialize();
            GiftableObjectList.Initialize();

            GiftKnowledgeService.InitializeGlobal(helper);
            GiftKnowledgeService.InitializeLocal(helper);

            GiftTooltipBuilder.Initialize();
            NPCGiftTooltipBuilder.Initialize();

            foreach (var npc in GiftableNPC.GetAllGiftableNPCs())
                GiftKnowledgeService.GetCanonicalTasteMap(npc);

            foreach (var obj in GiftableObjectList.AllGiftable)
            {
                string harvestKey = IdHelper.CanonicalItemId(obj.QualifiedItemId);
                var info = HarvestInfoBuilder.LookupFromKey(harvestKey);

                if (info != null)
                    IconInitializers.HarvestIcons(info);
            }

            SDVCommonLog.Log($"Gift Discovery Initialized",
                LogHelper.DebugOrTrace);

            SDVCommonLog.Log($"Giftable count = {GiftableObjectList.AllGiftable.Count}",
                LogHelper.DebugOrTrace);
            ModEntry.Instance.Monitor.Log($"[{DateTime.Now:HH:mm:ss}]", LogLevel.Warn);

        }
        public static void ResetAll(IModHelper helper)
        {
            GiftableObjectList.Reset();
            GiftTooltipBuilder.Reset();
            NPCGiftTooltipBuilder.Reset();

        }



    }
}
