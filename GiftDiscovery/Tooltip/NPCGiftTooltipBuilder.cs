using GiftDiscovery.Tooltip.NPCSections;
using Microsoft.Xna.Framework.Graphics;
using SDVCommon;
using SDVCommon.Helpers.Tooltip;
using SDVCommon.Models.Tooltip;
using SDVCommon.Rendering;
using StardewValley;

namespace GiftDiscovery.Tooltip
{
    public static class NPCGiftTooltipBuilder
    {
        internal static bool TooltipInvalidated { get; set; }

        private static List<TooltipElement>? _tooltip;
        private static NPC? _cachedNPC;

        //------------------------------------------------
        // Data lifecycle methods
        //------------------------------------------------

        public static void Reset()
        {
            _tooltip = null;
            _cachedNPC = null;
            TooltipInvalidated = false;
        }
        // Reset or update the relevent cached data
        internal static void RefreshCache(NPC npc)
        {
            if (_tooltip is not null)
                TooltipRenderer.InvalidateSize(_tooltip);
            _cachedNPC = npc;
            TooltipInvalidated = false;
        }

        //------------------------------------------------
        // Draw, Get, and Build methods
        //------------------------------------------------

        public static void DrawTooltip(SpriteBatch b, NPC npc)
        {
            var elements = GetTooltip(npc);
            if (elements != null)
                TooltipRenderer.DrawBottomRight(b, elements);

            return;
        }

        public static List<TooltipElement>? GetTooltip(NPC npc)
        {
            // Menu / config changes set TooltipInvalidated at their respective events.
            // Item or nearby NPC changes are checked here
            if (npc != _cachedNPC ||
                _tooltip == null)
            {
                TooltipInvalidated = true;
            }

            if (!TooltipInvalidated)
                return _tooltip;

            // Rebuild
            _tooltip = BuildTooltip(npc);
            RefreshCache(npc);

            return _tooltip;
        }

        private static List<TooltipElement> BuildTooltip(NPC npc)
        {
            var list = new List<TooltipElement>();
            int wrapSize = ModEntry.ModConfig.WrapSizeNPC;
            int maxRows = ModEntry.ModConfig.MaxRowsNPC;

            string timer = "Build NPC Tooltip";  // Logs {name} took {ms} ms
            SDVCommonServices.PerfBegin(timer);

            //* Keep for debug as it marks the start of a new tooltip
            //SDVCommonServices.PerfPing(timer, $" {npc.displayName}", 0, LogHelper.AlertOrTrace); // name it for what just finished

            TooltipBuildHelper.AddIfNotNull(list, NPCHeader.Build(npc));
            TooltipBuildHelper.AddSectionWithSeparator(list, () => NPCTasteSegments.Build(npc));
            TooltipBuildHelper.AddSectionWithSeparator(list, () => NPCLocationSegment.Build(npc));

            SDVCommonServices.PerfEnd(timer, npc.displayName, 10);

            return list;
        }
    }
}
