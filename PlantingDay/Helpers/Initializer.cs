using PlantingDay.Compatibility;
using SDVCommon.Helpers;
using SDVCommon.Helpers.Specific;
using SDVCommon.Helpers.Tooltip;
using SDVCommon.Icons;
using SDVCommon.Models.Wrappers;
using SDVCommon.Services;
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


            foreach (var plant in PlantInfoBuilder.AllPlants)
            {
                // seed, trade currency icons
                PlantIconInitializer.InitializeIcons(plant);

            }
            foreach (var harvest in HarvestInfoBuilder.AllHarvests)
            {
                IconInitializers.HarvestIcons(harvest);
            }

            CacheForTesting.DumpPlantInfoToJson();

            ModEntry.Instance.Monitor.Log(
                $"Plant Database Initialized",
                LogHelper.DebugOrTrace
            );

        }


    }
}
