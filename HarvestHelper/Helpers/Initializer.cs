using SDVCommon;
using SDVCommon.Compatibility;
using SDVCommon.GameData;
using SDVCommon.Helpers;
using SDVCommon.Services;
using SDVCommon.Models.Builders;
using StardewModdingAPI;

namespace HarvestHelper.Helpers
{
    public static class Initializer
    {
        public static void InitializeAll(IModHelper helper)
        {
            //TooltipIcons.Initialize();
            ModSourceHelper.Initialize(helper);
            GameDataHelper.BuildHarvestToSeedMap(); //must be before harvestinfobuilder
            HarvestInfoBuilder.Initialize();
            CookingInfoBuilder.Initialize();
            ArtisanInfoBuilder.Initialize();

#if DEBUG
            CacheForTesting.DumpHarvestInfoToJson();
#endif
            SDVCommonLog.Log($"Harvest Database Initialized");


        }

    }
}
