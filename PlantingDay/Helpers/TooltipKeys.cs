using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlantingDay.Helpers
{
    public static class TooltipKeys
    {
        // Growth
        public const string DaysToProduce = "tooltip.days_to_produce";
        public const string ReadyOn = "tooltip.ready_on";
        public const string TooLate = "tooltip.too_late";
        public const string RegrowQty = "tooltip.regrow_qty";

        // Paddy Growth
        //public const string PaddyDaysToProduce = "tooltip.paddy_days_to_produce";
        //public const string PaddyReadyOn = "tooltip.paddy_ready_on";
        //public const string PaddyRegrowQty = "tooltip.paddy_too_late";

        // Tree growth
        public const string TreeReadyInFuture = "tooltip.tree_ready_in_future";


        // Requirements
        public const string RequiresTrellis = "tooltip.requires_trellis";

        // Seasons
        public const string Seasons = "tooltip.seasons";

        // Misc
        public const string HarvestWithScythe = "tooltip.scythe";
        public const string NoWatering = "tooltip.no_watering";

        //public const string MultiHarvest = "tooltip.multi_harvest";
        public const string MultiSprite = "tooltip.multi_sprite";

    }

}

