using GiftDiscovery.GameData.Dynamic;
using GiftDiscovery.GameData.Static;
using GiftDiscovery.Helpers;
using GiftDiscovery.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SDVCommon.Compatibility;
using SDVCommon.Helpers.Tooltip;
using SDVCommon.Models.Tooltip;
using SDVCommon.Rendering;
using StardewValley;
using System.Security.Cryptography;


namespace GiftDiscovery.Tooltip
{
    public static class GiftTooltipBuilder
    {
        internal static bool TooltipInvalidated { get; set; }

        private static List<TooltipElement>? _tooltip;
        private static string? _cachedGiftQId;
        private static HashSet<string> _cachedNearbyNPCSet = new();

        //------------------------------------------------
        // Data lifecycle methods
        //------------------------------------------------
        internal static void Reset()
        {
            if (_tooltip is not null)
                TooltipRenderer.InvalidateSize(_tooltip);
            _tooltip = null;
            _cachedGiftQId = null;
            _cachedNearbyNPCSet.Clear();
            TooltipInvalidated = false;
        }
        // Reset or update the relevent cached data
        internal static void RefreshCache(string qId, HashSet<string> nearbyNPCSet)
        {
            if (_tooltip is not null)
                TooltipRenderer.InvalidateSize(_tooltip);
            _cachedGiftQId = qId;
            _cachedNearbyNPCSet = nearbyNPCSet.ToHashSet();
            TooltipInvalidated = false;
        }

        //------------------------------------------------
        // Draw, Get, and Build methods
        //------------------------------------------------

        public static void DrawTooltip(SpriteBatch b, StardewValley.Object obj)
        {
            var elements = GetTooltip(obj);
            if (elements is null || elements.Count == 0)
                return;

            TooltipRenderer.DrawBottomLeft(b, elements);
        }

        public static List<TooltipElement>? GetTooltip(StardewValley.Object obj)
        {
            // Menu / config changes set TooltipInvalidated at their respective events.
            // Item or nearby NPC changes are checked here
            string qId = obj.QualifiedItemId; 
            var nearbyNPCSet = NPCLocation.GetNearbyNPCNames(ModEntry.ModConfig.NearbyRangeTilesGiftTooltip);

            if (qId != _cachedGiftQId ||
                !nearbyNPCSet.SetEquals(_cachedNearbyNPCSet) ||
                _tooltip == null )
            {
                TooltipInvalidated = true;
            }

            if (!TooltipInvalidated)
                return _tooltip;

            _tooltip = BuildTooltip(obj);
            RefreshCache(qId, nearbyNPCSet.ToHashSet());

            return _tooltip;
        }


        public static List<TooltipElement> BuildTooltip(
            StardewValley.Object obj)
        {
            int wrapSize = ModEntry.ModConfig.WrapSizeGift;
            int maxRows = ModEntry.ModConfig.MaxRowsGift;

            var list = new List<TooltipElement>
            {
                new() {
                    Icon = IconRegistry.GetIcon(obj.ItemId),
                    Text = obj.DisplayName
                }
            };

            TasteSourceMode mode = ModEntry.ModConfig.TasteSourceMode;

            // ---------------------------------------------------------
            // Taste grouping
            // ---------------------------------------------------------
            string id = obj.QualifiedItemId;

            IEnumerable<NPCGiftStatusData> Known(GiftTaste t) =>
                LearnedGiftsHelper.GetKnownFor(id, t, mode)
                    .Select(npc => NPCGiftStatus.GetNPCGiftStatus(npc));

            int UnknownCount(GiftTaste t) =>
                LearnedGiftsHelper.GetUnknownFor(id, t, mode).Count();

            int UnmetCount() =>
                NPCGiftStatus.GetAllGiftableNPCs()
                    .Select(NPCGiftStatus.GetNPCGiftStatus)
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
                    .Select(npc => NPCGiftStatus.GetNPCGiftStatus(npc))
                    .Where(c => c.IsAvailable && c.IsMet)
                    .ToList();

                int unmet = UnmetCount();

                if (unknownNPCs.Count > 0 || unmet > 0)
                {
                    TooltipBuildHelper.AddSectionWithSeparator(list, () =>
                        BuildUndiscoveredSection(unknownNPCs, unmet, wrapSize, maxRows)
                    );
                }
            }

            // ---------------------------------------------------------
            // Mod Source Section
            // ---------------------------------------------------------
            if (ModEntry.ModConfig.ShowModSource)
            {
                var modSource = ModSource.GetModSource(obj.ItemId);

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
            IEnumerable<NPCGiftStatusData> known,
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
            IEnumerable<Models.NPCGiftStatusData> unknownNPCs,
            int unmetCount,
            int wrapSize,
            int maxRows)
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
                maxRows: maxRows,
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
            
            bool canGiftToday = NPCGiftStatus.GetNPCGiftStatus(npc).CanGiftToday;

            bool isBold = false;


            if (isNearby)
            {
                if (ModEntry.ModConfig.DeemphasizeAlreadyGifted && !canGiftToday)
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
