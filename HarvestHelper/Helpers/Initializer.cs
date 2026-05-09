using SDVCommon;
using SDVCommon.GameData;
using SDVCommon.Helpers;
using SDVCommon.Icons;
using SDVCommon.Models.Builders;

namespace HarvestHelper.Helpers
{
    public static class Initializer
    {
        public static void InitializeAll()
        {
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

        }

    }
}
