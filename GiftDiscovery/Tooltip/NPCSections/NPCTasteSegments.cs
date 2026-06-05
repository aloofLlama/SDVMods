using GiftDiscovery;
using GiftDiscovery.GameData;
using GiftDiscovery.Helpers;
using GiftDiscovery.Models;
using SDVCommon.GameData;
using SDVCommon.Helpers;
using SDVCommon.Helpers.Tooltip;
using SDVCommon.Icons;
using SDVCommon.Models.Builders;
using SDVCommon.Models.Tooltip;
using SDVCommon.Models.Wrappers;
using StardewValley;

namespace GiftDiscovery.Tooltip.NPCSections
{
    internal class NPCTasteSegments
    {
        public static List<TooltipElement> Build(NPC npc)
        {
            TasteSourceMode mode = ModEntry.ModConfig.TasteSourceMode;
            int wrapSize = ModEntry.ModConfig.WrapSizeNPC;
            int maxRows = ModEntry.ModConfig.MaxRowsNPC;

            var list = new List<TooltipElement>();

            if (ModEntry.ModConfig.SeparateUniversalLoves)
                AddTasteSeparatedLoves(list, npc, "Loves", mode, wrapSize, maxRows);
            else
                AddTaste(list, npc, "Loves", GiftTaste.Love, mode, wrapSize, maxRows);

            AddTaste(list, npc, "Likes", GiftTaste.Like, mode, wrapSize, maxRows);

            if (!LearnedGiftsHelper.HasDiscoveredAllLovesLikesForNPC(npc, mode))
            {
                AddTaste(list, npc, "Neutral", GiftTaste.Neutral, mode, wrapSize, maxRows);
                AddTaste(list, npc, "Dislikes", GiftTaste.Dislike, mode, wrapSize, maxRows);
                AddTaste(list, npc, "Hates", GiftTaste.Hate, mode, wrapSize, maxRows);
            }

            return list;
        }

        private static void AddTaste(
            List<TooltipElement> list,
            NPC npc,
            string label,
            GiftTaste taste,
            TasteSourceMode mode,
            int wrapSize,
            int maxRows)
        {
            var knownIds = LearnedGiftsHelper.GetKnownGiftsForNPC(npc, taste, mode);

            var filteredIds = knownIds
                .Where(id => GiftableObjectList.GiftableIds.Contains(id));

            var items = filteredIds
                .Select(id => HarvestInfoBuilder.LookupFromKey(IdHelper.ToItemId(id)))
                .Where(info => info is not null)
                .Cast<HarvestInfo>()
                .ToList();

            //sort so backpack items appear first, then alphabetically by name
            items = items
            .OrderByDescending(i => Inventory.IsInBackpack(i.Data.HarvestId))
            .ThenBy(i => i.Runtime.DisplayName)
            .ToList();

            // Unknown count
            int unknownCount = LearnedGiftsHelper
                .GetUnknownGiftsForNPC(npc, taste, mode)
                .Count(); ;

            // Skip if nothing to show
            if (items.Count == 0 && unknownCount == 0)
                return;

            TooltipBuildHelper.AddSectionWithSeparator(list, () =>
                BuildNPCTasteSection(label, items, unknownCount, wrapSize, maxRows)
            );
        }

