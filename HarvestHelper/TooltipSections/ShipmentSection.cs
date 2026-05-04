using HarvestHelper.Helpers;
using Microsoft.Xna.Framework;
using SDVCommon.Helpers;
using SDVCommon.Icons;
using SDVCommon.Models.Wrappers;
using SDVCommon.Tooltip;
using StardewModdingAPI;
using StardewValley;
using System.Xml.Linq;


namespace HarvestHelper.TooltipSections
{
    public static class ShipmentSection
    {
        public static List<TooltipElement> Build(HarvestInfo harvest, StardewValley.Object obj)
        {
            var list = new List<TooltipElement>();
            string id = harvest.Data.HarvestId;

            bool needsOne = AchievementHelper.NeedsShippedOne(harvest);
            bool needsPoly = AchievementHelper.NeedsPolyCultureShipped(harvest);
            bool needsMono = AchievementHelper.NeedsMonoCultureShipped(harvest);

            if (!needsOne && !needsPoly && !needsMono)
                return list;

            //ModEntry.Instance.Monitor.Log($"TEXT: {harvest.Data.ShipMonoCulture}", LogLevel.Info);

            Game1.player.basicShipped.TryGetValue(id, out int count);

            // Build the text items
            var items = new List<string>();
            if (needsOne) items.Add($"{count}/1");
            if (needsPoly) items.Add($"{count}/15");
            if (needsMono) items.Add($"{count}/300");

            // Build inline segments with separators
            var segments = TooltipBuildHelper.BuildInlineSegmentswithSeparators(
                items,
                text => new[]
                {
                new InlineSegment { 
                     Text = text,
                     TextColor = TooltipColors.Perfection
                     }
                }
            );

            // Prepend the icon
            segments.Insert(0, new InlineSegment
            {
                Icon = TooltipIcons.Get(IconKey.ShipBin)
            });

            list.Add(new TooltipElement
            {
                InlineSegments = segments
            });

            return list;
        }
    }
}

