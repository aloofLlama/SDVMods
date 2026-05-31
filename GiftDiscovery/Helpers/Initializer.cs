using GiftDiscovery.Compatibility;
using GiftDiscovery.GameData;
using GiftDiscovery.Models;
using GiftDiscovery.Services;
using GiftDiscovery.Tooltip;
using SDVCommon.Helpers;
using SDVCommon.Icons;
using SDVCommon.Models.Builders;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Characters;
using System.Diagnostics;
using static SDVCommon.TooltipRenderer;

namespace GiftDiscovery.Helpers
{
    public static class Initializer
    {
        public static void InitializeAll(IModHelper helper)
        {
            TooltipIcons.Initialize();
            APIManager.LoadApis(helper);

            HarvestInfoBuilder.Initialize();
            GiftableObjectList.Initialize();

            GiftKnowledgeService.InitializeGlobal(helper);
            GiftKnowledgeService.InitializeLocal(helper);

            GiftTooltipBuilder.Initialize();
            NPCGiftTooltipBuilder.Initialize();

            int cntgift = 0;
            foreach (var obj in GiftableObjectList.AllGiftable)
            {
                string harvestKey = IdHelper.CanonicalItemId(obj.QualifiedItemId);
                var info = HarvestInfoBuilder.LookupFromKey(harvestKey);

                if (info != null)
                {
                    IconInitializers.HarvestIcons(info);
                    cntgift++;
                }
            }

            SDVCommonLog.Log(
                $"[{DateTime.Now:HH:mm:ss}] Start get taste map",
                LogHelper.DebugOrTrace);

            foreach (var npc in GiftableNPC.GetAllGiftableNPCs())
            {
                GiftKnowledgeService.GetCanonicalTasteMap(npc);
            }

            SDVCommonLog.Log(
                $"[{DateTime.Now:HH:mm:ss}] End get taste map",
                LogHelper.DebugOrTrace);


#if DEBUG //TODO troubleshoot missing icons
            foreach (var obj in GiftableObjectList.AllGiftable)
            {
                string harvestKey = IdHelper.CanonicalItemId(obj.QualifiedItemId);
                var info = HarvestInfoBuilder.LookupFromKey(harvestKey);

                if (info == null)
                {
                    SDVCommonLog.Log(
                        $"Missing HarvestInfo for {obj.QualifiedItemId} (canonical={harvestKey})",
                        LogHelper.DebugOrTrace
                    );
                    continue;
                }

                if (info.Runtime.HarvestIcon == null)
                {
                    SDVCommonLog.Log(
                        $"Missing ICON for {obj.QualifiedItemId} (canonical={harvestKey})",
                        LogHelper.DebugOrTrace
                    );
                }
            }
#endif

            SDVCommonLog.Log($"Gift Discovery Initialized",
                LogHelper.DebugOrTrace);

            SDVCommonLog.Log($"Giftable items: {GiftableObjectList.AllGiftable.Count} | Gift icons: {cntgift}",
                LogHelper.DebugOrTrace);

            SDVCommonLog.Log($"Giftable NPCs: {GiftableNPC.GetAllGiftableNPCs().Count()}",
                LogHelper.DebugOrTrace);

    

        }
        public static void ResetAll()
        {
            HarvestInfoBuilder.Reset();
            GiftableObjectList.Reset();
            GiftTooltipBuilder.Reset();
            NPCGiftTooltipBuilder.Reset();
            GiftKnowledgeService.ResetCanonicalTasteCache();
        }
    }
}
