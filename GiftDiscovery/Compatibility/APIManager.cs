using SDVCommon.Compatibility;
using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GiftDiscovery.Compatibility
{
    internal class APIManager
    {
        public static void LoadApis(IModHelper helper)
        {
            BetterCraftingCompat.Api =
                helper.ModRegistry.GetApi<IBetterCrafting>("leclair.bettercrafting");
        }
    }
}
