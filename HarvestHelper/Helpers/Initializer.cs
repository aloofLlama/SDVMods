using SDVCommon;
using SDVCommon.Compatibility;
using SDVCommon.GameData;
using SDVCommon.Helpers;
using SDVCommon.Services;
using SDVCommon.Models.Builders;
using SDVCommon.GameData.Dictionaries;
using HarvestHelper.Services;
using StardewModdingAPI;

namespace HarvestHelper.Helpers
{
    public static class Initializer
    {
        public static void InitializeAll(IModHelper helper)
        {
            //TooltipIcons.Initialize();
            ModSourceHelper.Initialize(helper);
            SeedHarvestMap.Initialize();

            Harvest.Initialize();
            CookingInfoBuilder.Initialize();
            ArtisanInfoBuilder.Initialize();

            TooltipBuilder.Initialize();

#if DEBUG
            // TODO: disabled during the ID fixing, needs fixed
            //CacheForTesting.DumpHarvestInfoToJson();
#endif
            SDVCommonLog.Log($"Harvest Database Initialized");


        }
        public static void ResetAll()
        {
            SeedHarvestMap.Reset();
            Harvest.Reset();
            CookingInfoBuilder.Reset();
            ArtisanInfoBuilder.Reset();

            TooltipBuilder.Reset();
        }
    }
}
