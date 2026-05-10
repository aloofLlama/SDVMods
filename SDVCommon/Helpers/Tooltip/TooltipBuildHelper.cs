using SDVCommon.Models.Tooltip;

namespace SDVCommon.Helpers.Tooltip
{
    internal class TooltipBuildHelper
    {
        public static void AddIfNotNull(List<TooltipElement> list, TooltipElement? element)
        {
            if (element != null && IsVisible(element))
                list.Add(element);
        }

        public static bool SectionHasVisibleContent(IEnumerable<TooltipElement> section)
        {
            return section.Any(IsVisible);
        }

        public static bool IsVisible(TooltipElement e)
        {
            bool hasInline = e.InlineSegments != null &&
                             e.InlineSegments.Any(seg =>
                                 !string.IsNullOrWhiteSpace(seg.Text) ||
                                 seg.Icon != null);

            return
                hasInline ||
                !string.IsNullOrWhiteSpace(e.Text) ||
                e.IconTexture != null ||
                e.IsSeparator;
        }

        public static void AddSectionWithSeparator(
            List<TooltipElement> list,
            Func<IEnumerable<TooltipElement>> sectionBuilder,
            int paddingTop = 3,
            int paddingBottom = 3
            )
        {
            var section = sectionBuilder()?.ToList();
            if (section == null || !SectionHasVisibleContent(section))
                return;

            if (list.Count > 0)
            {
                list.Add(new TooltipElement
                {
                    IsSeparator = true,
                    PaddingTop = paddingTop,
                    PaddingBottom = paddingBottom
                });
            }

            foreach (var element in section)
                AddIfNotNull(list, element);
        }

        // Put multiple segments on the same line with separators (e.g., seasons)
        public static List<InlineSegment> BuildInlineSegmentswithSeparators<T>(
            IEnumerable<T> items,
            Func<T, IEnumerable<InlineSegment>> buildSegments,
            string separator = " • ")
        {
            var result = new List<InlineSegment>();
            var list = items.ToList();

            for (int i = 0; i < list.Count; i++)
            {
                var segs = buildSegments(list[i]).ToList();

                if (segs.Count == 0)
                    continue;

                result.AddRange(segs);

                // Look ahead: does the next item produce segments?
                for (int j = i + 1; j < list.Count; j++)
                {
                    var nextSegs = buildSegments(list[j]);
                    if (nextSegs != null && nextSegs.Any())
                    {
                        result.Add(new InlineSegment
                        {
                            Text = separator,
                            TextColor = TooltipColors.Muted,
                            Bold = false
                        });
                        break;
                    }
                }
            }

            return result;

        }

        public static List<InlineSegment> BuildInlineSegmentsWithCommas<T>(
    IEnumerable<T> items,
    Func<T, IEnumerable<InlineSegment>> buildSegments)
        {
            var result = new List<InlineSegment>();
            var list = items.ToList();

            for (int i = 0; i < list.Count; i++)
            {
                // Build segments for this item
                var segs = buildSegments(list[i]).ToList();

                if (segs.Count == 0)
                    continue;

                // Add the segments
                result.AddRange(segs);

                // Look ahead: does the next item produce segments?
                for (int j = i + 1; j < list.Count; j++)
                {
                    var nextSegs = buildSegments(list[j]);
                    if (nextSegs != null && nextSegs.Any())
                    {
                        // Add comma matching previous segment color
                        result.Add(new InlineSegment
                        {
                            Text = ", ",
                            TextColor = result.Last().TextColor,   // inherit color
                            Bold = false
                        });
                        break;
                    }
                }
            }

            return result;
        }



        public static List<InlineSegment> BuildWrappedSegmentBlock(
            List<InlineSegment> startSegments,
            List<InlineSegment> collapsibleSegments,
            List<InlineSegment> endSegments,
            int wrapSize,
            int maxRows,
            bool useCommas = true
        )
        {
            var unified = new List<InlineSegment>();
            unified.AddRange(startSegments);
            unified.AddRange(collapsibleSegments);
            unified.AddRange(endSegments);

            int S = startSegments.Count;
            int C = collapsibleSegments.Count;
            int E = endSegments.Count;

            int maxCapacity = wrapSize * maxRows;
            int total = S + C + E;

            // collapse only if we truly overflow
            if (total > maxCapacity)
            {
                // we must reserve 1 slot for the "+X known" segment
                int collapsibleFit = Math.Max(0, maxCapacity - S - E - 1);

                // how many get collapsed
                int collapsedCount = C - collapsibleFit;

                // rebuild unified list
                unified.Clear();
                unified.AddRange(startSegments);

                // explicit knowns that still fit
                if (collapsibleFit > 0)
                    unified.AddRange(collapsibleSegments.Take(collapsibleFit));

                // collapsed segment (only if there’s something to collapse)
                if (collapsedCount > 0)
                {
                    unified.Add(new InlineSegment
                    {
                        //TODO abstract the specific text out so the collapse can be used in other situations
                        Text = $"+{collapsedCount} known",
                        TextColor = TooltipColors.Deemphasize
                    });
                }

                unified.AddRange(endSegments);
            }

            return WrapWithCommas(unified, wrapSize, maxRows, S, useCommas);
        }

        private static List<InlineSegment> WrapWithCommas(
            List<InlineSegment> segments,
            int wrapSize,
            int maxRows,
            int startCount,
            bool useCommas
        )
        {
            var result = new List<InlineSegment>();

            int globalIndex = 0;
            int row = 0;

            while (globalIndex < segments.Count && row < maxRows)
            {
                if (row > 0)
                    result.Add(new InlineSegment { IsLineBreak = true });

                int itemsThisRow = 0;

                while (globalIndex < segments.Count && itemsThisRow < wrapSize)
                {
                    var seg = segments[globalIndex];
                    result.Add(seg);

                    bool isStart = (globalIndex < startCount);

                    itemsThisRow++;
                    globalIndex++;

                    if (useCommas && !isStart && itemsThisRow < wrapSize && globalIndex < segments.Count)
                    {
                        result.Add(new InlineSegment
                        {
                            Text = ", ",
                            TextColor = seg.TextColor
                        });
                    }
                    else
                    {
                        result.Add(new InlineSegment
                        {
                            Text = " ",
                            TextColor = seg.TextColor
                        });

                    }
                }

                row++;
            }

            return result;
        }





    }
}
