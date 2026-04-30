using SDVCommon.Compatibility;
using StardewModdingAPI;

namespace PlantingDay.Compatibility
{
    internal class APIManager
    {
        public static void LoadApis(IModHelper helper)
        {
            CustomBushCompat.Api =
                helper.ModRegistry.GetApi<ICustomBushApi>("furyx639.CustomBush");
        }
    }
}
