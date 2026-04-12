using PlantingDay.Helpers.Icons;
using PlantingDay.Models.Wrappers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SDVCommon.Icons
{
    internal static class HarvestIconInitializer
    {
        public static void InitializeIcons(HarvestInfo harvest)
        {
            // Seed + harvest icons

            harvest.Runtime.HarvestIcon = IconRegistry.GetIcon($"harvest:{harvest.Data.HarvestId}");

        }
    }
}
