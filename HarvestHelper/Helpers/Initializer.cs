using SDVCommon;
using SDVCommon.GameData;
using SDVCommon.Helpers;
using SDVCommon.Icons;
using SDVCommon.Models.Builders;
using StardewModdingAPI;

namespace HarvestHelper.Helpers
{
    public static class Initializer
    {
        public static void InitializeAll()
        {
            ModEntry.Instance.Monitor.Log($"[{DateTime.Now:HH:mm:ss}]", LogLevel.Warn);

            TooltipIcons.Initialize();
            GameDataHelper.BuildHarvestToSeedMap(); //must be before harvestinfobuilder
            HarvestInfoBuilder.Initialize();
            //GiftKnowledgeService.Initialize(helper); moved to modentry with harmony patch
            CookingInfoBuilder.BuildAll();

            foreach (var harvest in HarvestInfoBuilder.AllHarvests)
            {
                IconInitializers.HarvestIcons(harvest);
            }

            foreach (var recipe in CookingInfoBuilder.AllRecipes)
            {
                IconInitializers.CookingIcons(recipe);
            }

            CacheForTesting.DumpHarvestInfoToJson();

            ModEntry.Instance.Monitor.Log($"Harvest Database Initialized",
                LogHelper.DebugOrTrace
            );
            ModEntry.Instance.Monitor.Log($"[{DateTime.Now:HH:mm:ss}]", LogLevel.Warn);

        }

    }
}
