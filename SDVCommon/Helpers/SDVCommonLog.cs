using StardewModdingAPI;

namespace SDVCommon.Helpers
{
    public static class SDVCommonLog
    {
        public static IMonitor? Monitor { get; private set; }

        public static void Initialize(IMonitor monitor)
        {
            Monitor = monitor;
        }

        public static void Log(string message, LogLevel level = LogLevel.Debug)
        {
            Monitor?.Log(message, level);
        }
    }
}
