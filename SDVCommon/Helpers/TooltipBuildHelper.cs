using System;
using System.Collections.Generic;
using System.Text;
using SDVCommon.Tooltip;

namespace SDVCommon.Helpers
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


    }
}
