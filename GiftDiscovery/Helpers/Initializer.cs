using GiftDiscovery.Compatibility;
using GiftDiscovery.GameData.Static;
using GiftDiscovery.ModData;
using GiftDiscovery.Services;
using GiftDiscovery.Tooltip;
using SDVCommon;
using SDVCommon.Compatibility;
using SDVCommon.Icons;
using SDVCommon.Services;
using StardewModdingAPI;

namespace GiftDiscovery.Helpers
{
    internal static class Initializer
    {
        internal static void InitializeAll(IModHelper helper)
        {
            string timer = "Gift Discovery Initialize";
            LogLevel logLevel = LogHelper.InfoOrTrace;

            SDVCommonServices.PerfBegin(timer);

            //Compatibility
            APIManager.LoadApis(helper);

            //Pass down helper
            ModSource.Initialize(helper);

            //Gamedata static
            NPCGiftStatus.Initialize(); //updated
            GiftableObjectList.Initialize(); //updated

            //Moddata
            TasteMap.Initialize(); //updated
            UniversalLoveList.Initialize(); //updated

            //Services
            TasteLearning.Initialize(helper);

            SDVCommonServices.PerfEnd(timer, 0, logLevel);
        }

        internal static void ResetAll()
        {
            //Gamedata static
            GiftableObjectList.Reset();
            NPCGiftStatus.Reset();

            //Moddata
            TasteMap.Reset();
            UniversalLoveList.Reset();
            
            //Services
            TasteLearning.Reset();

            //Tooltips (cached)
            GiftTooltipBuilder.Reset();
            NPCGiftTooltipBuilder.Reset();

            //Common
            IconRegistry.Reset();

        }
    }
}
