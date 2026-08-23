using PlantingDay.Compatibility;
using SDVCommon.Compatibility;
using SDVCommon.Helpers;
using SDVCommon.Services;
using SDVCommon.GameData.Dictionaries;
using SDVCommon.Models.Builders;
using StardewModdingAPI;

namespace PlantingDay.Helpers
{
    internal class Initializer
    {
        public static void InitializeAll(IModHelper helper)
        {
            //TooltipIcons.Initialize();
            APIManager.LoadApis(helper);
            ModSourceHelper.Initialize(helper);

            MonsterDropBuilder.Initialize();
            PlantInfoBuilder.Initialize();
            Harvest.Initialize();

            //int cnt = 0;
            //foreach (var plant in PlantInfoBuilder.AllPlants)
            //{
            //    // seed, trade currency icons
            //    PlantIconInitializer.InitializeIcons(plant);
            //    cnt++;
            //}

            //int cnt2 = 0;
            //foreach (var harvest in Harvest.AllHarvests)
            //{
            //    IconInitializers.HarvestIcons(harvest);
            //    cnt2++;
            //}

#if DEBUG
            //CacheForTesting.DumpPlantInfoToJson();
#endif

            SDVCommonLog.Log($"Plant Database Initialized");
            //SDVCommonLog.Log($"Seed icons: {cnt} | Harvest icons: {cnt2}",
            //    LogHelper.DebugOrTrace);

        }

        public static void ResetAll()
        {
            PlantInfoBuilder.Reset();
            Harvest.Reset();

        }

    }
}
