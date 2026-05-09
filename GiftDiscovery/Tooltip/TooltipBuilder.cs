using GiftDiscovery;
using GiftDiscovery.Helpers;
using GiftDiscovery.Models;
using GiftDiscovery.Services;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SDVCommon;
using SDVCommon.Models.Tooltip;
using SDVCommon.Models.Builders;
using SDVCommon.Helpers.Tooltip;

namespace GiftDiscovery.Tooltip
{

    public static class TooltipBuilder
    {
        private static List<TooltipElement>? _cachedTooltip;
        private static string? _cachedItemId;
        private static int _cachedConfigHash;
        private static HashSet<string> _cachedNearbyNpcSet = new();
        private static bool _cachedMenuChanged;
        private static int _cachedToggleVersion;
        private static int _cachedGiftVersion;

        public static void DrawTooltip(SpriteBatch b, StardewValley.Object obj)
        {
            if (!obj.canBeGivenAsGift())
                return;

            if (!GiftHelper.GetKnownBy(obj, GiftTaste.Love, TasteSourceMode.All).Any() &&
                !GiftHelper.GetKnownBy(obj, GiftTaste.Like, TasteSourceMode.All).Any())
                return;

            var elements = GetTooltip(obj);

            if (elements is null || elements.Count == 0)
                return;

            TooltipRenderer.DrawBottomLeft(b, elements);
        }

        public static List<TooltipElement>? GetTooltip(StardewValley.Object obj)
        {
            string itemId = obj.ItemId;
            int configHash = ModEntry.ModConfig.GetHashCode();
            bool menuChanged = ModEntry.MenuStateChanged;
            int toggleVersion = ModEntry.ToggleVersion;
            int giftVersion = GiftKnowledgeService.GiftVersion;

            var nearbyNpcSet = DisplayHelper.GetNearbyNpcNames(ModEntry.ModConfig.NearbyRangeTiles);

            bool needsRebuild =
                _cachedTooltip == null ||
                itemId != _cachedItemId ||
                configHash != _cachedConfigHash ||
                menuChanged != _cachedMenuChanged ||
                !nearbyNpcSet.SetEquals(_cachedNearbyNpcSet) ||
                toggleVersion != _cachedToggleVersion ||
                giftVersion != _cachedGiftVersion;

            if (!needsRebuild)
                return _cachedTooltip;

            // Rebuild
            _cachedTooltip = BuildTooltip(obj);
            _cachedItemId = itemId;
            _cachedConfigHash = configHash;
            _cachedMenuChanged = menuChanged;
            _cachedNearbyNpcSet = nearbyNpcSet.ToHashSet();
            _cachedToggleVersion = toggleVersion;
            _cachedGiftVersion = giftVersion;

            return _cachedTooltip;
        }

        public static List<TooltipElement> BuildTooltip(
            StardewValley.Object obj)
        {
            int wrapSize = ModEntry.ModConfig.WrapSize;
            var list = new List<TooltipElement>();

            //Icon and display name
            var giftItem = HarvestInfoBuilder.LookupFromKey(obj.ItemId);

            if (giftItem != null)
            {
                list.Add(new TooltipElement
                {
                    Icon = giftItem.Runtime.HarvestIcon,
                    Text = obj.DisplayName
                });
            }

            var allGiftable = GiftHelper.GetAllGiftableNPCs()
                .Select(NpcGiftClassificationBuilder.Classify)
                .Where(c => c.IsGiftable)
                .ToList();

            var available = allGiftable.Where(c => c.IsAvailable).ToList();

            // ---------------------------------------------------------
            // Use the user selected mode
            // ---------------------------------------------------------
            TasteSourceMode mode = ModEntry.ModConfig.TasteSourceMode;

            List<NpcGiftClassification> pool =
                mode == TasteSourceMode.All ? allGiftable : available;

            // ---------------------------------------------------------
            // Taste grouping
            // ---------------------------------------------------------
            IEnumerable<NpcGiftClassification> Known(GiftTaste t) =>
                GiftHelper.GetKnownBy(obj, t, mode)
                    .Select(npc => NpcGiftClassificationBuilder.Classify(npc));

            int UnknownCount(GiftTaste t) =>
                GiftHelper.GetUnknownBy(obj, t, mode).Count();

            int UnmetCount() =>
                pool.Count(c => c.IsUnmet);

            // ---------------------------------------------------------
            // Taste Sections
            // ---------------------------------------------------------
            void AddTaste(string label, GiftTaste t, bool enabled)
            {
                if (!enabled)
                    return;

                var known = Known(t).ToList();
                int unknownCount = UnknownCount(t);

                TooltipBuildHelper.AddSectionWithSeparator(list, () =>
                    BuildTasteSection(label, known, unknownCount, wrapSize)
                );
            }

            if (!GiftHelper.HasDiscoveredAllLovesLikes(obj, mode))
            {
                AddTaste("Loves", GiftTaste.Love, ModEntry.ModConfig.ShowLoves);
                AddTaste("Likes", GiftTaste.Like, ModEntry.ModConfig.ShowLikes);
                AddTaste("Neutral", GiftTaste.Neutral, ModEntry.ModConfig.ShowNeutral);
                AddTaste("Dislikes", GiftTaste.Dislike, ModEntry.ModConfig.ShowDislikes);
                AddTaste("Hates", GiftTaste.Hate, ModEntry.ModConfig.ShowHates);
            }

            //hide the non love/like (and empty) if all love/like are discovered
            else
            {
                if (Known(GiftTaste.Love).Any())
                    AddTaste("Loves", GiftTaste.Love, ModEntry.ModConfig.ShowLoves);

                if (Known(GiftTaste.Like).Any())
                    AddTaste("Likes", GiftTaste.Like, ModEntry.ModConfig.ShowLikes);
            }

            // ---------------------------------------------------------
            // Undiscovered Section (only in Global/Local and if there are still loves/likes to discover
            // ---------------------------------------------------------
            if (mode != TasteSourceMode.All &&
                ModEntry.ModConfig.ShowUndiscovered &&
                !GiftHelper.HasDiscoveredAllLovesLikes(obj, mode))
            {
                // Unknown NPCs (all 5 tastes)
                var unknownNPCs = GiftHelper.GetUndiscoveredBy(obj, mode)
                    .Select(npc => NpcGiftClassificationBuilder.Classify(npc))
                    .Where(c => c.IsAvailable && c.IsMet)
                    .ToList();

                int unmet = UnmetCount();

                if (unknownNPCs.Count > 0 || unmet > 0)
                {
                    TooltipBuildHelper.AddSectionWithSeparator(list, () =>
                        BuildUndiscoveredSection(unknownNPCs, unmet, wrapSize)
                    );
                }
            }

            return list;
        }

