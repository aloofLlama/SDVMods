using GiftDiscovery.GameData.Dynamic;
using SDVCommon.Models.Tooltip;
using StardewValley;

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

            string name = NPCLocation.GetNPCLocation(npc);

            return new List<TooltipElement> {
                new() {
                    InlineSegments = new List<InlineSegment> {
                        new() { Text = name }
                    }
                }
            };

        }
    }
}
