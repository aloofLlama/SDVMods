using GiftDiscovery;
using GiftDiscovery.Helpers;
using GiftDiscovery.Models;
using GiftDiscovery.Services;
using Microsoft.Xna.Framework;
using SDVCommon.Helpers;
using SDVCommon.Tooltip;
using StardewValley;
using System.Reflection.Emit;


public static class TooltipBuilder
{
    public static List<TooltipElement> BuildTooltip(
        StardewValley.Object obj)
    {
        int wrapSize = 6;
        var list = new List<TooltipElement>();

        var allGiftable = GiftHelper.GetAllGiftableNPCs()
            .Select(NpcGiftClassificationBuilder.Classify)
            .Where(c => c.IsGiftable)
            .ToList();

        var available = allGiftable.Where(c => c.IsAvailable).ToList();

        // ---------------------------------------------------------
        // Use the user selected mode
        // ---------------------------------------------------------
        TasteSourceMode mode = ModEntry.ModConfig.TasteSourceMode;

        List< NpcGiftClassification> pool =
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
            int unknown = UnknownCount(t);

            TooltipBuildHelper.AddSectionWithSeparator(list, () =>
                BuildTasteSection(label, known, unknown, wrapSize)
            );
        }

        AddTaste("Loves", GiftTaste.Love, ModEntry.ModConfig.ShowLoves);
        AddTaste("Likes", GiftTaste.Like, ModEntry.ModConfig.ShowLikes);
        AddTaste("Neutral", GiftTaste.Neutral, ModEntry.ModConfig.ShowNeutral);
        AddTaste("Dislikes", GiftTaste.Dislike, ModEntry.ModConfig.ShowDislikes);
        AddTaste("Hates", GiftTaste.Hate, ModEntry.ModConfig.ShowHates);

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
            new TooltipElement { InlineSegments = wrapped }
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
            new TooltipElement { InlineSegments = wrapped }
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
