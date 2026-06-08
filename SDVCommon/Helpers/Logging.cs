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

    public static class LogHelper
    {
        public static LogLevel DebugOrTrace =>
#if DEBUG
            LogLevel.Info;
#else
            LogLevel.Trace;
#endif


        public static LogLevel DebugWarnOrTrace =>
#if DEBUG
            LogLevel.Warn;
#else
            LogLevel.Trace;
#endif

        #if DEBUG

        //Debug Only
        public static LogLevel DebugAlert =>
        LogLevel.Alert;

        public static LogLevel DebugWarn =>
            LogLevel.Warn;

        public static LogLevel DebugInfo =>
            LogLevel.Info;

        public static LogLevel DebugDebug =>
            LogLevel.Debug;
#endif




    }
}