        private static void AddTasteSeparatedLoves(
            List<TooltipElement> list,
            NPC npc,
            string label,
            TasteSourceMode mode,
            int wrapSize,
            int maxRows)
        {
            var knownIds = LearnedGiftsHelper.GetKnownGiftsForNPC(npc, GiftTaste.Love, mode);

            var filteredIds = knownIds
                .Where(id => GiftableObjectList.GiftableIds.Contains(id));

            var allItems = filteredIds
                .Select(id => HarvestInfoBuilder.LookupFromKey(IdHelper.ToItemId(id)))
                .Where(info => info is not null)
                .Cast<HarvestInfo>()
                .ToList();

            // Split into regular + universal
            var regular = new List<HarvestInfo>();
            var universal = new List<HarvestInfo>();

            foreach (var info in allItems)
            {
                if (GiftType.IsUniversalLove(info.Data.HarvestId))
                    universal.Add(info);
                else
                    regular.Add(info);
            }

            // Sort each group
            regular = regular
                .OrderByDescending(i => Inventory.IsInBackpack(i.Data.HarvestId))
                .ThenBy(i => i.Runtime.DisplayName)
                .ToList();

            universal = universal
                .OrderByDescending(i => Inventory.IsInBackpack(i.Data.HarvestId))
                .ThenBy(i => i.Runtime.DisplayName)
                .ToList();

            // Unknowns: split by universal vs regular
            int unknownRegular = 0;
            int unknownUniversal = 0;

            foreach (var qualifiedId in LearnedGiftsHelper.GetUnknownGiftsForNPC(npc, GiftTaste.Love, mode))
            {
                var ItemId = IdHelper.ToItemId(qualifiedId);

                if (GiftType.IsUniversalLove(ItemId))
                    unknownUniversal++;
                else
                    unknownRegular++;
            }

            var segments = new List<InlineSegment>();

            // Regular loves
            foreach (var info in regular)
                segments.Add(BuildItemSegment(info));

            if (unknownRegular > 0)
            {
                segments.Add(new InlineSegment
                {
                    Text = $"({unknownRegular})",
                    TextColor = TooltipColors.Muted
                });
            }

            // Separator
            if ((regular.Count > 0 || unknownRegular > 0) &&
                (universal.Count > 0 || unknownUniversal > 0))
            {
                segments.Add(new InlineSegment
                {
                    Text = "|",
                    //Bold = true,
                });
            }

            // Universal loves
            foreach (var info in universal)
                segments.Add(BuildItemSegment(info));

            if (unknownUniversal > 0)
            {
                segments.Add(new InlineSegment
                {
                    Text = $"({unknownUniversal})",
                    TextColor = TooltipColors.Muted
                });
            }

            // Skip if nothing to show
            if (segments.Count == 0)
                return;

            TooltipBuildHelper.AddSectionWithSeparator(list, () =>
            {
                var labelSegment = new InlineSegment
                {
                    Text = label + ": ",
                    Bold = true,
                    TextColor = TooltipColors.Normal
                };

                var wrapped = TooltipBuildHelper.BuildWrappedSegmentBlock(
                    startSegments: new List<InlineSegment> { labelSegment },
                    collapsibleSegments: segments,
                    endSegments: new List<InlineSegment>(),
                    wrapSize: wrapSize,
                    maxRows: maxRows,
                    useCommas: false
                );

                return new List<TooltipElement>
                {
                    new TooltipElement { InlineSegments = wrapped }
                };
            });
        }


        private static List<TooltipElement> BuildNPCTasteSection(
            string label,
            List<HarvestInfo> items,
            int unknownCount,
            int wrapSize,
            int maxRows)
        {
            var segments = new List<InlineSegment>();

            foreach (var obj in items)
                segments.Add(BuildItemSegment(obj));

            // Unknown count
            if (unknownCount > 0)
            {
                segments.Add(new InlineSegment
                {
                    Text = $"({unknownCount})",
                    TextColor = TooltipColors.Muted
                });
            }

            // Label segment
            var labelSegment = new InlineSegment
            {
                Text = label + ": ",
                Bold = true,
                TextColor = TooltipColors.Normal
            };

            var wrapped = TooltipBuildHelper.BuildWrappedSegmentBlock(
                startSegments: new List<InlineSegment> { labelSegment },
                collapsibleSegments: segments,
                endSegments: new List<InlineSegment>(),
                wrapSize: wrapSize,
                maxRows: maxRows,
                useCommas: false
            );

            return new List<TooltipElement>
                {
                    new TooltipElement { InlineSegments = wrapped }
                };
        }

        private static InlineSegment BuildItemSegment(HarvestInfo harvest)
        {
            bool inBackpack = Inventory.IsInBackpack(harvest.Data.HarvestId);

            string name = inBackpack ? harvest.Runtime.DisplayName : "";

            // Add comma *only* if we have a display name
            if (!string.IsNullOrEmpty(name))
                name += ",";

            return new InlineSegment
            {
                Icon = harvest.Runtime.HarvestIcon,
                Text = name,
                TextColor = TooltipColors.Normal,
                Underline = false
            };
        }

    }
}
