using GiftDiscovery;
using GiftDiscovery.GameData;
using GiftDiscovery.Helpers;
using GiftDiscovery.Models;
using GiftDiscovery.Services;
using SDVCommon;
using SDVCommon.GameData;
using SDVCommon.Helpers;
using SDVCommon.Helpers.Tooltip;
using SDVCommon.Icons;
using SDVCommon.Models.Builders;
using SDVCommon.Models.Tooltip;
using SDVCommon.Services;
using SDVData;
using StardewModdingAPI;
using StardewValley;

namespace GiftDiscovery.Tooltip.NPCSections
{
    internal class NPCTasteSegments
    {
        public static List<TooltipElement> Build(NPC npc)
        {
            string timer = "Taste Segments";
            LogLevel logLevel = LogHelper.DebugOrTrace;
            SDVCommonServices.PerfBegin(timer);

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

            SDVCommonServices.PerfEnd(timer, 10, logLevel);

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
            string timer = "Add taste";  // Logs {name} took {ms} ms
            SDVCommonServices.PerfBegin(timer);

            var knownIds = LearnedGiftsHelper.GetKnownGiftsForNPC(npc, taste, mode);

            var items = knownIds
                .Where(id => GiftableObjectList.GetAllGiftableIds().Contains(id))
                .Select(id => ItemRegistry.Create(id))
                .Where(item => item is not null)
                .ToList();

            // Sort: backpack first, then alphabetical
            items = items
                .OrderByDescending(item => Inventory.IsInBackpack(item.QualifiedItemId))
                .ThenBy(item => item.DisplayName)
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

            SDVCommonServices.PerfEnd(timer, $"{taste}", 10);

        }

        private static void AddTasteSeparatedLoves(
            List<TooltipElement> list,
            NPC npc,
            string label,
            TasteSourceMode mode,
            int wrapSize,
            int maxRows)
        {

            string timer = "Add taste separated loves";  // Logs {name} took {ms} ms
            LogLevel logLevel = LogHelper.InfoOrTrace;
            SDVCommonServices.PerfBegin(timer);

            //// Knowns: Split into regular + universal
            var knownIds = LearnedGiftsHelper.GetKnownGiftsForNPC(npc, GiftTaste.Love, mode);

            var knownItems = knownIds
                .Where(id => GiftableObjectList.GetAllGiftableIds().Contains(id))
                .Select(id => ItemRegistry.Create(id))
                .Where(item => item is not null)
                .ToList();

            var knownRegular = new List<Item>();
            var knownUniversal = new List<Item>();

            foreach (var item in knownItems)
            {
                if (GiftType.GetUniversalLoveIds().Contains(item.ItemId))
                    knownUniversal.Add(item);
                else
                    knownRegular.Add(item);
            }

            // Sort both lists
            knownRegular = knownRegular
                .OrderByDescending(item => Inventory.IsInBackpack(item.ItemId))
                .ThenBy(item => item.DisplayName)
                .ToList();

            knownUniversal = knownUniversal
                .OrderByDescending(item => Inventory.IsInBackpack(item.ItemId))
                .ThenBy(item => item.DisplayName)
                .ToList();

            SDVCommonServices.PerfPing(timer, "Known items", 10, logLevel); // name it for what just finished

            // Unknowns: Count regular + universal
            var unknownIds = LearnedGiftsHelper.GetUnknownGiftsForNPC(npc, GiftTaste.Love, mode);

            int unknownRegular = unknownIds
                .Where(id => !GiftType.GetUniversalLoveIds().Contains(id))
                .Count();

            int unknownUniversal = unknownIds
                .Where(id => GiftType.GetUniversalLoveIds().Contains(id))
                .Count();

            SDVCommonServices.PerfPing(timer, "Unknown items", 10, logLevel); // name it for what just finished

            var segments = new List<InlineSegment>();

            // Regular loves
            foreach (var info in knownRegular)
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
            if ((knownRegular.Count > 0 || unknownRegular > 0) &&
                (knownUniversal.Count > 0 || unknownUniversal > 0))
            {
                segments.Add(new InlineSegment
                {
                    Text = " | ",
                    //Bold = true,
                });
            }

            // Universal loves
            foreach (var info in knownUniversal)
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

            SDVCommonServices.PerfPing(timer, "Build segments", 10); // name it for what just finished
            SDVCommonServices.PerfEnd(timer, 10);

        }


        private static List<TooltipElement> BuildNPCTasteSection(
            string label,
            List<Item> items,
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

        private static InlineSegment BuildItemSegment(Item item)
        {
            bool inBackpack = Inventory.IsInBackpack(item.ItemId);

            string name = inBackpack ? item.DisplayName : "";

            if (!string.IsNullOrEmpty(name))
                name += ",";

            return new InlineSegment
            {
                Icon = IconRegistry.GetIcon(item.ItemId),
                Text = name,
                TextColor = TooltipColors.Normal,
                Underline = false
            };
        }

    }
}
