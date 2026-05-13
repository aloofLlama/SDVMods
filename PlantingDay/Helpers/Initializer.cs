using PlantingDay.Compatibility;
using SDVCommon.Helpers;
using SDVCommon.Icons;
using SDVCommon.Models.Builders;
using StardewModdingAPI;

namespace PlantingDay.Helpers
{
    internal class Initializer
    {
        public static void InitializeAll(IModHelper helper)
        {
            TooltipIcons.Initialize();
            APIManager.LoadApis(helper);
            MonsterDropBuilder.Initialize();
            PlantInfoBuilder.Initialize();
            HarvestInfoBuilder.Initialize();

            int cnt = 0;
            foreach (var plant in PlantInfoBuilder.AllPlants)
            {
                // seed, trade currency icons
                PlantIconInitializer.InitializeIcons(plant);
                cnt++;
            }

            int cnt2 = 0;
            foreach (var harvest in HarvestInfoBuilder.AllHarvests)
            {
                IconInitializers.HarvestIcons(harvest);
                cnt2++;
            }

#if DEBUG
            CacheForTesting.DumpPlantInfoToJson();
#endif

            SDVCommonLog.Log($"Plant Database Initialized",
                LogHelper.DebugOrTrace);
            SDVCommonLog.Log($"Seed icons: {cnt} | Harvest icons: {cnt2}",
                LogHelper.DebugOrTrace);

        }


    }
}
