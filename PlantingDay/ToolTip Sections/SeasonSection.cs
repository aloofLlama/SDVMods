using PlantingDay.Helpers;
using PlantingDay.Models;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PlantingDay.ToolTip_Sections
{
    public static class SeasonSection
    {
        // Display the relevant seasons and highlight the current season

        public static TooltipElement? Build(PlantInfo plant)
        {
            if (plant.Data.Seasons.Count == 0)
                return null;

            var segments = TooltipRenderer.BuildInlineSegments(
                plant.Data.Seasons,
                season =>
                {
                    var (color, bold) = Style(season);

                    return new[] { new InlineSegment
                    {
                        Text = SeasonHelper.Translate(season),
                        Color = color,
                        Bold = bold }
                    };
                }
            );
            return new TooltipElement { InlineSegments = segments };

        }

        public static (Color color, bool bold) Style(SeasonId season)
        {
            bool isCurrent = season.ToString().Equals(Game1.currentSeason, StringComparison.OrdinalIgnoreCase);

            Color color = season switch
            {
                SeasonId.Spring => TooltipColors.SpringColor,
                SeasonId.Summer => TooltipColors.SummerColor,
                SeasonId.Fall => TooltipColors.FallColor,
                SeasonId.Winter => TooltipColors.WinterColor,
                _ => TooltipColors.Normal
            };

            return isCurrent
                ? (color, true)
                : (TooltipColors.Normal, false);
        }



    }
}
