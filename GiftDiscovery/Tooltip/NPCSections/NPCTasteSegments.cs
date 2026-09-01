using GiftDiscovery.Helpers;
using GiftDiscovery.ModData;
using GiftDiscovery.Models;
using SDVCommon;
using SDVCommon.GameData;
using SDVCommon.Helpers.Tooltip;
using SDVCommon.Icons;
using SDVCommon.Models.Tooltip;
using StardewValley;
using SObject = StardewValley.Object;

namespace GiftDiscovery.Tooltip.NPCSections
{
    internal class NPCTasteSegments
    {
        internal static List<TooltipElement> Build(NPC npc)
        {
            string timer = "Taste Segments";
            SDVCommonServices.PerfBegin(timer);

            TasteSourceMode mode = ModEntry.ModConfig.TasteSourceMode;

            var list = new List<TooltipElement>();

            // Loves
            if (ModEntry.ModConfig.SeparateUniversalLoves)
                AddTasteWithSeparatedLoves(list, npc, "Loves");
            else
                AddTaste(list, npc, "Loves", GiftTaste.Love);

            // Likes
            AddTaste(list, npc, "Likes", GiftTaste.Like);

            // Neutral, Dislikes, Hates
            if (!LearnedGiftsHelper.HasDiscoveredAllLovesLikesForNPC(npc, mode))
            {
                AddTaste(list, npc, "Neutral", GiftTaste.Neutral);
                AddTaste(list, npc, "Dislikes", GiftTaste.Dislike);
                AddTaste(list, npc, "Hates", GiftTaste.Hate);
            }

            SDVCommonServices.PerfEnd(timer, 10);

            return list;
        }

        private static void AddTaste(
            List<TooltipElement> list,
            NPC npc,
            string label,
            GiftTaste taste)
        {
            string timer = "Add taste";  // Logs {name} took {ms} ms
            SDVCommonServices.PerfBegin(timer);

            TasteSourceMode mode = NPCTooltipSettings.Mode;

            var knownQIds = LearnedGiftsHelper.GetKnownGiftsForNPC(npc, taste, mode);
            var knownObjects = GameObject.GetObjects(knownQIds).ToList();

            // Sort
            knownObjects = SortObjectsForTooltip(knownObjects);

            // Unknown count
            int unknownCount = LearnedGiftsHelper
                .GetUnknownGiftsForNPC(npc, taste, mode)
                .Count();

            // Skip if nothing to show
            if (knownObjects.Count == 0 && unknownCount == 0)
                return;

            // Build segments
            var segments = AssembleKnownAndUnknown(knownObjects, unknownCount);

            TooltipBuildHelper.AddSectionWithSeparator(list, () =>
                AssembleTasteSection(label, segments));

            SDVCommonServices.PerfEnd(timer, $"{taste}", 10);
        }

        private static void AddTasteWithSeparatedLoves(
            List<TooltipElement> list,
            NPC npc,
            string label)
        {
            string timer = "Add taste separated loves";  // Logs {name} took {ms} ms
            SDVCommonServices.PerfBegin(timer);

            TasteSourceMode mode = NPCTooltipSettings.Mode;

            // Knowns: Split into regular + universal
            var knownQIds = LearnedGiftsHelper.GetKnownGiftsForNPC(npc, GiftTaste.Love, mode);
            var knownObjects = GameObject.GetObjects(knownQIds).ToList();

            var knownRegular = new List<SObject>();
            var knownUniversal = new List<SObject>();

            foreach (var obj in knownObjects)
            {
                if (UniversalLoveList.IsUniversalLove(obj.QualifiedItemId))
                    knownUniversal.Add(obj);
                else
                    knownRegular.Add(obj);
            }

            // Sort both lists
            knownRegular = SortObjectsForTooltip(knownRegular);
            knownUniversal = SortObjectsForTooltip(knownUniversal);

            SDVCommonServices.PerfPing(timer, "Known items", 10); // name it for what just finished

            // Unknown count both lists
            var unknownQIds = LearnedGiftsHelper.GetUnknownGiftsForNPC(npc, GiftTaste.Love, mode);

            int unknownRegular = unknownQIds
                .Where(qId => !UniversalLoveList.IsUniversalLove(qId))
                .Count();

            int unknownUniversal = unknownQIds
                .Where(qId => UniversalLoveList.IsUniversalLove(qId))
                .Count();

            SDVCommonServices.PerfPing(timer, "Unknown items", 10); // name it for what just finished

            // Assemble both with divider between
            // Regular loves
            var segments = AssembleKnownAndUnknown(knownRegular, unknownRegular);

            // Divider
            if ((knownRegular.Count > 0 || unknownRegular > 0) &&
                (knownUniversal.Count > 0 || unknownUniversal > 0))
            {
                segments.Add(new InlineSegment
                {
                    Text = " | ",
                });
            }

            // Universal loves
            segments.AddRange(AssembleKnownAndUnknown(knownUniversal, unknownUniversal));

            // Skip if nothing to show
            if (segments.Count == 0)
                return;

            TooltipBuildHelper.AddSectionWithSeparator(list, () =>
                AssembleTasteSection(label, segments));

            SDVCommonServices.PerfPing(timer, "Build segments", 10); // name it for what just finished
            SDVCommonServices.PerfEnd(timer, 10);
        }

        private static List<SObject> SortObjectsForTooltip(IEnumerable<SObject> objects)
        {
            return objects
                .OrderByDescending(obj => Inventory.IsInBackpack(obj.QualifiedItemId))
                .ThenBy(obj => obj.DisplayName)
                .ToList();
        }

        private static List<InlineSegment> AssembleKnownAndUnknown(
            IEnumerable<SObject> knownObjects,
            int unknownCount)
        {
            var segments = new List<InlineSegment>();

            // Convert each object into a segment
            foreach (var obj in knownObjects)
                segments.Add(BuildOneObjectSegment(obj));

            // Add "(X)" unknown count
            if (unknownCount > 0)
            {
                segments.Add(new InlineSegment
                {
                    Text = $"({unknownCount})",
                    TextColor = TooltipColors.Muted
                });
            }
            return segments;
        }

        private static InlineSegment BuildOneObjectSegment(SObject obj)
        {
            string qId = obj.QualifiedItemId;
            bool inBackpack = Inventory.IsInBackpack(qId);

            // Display name only if in backpack
            string name = inBackpack ? obj.DisplayName : "";

            if (!string.IsNullOrEmpty(name))
                name += ", ";

            return new InlineSegment
            {
                Icon = IconRegistry.GetIcon(qId),
                Text = name,
                TextColor = TooltipColors.Normal,
                Underline = false
            };
        }

        private static List<TooltipElement> AssembleTasteSection(
            string label,
            List<InlineSegment> segments)
        {
            int wrapSize = NPCTooltipSettings.WrapSizeNPC;
            int maxRows = NPCTooltipSettings.MaxRowsNPC;

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

            return new List<TooltipElement> {
                new() { InlineSegments = wrapped }
            };
        }


    }
}
