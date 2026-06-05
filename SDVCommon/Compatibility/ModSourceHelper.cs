using SDVCommon.GameData;
using SDVCommon.Helpers;
using SDVData;
using StardewModdingAPI;
using StardewValley;
using System.ComponentModel.Design;

namespace SDVCommon.Compatibility
{
    public static class ModSourceHelper
    {

        private static IModHelper _helper = null!;

        public static void Initialize(IModHelper helper)
        {
            _helper = helper;
        }

        public static string GetModSource(string itemId)
        {
            string prefix = IdHelper.GetModPrefix(itemId);

            if (DataOverrides.ModSource.TryGetValue(prefix, out string? overrideName)
                && overrideName is not null)
            {
                return overrideName;
            }

            string modName = _helper.ModRegistry.Get(prefix)?.Manifest.Name
                   ?? "Modded Item";

            //Only add when debugging or adding overrides
            //SDVCommonLog.Log(
            //    $"Modname {itemId} -> {prefix} -> {modName}",
            //    LogHelper.DebugOrTrace);

            return modName;
        }

    }

}
