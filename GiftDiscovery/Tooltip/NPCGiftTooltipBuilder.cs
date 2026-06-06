using GiftDiscovery.GameData;
using GiftDiscovery.Services;
using GiftDiscovery.Tooltip.NPCSections;
using Microsoft.Xna.Framework.Graphics;
using SDVCommon;
using SDVCommon.Helpers;
using SDVCommon.Helpers.Tooltip;
using SDVCommon.Models.Tooltip;
using SDVCommon.Rendering;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using System.Diagnostics;

namespace GiftDiscovery.Tooltip
{
    public static class NPCGiftTooltipBuilder
    {
        private static bool _isInitialized;

        private static List<TooltipElement>? _cachedTooltip;
        private static NPC? _cachedNPC;
        private static int _cachedRange;
        private static int _cachedConfigHash;
        private static bool _cachedMenuChanged;
        private static int _cachedToggleVersion;
        private static int _cachedGiftVersion;

        public static void Initialize()
        {
            if (_isInitialized)
                return;

            Reset();
            _isInitialized = true;
        }

        public static void Reset()
        {
            _cachedTooltip = null;
            _cachedNPC = null;
            _cachedRange = 0;
            _cachedMenuChanged = false;
            _cachedToggleVersion = -1;
            _cachedGiftVersion = -1;
        }

        public static void DrawTooltip(SpriteBatch b, NPC npc)
        {
            // Social Page
            if (Game1.activeClickableMenu is GameMenu gm && gm.currentTab == GameMenu.socialTab)
            {
                var elements = GetTooltip(npc);
                if (elements != null)
                    TooltipRenderer.DrawBottomRight(b, elements);
                return;
            }

            // World hover
            int range = ModEntry.ModConfig.NearbyRangeTilesNPCTooltip;
            NPC? nearest = GiftableNPC.GetClosestNearbyNPC(range);

            if (nearest is null)
            {
                _cachedNPC = null;
                return;
            }

            var elements2 = GetTooltip(nearest);
            if (elements2 != null)
                TooltipRenderer.DrawBottomRight(b, elements2);
        }

        public static List<TooltipElement>? GetTooltip(NPC npc)
        {
            int configHash = ModEntry.ModConfig.GetHashCode();
            bool menuChanged = ModEntry.MenuStateChanged;
            int toggleVersion = ModEntry.ToggleVersion;
            int giftVersion = GiftKnowledgeService.GiftVersion;

            int range = ModEntry.ModConfig.NearbyRangeTilesNPCTooltip;

            bool needsRebuild =
                _cachedTooltip == null ||
                _cachedNPC != npc ||
                _cachedRange != range ||
                configHash != _cachedConfigHash ||
                menuChanged != _cachedMenuChanged ||
                toggleVersion != _cachedToggleVersion ||
                giftVersion != _cachedGiftVersion;


            if (!needsRebuild)
                return _cachedTooltip;

            // Rebuild
            _cachedTooltip = BuildTooltip(npc);
            TooltipRenderer.InvalidateSize(_cachedTooltip);
            _cachedNPC = npc;
            _cachedRange = range;
            _cachedConfigHash = configHash;
            _cachedMenuChanged = menuChanged;
            _cachedToggleVersion = toggleVersion;
            _cachedGiftVersion = giftVersion;

            return _cachedTooltip;
        }

        private static List<TooltipElement> BuildTooltip(NPC npc)
        {
            var list = new List<TooltipElement>();
            int wrapSize = ModEntry.ModConfig.WrapSizeNPC;
            int maxRows = ModEntry.ModConfig.MaxRowsNPC;

            TooltipBuildHelper.AddIfNotNull(list, NPCHeader.Build(npc));
            TooltipBuildHelper.AddSectionWithSeparator(list, () => NPCTasteSegments.Build(npc));
            TooltipBuildHelper.AddSectionWithSeparator(list, () => NPCLocation.Build(npc));

            return list;
        }
    }
}
