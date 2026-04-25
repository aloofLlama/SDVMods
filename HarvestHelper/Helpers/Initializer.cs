using SDVCommon;
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
            HarvestInfoBuilder.Initialize();
            GiftKnowledgeService.Initialize(helper);
            //GiftDetection.Initialize(helper);

            foreach (var harvest in HarvestInfoBuilder.AllHarvests)
            {
                HarvestIconInitializer.InitializeIcons(harvest);
            }

            CacheForTesting.DumpHarvestInfoToJson();

            ModEntry.Instance.Monitor.Log($"[{DateTime.Now:HH:mm:ss}]  Harvest Database Initialized",
                LogLevel.Alert);


        }

    }
}
