using SDVCommon;
using SDVCommon.Compatibility;
using SDVCommon.GameData;
using SDVCommon.Helpers;
using SDVCommon.Icons;
using SDVCommon.Models.Builders;
using StardewModdingAPI;

namespace HarvestHelper.Helpers
{
    public static class Initializer
    {
        public static void InitializeAll(IModHelper helper)
        {
            TooltipIcons.Initialize();
            ModSourceHelper.Initialize(helper);
            GameDataHelper.BuildHarvestToSeedMap(); //must be before harvestinfobuilder
            HarvestInfoBuilder.Initialize();
            CookingInfoBuilder.BuildAll();

            int cnt = 0;
            foreach (var harvest in HarvestInfoBuilder.AllHarvests)
            {
                IconInitializers.HarvestIcons(harvest);
                cnt++;
            }

            int cnt2 = 0;
            foreach (var recipe in CookingInfoBuilder.AllRecipes)
            {
                IconInitializers.CookingIcons(recipe);
                cnt2++;
            }

#if DEBUG
            CacheForTesting.DumpHarvestInfoToJson();
#endif
            SDVCommonLog.Log($"Harvest Database Initialized",
                LogHelper.DebugOrTrace);
            SDVCommonLog.Log($"Harvest icons: {cnt} | Cooking icons: {cnt2}",
                LogHelper.DebugOrTrace);


        }

    }
}
