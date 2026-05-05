using PlantingDay.Helpers;
using StardewValley;
using Microsoft.Xna.Framework;
using SDVData;
using SDVCommon.Helpers;
using SDVCommon.Tooltip;
using PlantingDay.Models.Wrappers;

namespace PlantingDay.ToolTip_Sections
{
    public static class SeasonSection
    {
        // Display the relevant seasons and highlight the current season

        public static TooltipElement? Build(PlantInfo plant)
        {
            if (plant.Data.Seasons.Count == 0)
                return null;

            // SPECIAL CASE: Indoor crops → show only one segment styled like current season
            if (plant.Data.Location == Location.Indoor)
            {
                // Convert Game1.currentSeason ("spring") → SeasonId.Spring
                SeasonId current = Enum.Parse<SeasonId>(Game1.currentSeason, true);

                var (color, bold) = Style(current);

                return new TooltipElement
                {
                    Text = ModEntry.ModHelper.Translation.Get(TooltipKeys.PlantIndoor),
                    TextColor = color,
                    Bold = bold
                };
            }

            // NORMAL CASE: Seasonal crops
            var segments = TooltipBuildHelper.BuildInlineSegmentswithSeparators(
                plant.Data.Seasons,
                season =>
                {
                    var (color, bold) = Style(season);

                    return new[]
                    {
                        new InlineSegment
                        {
                            Text = SeasonHelper.Translate(season),
                            TextColor = color,
                            Bold = bold
                        }
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
