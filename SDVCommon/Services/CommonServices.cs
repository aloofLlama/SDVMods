using SDVCommon.Services;
using StardewModdingAPI;
using System.Diagnostics;
using static System.Collections.Specialized.BitVector32;

namespace SDVCommon
{

    /* TIMER TEMPLATE
    string timer = "name";
    LogLevel logLevel = LogHelper.DebugOrTrace;
    SDVCommonServices.PerfBegin(timer);

    SDVCommonServices.PerfEnd(timer, 0, logLevel);

    SDVCommonServices.PerfPing(timer, "pingName", 0, logLevel); // name it for what just finished

 */

    public static class SDVCommonServices
    {
        public static IModHelper Helper { get; internal set; } = null!;
        public static IMonitor Monitor { get; internal set; } = null!;

        public static void Initialize(IModHelper helper, IMonitor monitor)
        {
            Helper = helper;
            Monitor = monitor;
        }



        // Named performance timers
        private class PerfTimer
        {
            public Stopwatch Watch = new Stopwatch();
            public long LastPingMs = 0;
        }

        private static readonly Dictionary<string, PerfTimer> PerfTimers = new();

        // Start or restart a named timer
        public static void PerfBegin(string name)
        {
            if (!PerfTimers.TryGetValue(name, out var timer))
                PerfTimers[name] = timer = new PerfTimer();

            timer.Watch.Restart();
            timer.LastPingMs = 0;
        }

        // Use between the Begin and End to output the elapsed time since start or the previous ping
        // Use case is starting a timer and setting pings throughout to narrow down which section is using the time
        // only display the ping text if value is above the threshold in ms
        public static long PerfPing(string name, string pingName, int thresholdMs)
        {
            var timer = PerfTimers[name];
            long total = timer.Watch.ElapsedMilliseconds;
            long section = total - timer.LastPingMs;

            if (section >= thresholdMs)
                SDVCommonLog.Log($"  {section} ms | {name} | {pingName}");

            timer.LastPingMs = total;
            return section;
        }

        public static long PerfPing(string name, string pingName, int thresholdMs, LogLevel level)
        {
            var timer = PerfTimers[name];
            long total = timer.Watch.ElapsedMilliseconds;
            long section = total - timer.LastPingMs;

            if (section >= thresholdMs)
                SDVCommonLog.Log($"   {section} ms | {name} | {pingName}",
                    level);

            timer.LastPingMs = total;
            return section;
        }



        // End a named timer and log the result as debug or trace
        public static long PerfEnd(string name, int thresholdMs)
        {
            var timer = PerfTimers[name];
            long total = timer.Watch.ElapsedMilliseconds;

            if (total >= thresholdMs)
                SDVCommonLog.Log($"{total} ms | {name}");

            timer.Watch.Stop();
            return total;
        }

        // End a named timer and log the result with some extra text
        public static long PerfEnd(string name, string text, int thresholdMs)
        {
            var timer = PerfTimers[name];
            long total = timer.Watch.ElapsedMilliseconds;

            if (total >= thresholdMs)
                SDVCommonLog.Log($"{total} ms | {name} | {text}");

            timer.Watch.Stop();
            return total;
        }

        // End a named timer and log the result at the specified log level
        public static long PerfEnd(string name, int thresholdMs, LogLevel level)
        {
            var timer = PerfTimers[name];
            long total = timer.Watch.ElapsedMilliseconds;

            if (total >= thresholdMs)
                SDVCommonLog.Log($"{total} ms | {name}",
                level);

            timer.Watch.Stop();
            return total;
        }

        // End a named timer and log the result at the specified log level with some extra text
        public static long PerfEnd(string name, string text, int thresholdMs, LogLevel level)
        {
            var timer = PerfTimers[name];
            long total = timer.Watch.ElapsedMilliseconds;

            if (total >= thresholdMs)
                SDVCommonLog.Log($"{total} ms | {name} | {text}",
                level);

            timer.Watch.Stop();
            return total;
        }

        // Get the current time of the ping timer
        public static long GetPerfPingMs(string name)
        {
            var timer = PerfTimers[name];
            long total = timer.Watch.ElapsedMilliseconds;
            long section = total - timer.LastPingMs;

            return section;

        }
    }
}
