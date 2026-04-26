using SDVCommon.Models.Wrappers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SDVCommon.Icons
{
    internal static class IconInitializers
    {
        public static void HarvestIcons(HarvestInfo harvest)
        {
            harvest.Runtime.HarvestIcon = IconRegistry.GetIcon($"harvest:{harvest.Data.HarvestId}");
        }

        public static void CookingIcons(CookingInfo cooking)
        {
            cooking.Runtime.DishIcon =
                IconRegistry.GetIcon($"cooking:{cooking.Data.OutputId}");
        }
    }
}
