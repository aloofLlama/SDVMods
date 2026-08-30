using GiftDiscovery.Compatibility;
using GiftDiscovery.GameData.Static;
using GiftDiscovery.ModData;
using GiftDiscovery.Models;
using GiftDiscovery.Services;
using GiftDiscovery.Tooltip;
using SDVCommon;
using SDVCommon.Compatibility;
using SDVCommon.Icons;
using SDVCommon.Models.Builders;
using SDVCommon.Services;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Characters;
using System.Diagnostics;

namespace GiftDiscovery.Helpers
{
    public static class Initializer
    {

        public static void InitializeAll(IModHelper helper)
        {
            string timer = "Gift Discovery Initialize";
            LogLevel logLevel = LogHelper.InfoOrTrace;

            SDVCommonServices.PerfBegin(timer);

            APIManager.LoadApis(helper);
            ModSourceHelper.Initialize(helper);

            //Gamedata static
            GiftableObjectList.Initialize(); //updated
            NPCGiftStatus.Initialize(); //updated

            //Moddata
            TasteMap.Initialize(); //updated
            UniversalLoveList.Initialize(); //updated

            //
            TasteLearning.Initialize(helper);

            GiftTooltipBuilder.Initialize();
            NPCGiftTooltipBuilder.Initialize();

            SDVCommonServices.PerfEnd(timer, 0, logLevel);

        }

        public static void ResetAll()
        {
            GiftableObjectList.Reset();
            NPCGiftStatus.Reset();

            TasteLearning.Reset();
            TasteMap.Reset();
            UniversalLoveList.Reset();

            GiftTooltipBuilder.Reset();
            NPCGiftTooltipBuilder.Reset();

        }
    }
}
