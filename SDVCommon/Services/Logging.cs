using StardewModdingAPI;

namespace SDVCommon.Services
{
    /* LOG TEMPLATES
     * 
     * Debug which changes to trace upon release
        SDVCommonLog.Log($"", LogHelper.AlertOrTrace);

        SDVCommonLog.Log($"", LogHelper.WarnOrTrace);

        SDVCommonLog.Log($"", LogHelper.InfoOrTrace);

        SDVCommonLog.Log($"", LogHelper.DebugOrTrace);


     * Same for Debug and Release

        // yellow
        SDVCommonLog.Log($"", LogHelper.Warn);

        // white
        SDVCommonLog.Log($"", LogHelper.Info);

        // grey
        SDVCommonLog.Log($"", LogHelper.Debug);

    * Helpful logging functions
    
        //Displays all the ObjectInfo data for a qId
        GameObject.DumpObjectInfo("(O)74");

    */

    public static class SDVCommonLog
    {
        public static void Log(string message)
        {
            SDVCommonServices.Monitor?.Log(message, LogHelper.DebugOrTrace);
        }

        public static void Log(string message, LogLevel level)
        {
            SDVCommonServices.Monitor?.Log(message, level);
        }

        // Same as Log except it is easier to find and remove when finished.
        public static void TempLog(string message, LogLevel level)
        {
            SDVCommonServices.Monitor?.Log(message, level);
        }


        public static void TimestampLog(string message, LogLevel level)
        {
            string ts = DateTime.Now.ToString("HH:mm:ss.fff"); // includes milliseconds
            SDVCommonServices.Monitor?.Log($"[{ts}] {message}", level);
        }

    }

    public static class LogHelper
    {
        // Same for Debug and Release
        public static LogLevel Info =>
            LogLevel.Info;

        public static LogLevel Debug =>
            LogLevel.Debug;

        public static LogLevel Warn =>
             LogLevel.Warn;


        // Trace on Release
        public static LogLevel AlertOrTrace =>
#if DEBUG
            LogLevel.Alert;
#else
    LogLevel.Trace;
#endif

        public static LogLevel WarnOrTrace =>
#if DEBUG
            LogLevel.Warn;
#else
    LogLevel.Trace;
#endif
        public static LogLevel InfoOrTrace =>
#if DEBUG
            LogLevel.Info;
#else
    LogLevel.Trace;
#endif


        public static LogLevel DebugOrTrace =>
#if DEBUG
            LogLevel.Debug;
#else
            LogLevel.Trace;
#endif


    }
}
