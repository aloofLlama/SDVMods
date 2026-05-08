using PlantingDay.Helpers.Icons;
using PlantingDay.Helpers.SeedSource;
using PlantingDay.Services;
using SDVCommon;
using PlantingDay.Compatibility;
using SDVCommon.Icons;
using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlantingDay.Helpers
{
    internal class Initializer
    {
        public static void InitializeAll(IModHelper helper)
        {
            TooltipIcons.Initialize();
            APIManager.LoadApis(helper);
            MonsterDropLoader.Initialize();
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

#if DEBUG
            ModEntry.Instance.Monitor.Log($"[{DateTime.Now:HH:mm:ss}] Plant Database Initialized",
                LogLevel.Warn);
#endif



        }


}
}
