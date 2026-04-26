using SDVCommon;
using SDVCommon.GameData;
using SDVCommon.Helpers;
using SDVCommon.Icons;
using SDVCommon.Services;
using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HarvestHelper.Helpers
{
    public static class Initializer
    {
        public static void InitializeAll(IModHelper helper)
        {
            TooltipIcons.Initialize();
            GameObjectInfoHelper.BuildHarvestToSeedMap(); //must be before harvestinfobuilder
            HarvestInfoBuilder.Initialize();
            GiftKnowledgeService.Initialize(helper);
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

            ModEntry.Instance.Monitor.Log($"[{DateTime.Now:HH:mm:ss}]  Harvest Database Initialized",
                LogLevel.Alert);


        }

    }
}
