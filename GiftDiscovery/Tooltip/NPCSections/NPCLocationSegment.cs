using SDVCommon.Helpers.Tooltip;
using SDVCommon.Models.Tooltip;
using GiftDiscovery.GameData;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GiftDiscovery.Tooltip.NPCSections
{
    public class NPCLocationSegment
    {
        public static List<TooltipElement> Build(NPC npc)
        {
            // Only show location in menus, not in HUD proximity tooltips
            // Only show when checked in GMCM
            if (!ModEntry.IsInMenuTooltip || !ModEntry.ModConfig.ShowLocation)
                return new List<TooltipElement>();

            string name =NPCLocation.GetNPCLocation(npc);

            var segments = new List<InlineSegment>
            {
                //new InlineSegment
                //{
                //    Text = "Location: ",
                //    Bold = true
                //},
                new InlineSegment
                {
                    Text = name,
                }
            };

            return new List<TooltipElement>
                {
                    new TooltipElement
                    {
                        InlineSegments = segments
                    }
                };
            }

    }
}
