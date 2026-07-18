using StardewModdingAPI;

namespace SDVCommon
{
    public static class SDVCommonServices
    {
        public static IModHelper Helper { get; internal set; } = null!;
        public static IMonitor Monitor { get; internal set; } = null!;

        public static void Initialize(IModHelper helper, IMonitor monitor)
        {
            Helper = helper;
            Monitor = monitor;
        }
    }
}
