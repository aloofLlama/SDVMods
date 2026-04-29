using HarvestHelper.Helpers;
using Microsoft.Xna.Framework;
using SDVCommon.Helpers;
using SDVCommon.Icons;
using SDVCommon.Models.Wrappers;
using SDVCommon.Services;
using SDVCommon.Tooltip;
using StardewModdingAPI;


namespace HarvestHelper.TooltipSections
{
    public static class GiftLovesSection
    {
        public static List<TooltipElement> Build(HarvestInfo harvest, StardewValley.Object obj)
        {
            var list = new List<TooltipElement>();

            var known = GiftHelper.GetKnownLovedBy(obj)
                .Select(npc => npc.displayName)
                .ToList();

            var unknown = GiftHelper.GetUnknownLovedBy(obj).Count();

            // Build collapsible segments (known)
            var collapsible = known
                .Select(name => new InlineSegment
                {
                    Text = name,
                    
                })
                .ToList();

            // Build always-append segments (unknown)
            var end = new List<InlineSegment>();
            if (unknown > 0)
            {
                end.Add(new InlineSegment
                {
                    Text = string.Format(
                        ModEntry.ModHelper.Translation.Get(TooltipKeys.Qty_Unknown),
                        unknown
                    ),
                    TextColor = TooltipColors.Muted
                });
            }

            // Wrap them: 4 wide, 2 rows
            var wrapped = TooltipBuildHelper.BuildWrappedSegmentBlock(
                startSegments: new List<InlineSegment>(),   // none for this section
                collapsibleSegments: collapsible,
                endSegments: end,
                wrapSize: 4,
                maxRows: 2
            );

            // Add tooltip element with heart icon
            list.Add(new TooltipElement
            {
                Icon = TooltipIcons.Get(IconKey.Heart),
                InlineSegments = wrapped
            });

            return list;
        }
    }
}

