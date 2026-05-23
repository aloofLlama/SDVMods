using GiftDiscovery.GameData;
using GiftDiscovery.Helpers;
using GiftDiscovery.Models;
using GiftDiscovery.Services;
using GiftDiscovery.Models.Builders;
using GiftDiscovery.Tooltip.NPCSections;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SDVCommon;
using SDVCommon.GameData;
using SDVCommon.Helpers;
using SDVCommon.Helpers.Tooltip;
using SDVCommon.Icons;
using SDVCommon.Models.Builders;
using SDVCommon.Models.Tooltip;
using SDVCommon.Models.Wrappers;
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
            //var nearbyNPCSet = GiftableNPC.GetNearbyNPCNames(range);

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

            // Build canonical taste map ONCE per NPC
            //GiftKnowledgeService.GetCanonicalTasteMap(npc);

            // Rebuild
            _cachedTooltip = BuildTooltip(npc);
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












//        private static List<TooltipElement> BuildTooltip(NPC npc)
//        {

//            var list = new List<TooltipElement>();
//            int wrapSize = ModEntry.ModConfig.WrapSizeNPC;
//            int maxRows = ModEntry.ModConfig.MaxRowsNPC;


//            var portrait = NPCGameData.GetPortraitIcon(npc);
//            portrait = new Icon(portrait.Texture, portrait.Source, portrait.Size, scale: 0.6f);

//            list.Add(new TooltipElement
//            {
//                Icon = portrait,
//                Text = npc.displayName,
//                TextColor = DisplayHelper.GetNPCNameColor(npc)

//            });

//            TasteSourceMode mode = ModEntry.ModConfig.TasteSourceMode;

//            AddTaste(list, npc, "Loves", GiftTaste.Love, mode, wrapSize, maxRows);
//            AddTaste(list, npc, "Likes", GiftTaste.Like, mode, wrapSize, maxRows);

//            if (!LearnedGiftsHelper.HasDiscoveredAllLovesLikesForNPC(npc, mode))
//            {
//                AddTaste(list, npc, "Neutral", GiftTaste.Neutral, mode, wrapSize, maxRows);
//                AddTaste(list, npc, "Dislikes", GiftTaste.Dislike, mode, wrapSize, maxRows);
//                AddTaste(list, npc, "Hates", GiftTaste.Hate, mode, wrapSize, maxRows);
//            }

//            return list;
//        }

//        private static void AddTaste(
//            List<TooltipElement> list,
//            NPC npc,
//            string label,
//            GiftTaste taste,
//            TasteSourceMode mode,
//            int wrapSize,
//            int maxRows)
//        {
//            var knownIds = LearnedGiftsHelper.GetKnownGiftsForNPC(npc, taste, mode);

//            var filteredIds = knownIds
//                .Where(id => GiftableObjectList.GiftableIds.Contains(id));

//            var items = filteredIds
//                .Select(id => HarvestInfoBuilder.LookupFromKey(IdHelper.CanonicalItemId(id)))
//                .Where(info => info is not null)
//                .Cast<HarvestInfo>()
//                .ToList();

//            //sort so backpack items appear first, then alphabetically by name
//            items = items
//            .OrderByDescending(i => Inventory.IsInBackpack(i.Data.HarvestId))
//            .ThenBy(i => i.Runtime.DisplayName)
//            .ToList();

//            // Unknown count
//            int unknownCount = LearnedGiftsHelper
//                .GetUnknownGiftsForNPC(npc, taste, mode)
//                .Count(); ;

//            // Skip if nothing to show
//            if (items.Count == 0 && unknownCount == 0)
//                return;

//            TooltipBuildHelper.AddSectionWithSeparator(list, () =>
//                BuildNPCTasteSection(label, items, unknownCount, wrapSize, maxRows)
//            );
//        }

//        private static List<TooltipElement> BuildNPCTasteSection(
//            string label,
//            List<HarvestInfo> items,
//            int unknownCount,
//            int wrapSize,
//            int maxRows)
//        {
//            var segments = new List<InlineSegment>();

//            foreach (var obj in items)
//                segments.Add(BuildItemSegment(obj));

//            // Unknown count
//            if (unknownCount > 0)
//            {
//                segments.Add(new InlineSegment
//                {
//                    Text = $"({unknownCount})",
//                    TextColor = TooltipColors.Muted
//                });
//            }

//            // Label segment
//            var labelSegment = new InlineSegment
//            {
//                Text = label + ": ",
//                Bold = true,
//                TextColor = TooltipColors.Normal
//            };

//            var wrapped = TooltipBuildHelper.BuildWrappedSegmentBlock(
//                startSegments: new List<InlineSegment> { labelSegment },
//                collapsibleSegments: segments,
//                endSegments: new List<InlineSegment>(),
//                wrapSize: wrapSize,
//                maxRows: maxRows,
//                useCommas: false
//            );

//            return new List<TooltipElement>
//                {
//                    new TooltipElement { InlineSegments = wrapped }
//                };
//        }

//        private static InlineSegment BuildItemSegment(HarvestInfo harvest)
//        {
//            bool inBackpack = Inventory.IsInBackpack(harvest.Data.HarvestId);

//            string name = inBackpack ? harvest.Runtime.DisplayName : "";

//            // Add comma *only* if we have a display name
//            if (!string.IsNullOrEmpty(name))
//                name += ",";

//            return new InlineSegment
//            {
//                Icon = harvest.Runtime.HarvestIcon,
//                Text = name,
//                TextColor = TooltipColors.Normal,
//                Underline = false
//            };
//        }



//    }
//}
