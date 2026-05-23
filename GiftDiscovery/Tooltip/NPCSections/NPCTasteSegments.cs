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
                .Select(id => HarvestInfoBuilder.LookupFromKey(IdHelper.CanonicalItemId(id)))
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
