using GiftDiscovery;
using GiftDiscovery.Compatibility;
using GiftDiscovery.GameData;
using GiftDiscovery.Helpers;
using GiftDiscovery.Models;
using GiftDiscovery.Models.Builders;
using GiftDiscovery.Services;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SDVCommon;
using SDVCommon.Compatibility;
using SDVCommon.Helpers;
using SDVCommon.Helpers.Tooltip;
using SDVCommon.Models.Builders;
using SDVCommon.Models.Tooltip;
using SDVCommon.Rendering;
using SDVData;
using StardewModdingAPI;
using StardewValley;


namespace GiftDiscovery.Tooltip
{

    public static class GiftTooltipBuilder
    {
        private static bool _isInitialized;

        private static List<TooltipElement>? _cachedTooltip;
        private static string? _cachedItemId;
        private static int _cachedConfigHash;
        private static HashSet<string> _cachedNearbyNPCSet = new();
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
            _cachedItemId = null;
            _cachedNearbyNPCSet.Clear();
            _cachedMenuChanged = false;
            _cachedToggleVersion = -1;
            _cachedGiftVersion = -1;
        }

        public static void DrawTooltip(SpriteBatch b, StardewValley.Object obj)
        {
            if (!GiftableObjectList.GiftableIds.Contains(obj.QualifiedItemId))
                return;

            var elements = GetTooltip(obj);
            if (elements is null || elements.Count == 0)
                return;

            TooltipRenderer.DrawBottomLeft(b, elements);
        }

        public static List<TooltipElement>? GetTooltip(StardewValley.Object obj)
        {
            string key = obj.ItemId;
            int configHash = ModEntry.ModConfig.GetHashCode();
            bool menuChanged = ModEntry.MenuStateChanged;
            int toggleVersion = ModEntry.ToggleVersion;
            int giftVersion = GiftKnowledgeService.GiftVersion;

            var nearbyNPCSet = NPCLocation.GetNearbyNPCNames(ModEntry.ModConfig.NearbyRangeTilesGiftTooltip);

            bool needsRebuild =
                _cachedTooltip == null ||
                key != _cachedItemId ||
                configHash != _cachedConfigHash ||
                menuChanged != _cachedMenuChanged ||
                !nearbyNPCSet.SetEquals(_cachedNearbyNPCSet) ||
                toggleVersion != _cachedToggleVersion ||
                giftVersion != _cachedGiftVersion;

            if (!needsRebuild)
                return _cachedTooltip;

            _cachedTooltip = BuildTooltip(obj);
            TooltipRenderer.InvalidateSize(_cachedTooltip);
            _cachedItemId = key;
            _cachedConfigHash = configHash;
            _cachedMenuChanged = menuChanged;
            _cachedNearbyNPCSet = nearbyNPCSet.ToHashSet();
            _cachedToggleVersion = toggleVersion;
            _cachedGiftVersion = giftVersion;

            return _cachedTooltip;
        }


        public static List<TooltipElement> BuildTooltip(
            StardewValley.Object obj)
        {
            int wrapSize = ModEntry.ModConfig.WrapSizeGift;
            int maxRows = ModEntry.ModConfig.MaxRowsGift;

            var list = new List<TooltipElement>();

                list.Add(new TooltipElement
                {
                    Icon = IconRegistry.GetIcon(obj.ItemId),
                    Text = obj.DisplayName
                });

            TasteSourceMode mode = ModEntry.ModConfig.TasteSourceMode;


            // ---------------------------------------------------------
            // Taste grouping
            // ---------------------------------------------------------
            string id = obj.QualifiedItemId;

            IEnumerable<NPCGiftStatus> Known(GiftTaste t) =>
                LearnedGiftsHelper.GetKnownFor(id, t, mode)
                    .Select(npc => NPCGiftStatusBuilder.GiftStatus(npc));

            int UnknownCount(GiftTaste t) =>
                LearnedGiftsHelper.GetUnknownFor(id, t, mode).Count();

            int UnmetCount() =>
                GiftableNPC.GetAllGiftableNPCs()
                    .Select(NPCGiftStatusBuilder.GiftStatus)
                    .Count(c => c.IsUnmet);

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
                    BuildTasteSection(label, known, unknownCount, wrapSize, maxRows)
                );
            }

            if (!LearnedGiftsHelper.HasDiscoveredAllLovesLikesforItem(id, mode))
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
                !LearnedGiftsHelper.HasDiscoveredAllLovesLikesforItem(id, mode))
            {
                // Unknown NPCs (all 5 tastes)
                var unknownNPCs = LearnedGiftsHelper.GetUndiscoveredBy(id, mode)
                    .Select(npc => NPCGiftStatusBuilder.GiftStatus(npc))
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

            // ---------------------------------------------------------
            // Mod Source Section
            // ---------------------------------------------------------
            if (ModEntry.ModConfig.ShowModSource)
            {
                var modSource = ModSourceHelper.GetModSource(obj.ItemId);

                if (!string.IsNullOrEmpty(modSource))
                {
                    TooltipBuildHelper.AddSectionWithSeparator(list, () =>
                        new List<TooltipElement>
                        {
                            new TooltipElement
                            {
                                Text = modSource,
                            }
                        }
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
            IEnumerable<NPCGiftStatus> known,
            int unknownCount,
            int wrapSize,
            int maxRows)
        {
            var collapsible = known
                .OrderBy(c => c.NPC.displayName)
                .Select(c => BuildNPCSegment(c.NPC))
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
                maxRows: maxRows,
                useCommas: true
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
            IEnumerable<NPCGiftStatus> unknownNPCs,
            int unmetCount,
            int wrapSize)
        {
            var collapsible = unknownNPCs
                .OrderBy(c => c.NPC.displayName)
                .Select(c => BuildNPCSegment(c.NPC))
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
                maxRows: 20,
                useCommas: true
            );

            return new List<TooltipElement>
        {
            new() { InlineSegments = wrapped }
        };
        }


        private static InlineSegment BuildNPCSegment(NPC npc)
        {
            Color color = DisplayHelper.GetNPCNameColor(npc);

            bool isNearby =
                ModEntry.ModConfig.EmphasizeNearbyNPCs &&
                NPCLocation.IsNPCNearby(npc, ModEntry.ModConfig.NearbyRangeTilesGiftTooltip);
            bool isBold = false;

            if (isNearby)
            {
                if (ModEntry.ModConfig.DeemphasizeAlreadyGifted && !NPCGiftStatusBuilder.GiftStatus(npc).CanGiftToday)
                    isBold = false;
                else
                    isBold = true;
            }

            return new InlineSegment
            {
                Text = npc.displayName,
                TextColor = color,
                Bold = isBold,
                Underline = isNearby
            };
        }
    }
}