        // ---------------------------------------------------------
        // Taste Section Builder
        // ---------------------------------------------------------
        private static List<TooltipElement> BuildTasteSection(
            string label,
            IEnumerable<NpcGiftClassification> known,
            int unknownCount,
            int wrapSize)
        {
            var collapsible = known
                .OrderBy(c => c.Npc.displayName)
                .Select(BuildNpcSegment)
                .ToList();

            var end = new List<InlineSegment>();
            if (unknownCount > 0)
            {
                end.Add(new InlineSegment
                {
                    Text = $"+{unknownCount} unknown",
                    TextColor = TooltipColors.Muted
                });
            }

            var labelSegment = new InlineSegment
            {
                Text = label + ": ",
                TextColor = TooltipColors.Normal,
                Bold = true
            };

            var wrapped = TooltipBuildHelper.BuildWrappedSegmentBlock(
                startSegments: new List<InlineSegment> { labelSegment },
                collapsibleSegments: collapsible,
                endSegments: end,
                wrapSize: wrapSize,
                maxRows: 20
            );

            return new List<TooltipElement>
        {
            new() { InlineSegments = wrapped }
        };
        }

        // ---------------------------------------------------------
        // Undiscovered Section Builder
        // ---------------------------------------------------------
        private static List<TooltipElement> BuildUndiscoveredSection(
            IEnumerable<NpcGiftClassification> unknownNPCs,
            int unmetCount,
            int wrapSize)
        {
            var collapsible = unknownNPCs
                .OrderBy(c => c.Npc.displayName)
                .Select(BuildNpcSegment)
                .ToList();

            var end = new List<InlineSegment>();

            if (unmetCount > 0)
            {
                end.Add(new InlineSegment
                {
                    Text = $"+{unmetCount} unmet",
                    TextColor = TooltipColors.Muted
                });
            }

            var labelSegment = new InlineSegment
            {
                Text = "Undiscovered: ",
                TextColor = TooltipColors.Normal,
                Bold = true
            };


            var wrapped = TooltipBuildHelper.BuildWrappedSegmentBlock(
                startSegments: new List<InlineSegment> { labelSegment },
                collapsibleSegments: collapsible,
                endSegments: end,
                wrapSize: wrapSize,
                maxRows: 20
            );

            return new List<TooltipElement>
        {
            new() { InlineSegments = wrapped }
        };
        }

        private static Color GetNpcNameColor(NpcGiftClassification c)
        {
            if (ModEntry.ModConfig.DeemphasizeAlreadyGifted &&
                !c.CanGiftToday)
                return TooltipColors.Muted;

            if (ModEntry.ModConfig.HighlightNotMaxFriendship)
                return c.IsMaxHeart ? TooltipColors.Normal : TooltipColors.Perfection;

            return TooltipColors.Normal;
        }

        private static InlineSegment BuildNpcSegment(NpcGiftClassification c)
        {
            Color color = GetNpcNameColor(c);

            bool isNearby =
                ModEntry.ModConfig.EmphasizeNearbyNPCs &&
                GiftHelper.IsNpcNearby(c.Npc, ModEntry.ModConfig.NearbyRangeTiles);

            bool isBold = false;

            if (isNearby)
            {
                if (ModEntry.ModConfig.DeemphasizeAlreadyGifted && !c.CanGiftToday)
                    isBold = false;
                else
                    isBold = true;
            }

            return new InlineSegment
            {
                Text = c.Npc.displayName,
                TextColor = color,
                Bold = isBold,
                Underline = isNearby
            };
        }
    }
}
