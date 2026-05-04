using GiftDiscovery.Services;
using SDVCommon.Icons;
using StardewModdingAPI;
using static SDVCommon.TooltipRenderer;

namespace GiftDiscovery.Helpers
{
    public static class Initializer
    {
        public static void InitializeAll(IModHelper helper)
        {
            TooltipIcons.Initialize();
            GiftKnowledgeService.InitializeLocal(helper);

            ModEntry.Instance.Monitor.Log($"[{DateTime.Now:HH:mm:ss}]  Gift Discovery Initialized",
                LogLevel.Alert);
        }

    }
}
