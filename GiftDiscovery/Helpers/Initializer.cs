using GiftDiscovery.Services;
using SDVCommon.Services;
using SDVCommon.Helpers;
using SDVCommon.Icons;
using StardewModdingAPI;
using static SDVCommon.TooltipRenderer;

namespace GiftDiscovery.Helpers
{
    public static class Initializer
    {
        public static void InitializeAll(IModHelper helper)
        {
            TooltipIcons.Initialize();
            GiftKnowledgeService.InitializeLocal(helper);
            HarvestInfoBuilder.Initialize();

            foreach (var harvest in HarvestInfoBuilder.AllHarvests)
            {
                IconInitializers.HarvestIcons(harvest);
            }

            ModEntry.Instance.Monitor.Log($"Gift Discovery Initialized",
                LogHelper.DebugOrTrace
            );
        }

    }
}
