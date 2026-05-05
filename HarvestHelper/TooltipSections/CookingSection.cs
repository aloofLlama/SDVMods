using HarvestHelper.Helpers;
using SDVCommon.Helpers;
using SDVCommon.Icons;
using SDVCommon.Models.Wrappers;
using SDVCommon.Services;
using SDVCommon.Tooltip;
using StardewModdingAPI;
using System.Collections.Generic;
using Microsoft.Xna.Framework;


namespace HarvestHelper.TooltipSections
{
    public static class CookingSection
    {
        public static List<TooltipElement> Build(HarvestInfo harvest)
        {
            var list = new List<TooltipElement>();

            string harvestId = harvest.Data.HarvestId;
            var (known, unknown) = CookingRecipeService.CountRecipesUsing(harvestId);
            var (cooked, uncooked) = CookingRecipeService.CountCookedRecipesUsing(harvestId);
            int total = known + unknown;

            //ModEntry.Instance.Monitor.Log($"TEXT: {harvestId} {known}  {unknown}", LogLevel.Info);

            if (known == 0 && unknown == 0)
                return list;

            List<InlineSegment> achievementSegment;

            if (cooked != total)
            {
                achievementSegment = new List<InlineSegment>
                {
                    new InlineSegment
                    {
                        Text = $"{cooked}/{total}",
                        TextColor = TooltipColors.Perfection
                    }
                };
            }
            else
            {
                achievementSegment = new List<InlineSegment>();
            }

            List<InlineSegment> knownUnknownCombined;

            var knownIconSegments = CookingRecipeService
                .GetKnownRecipesUsing(harvestId)
                .Select(r => new InlineSegment
                {
                    Icon = r.Runtime.DishIcon
                })
                .ToList();


            if (known < 5)
            {
                //
                // CASE A: FEWER THAN 5 KNOWN → ICONS + (UNKNOWN)
                //
                knownUnknownCombined = new List<InlineSegment>();

                // Add icons
                knownUnknownCombined.AddRange(knownIconSegments);

                // Add "(unknown)" text if needed
                if (unknown > 0)
                {
                    knownUnknownCombined.Add(new InlineSegment
                    {
                        Text = string.Format(
                            ModEntry.ModHelper.Translation.Get(TooltipKeys.Qty_Unknown),
                            unknown
                        ),
                        TextColor = TooltipColors.Muted
                    });
                }
            }
            else
            {
                //
                // CASE B: 5 OR MORE KNOWN → "X known Y unknown" (no comma)
                //
                knownUnknownCombined = new List<InlineSegment>();

                if (known > 0)
                {
                    knownUnknownCombined.Add(new InlineSegment
                    {
                        Text = string.Format(
                            ModEntry.ModHelper.Translation.Get(TooltipKeys.Qty_Known),
                            known
                        ),
                        TextColor = TooltipColors.Normal
                    });
                }

                if (unknown > 0)
                {
                    // add a space if both segments exist
                    if (known > 0)
                    {
                        knownUnknownCombined.Add(new InlineSegment
                        {
                            Text = " ",
                            TextColor = TooltipColors.Normal
                        });
                    }

                    knownUnknownCombined.Add(new InlineSegment
                    {
                        Text = string.Format(
                            ModEntry.ModHelper.Translation.Get(TooltipKeys.Qty_Unknown),
                            unknown
                        ),
                        TextColor = TooltipColors.Muted
                    });
                }
                //
                // CASE B: 5 OR MORE KNOWN → "X known Y unknown" (no comma)
                //
                knownUnknownCombined = new List<InlineSegment>();

                if (known > 0)
                {
                    knownUnknownCombined.Add(new InlineSegment
                    {
                        Text = string.Format(
                            ModEntry.ModHelper.Translation.Get(TooltipKeys.Qty_Known),
                            known
                        ),
                        TextColor = TooltipColors.Normal
                    });
                }

                if (unknown > 0)
                {
                    // add a space if both segments exist
                    if (known > 0)
                    {
                        knownUnknownCombined.Add(new InlineSegment
                        {
                            Text = " ",
                            TextColor = TooltipColors.Normal
                        });
                    }

                    knownUnknownCombined.Add(new InlineSegment
                    {
                        Text = string.Format(
                            ModEntry.ModHelper.Translation.Get(TooltipKeys.Qty_Unknown),
                            unknown
                        ),
                        TextColor = TooltipColors.Muted
                    });
                }
            }

            //var knownIconSegments = CookingRecipeService
            //    .GetKnownRecipesUsing(harvestId)
            //    .Select(r => new InlineSegment
            //    {
            //        Icon = r.Runtime.DishIcon
            //    })
            //    .ToList();

            //var unknownSegments = new List<InlineSegment>();

            //if (unknown > 0)
            //{
            //    unknownSegments.Add(new InlineSegment
            //    {
            //        Text = string.Format(
            //            ModEntry.ModHelper.Translation.Get(TooltipKeys.Qty_Unknown),
            //            unknown
            //        ),
            //        TextColor = TooltipColors.Muted
            //    });
            //}

            //var knownUnknownCombined = new List<InlineSegment>();
            //knownUnknownCombined.AddRange(knownIconSegments);
            //knownUnknownCombined.AddRange(unknownSegments);



            //var knownUnknownSegments = TooltipBuildHelper.BuildInlineSegmentsWithCommas(
            //    new[]
            //    {
            //        new { Count = known, Key = TooltipKeys.Qty_Known, Color = TooltipColors.Normal },
            //        new { Count = unknown, Key = TooltipKeys.Qty_Unknown, Color = TooltipColors.Muted }
            //    }
            //    ,
            //    x =>
            //    {
            //        if (x.Count == 0)
            //            return Enumerable.Empty<InlineSegment>();

            //        return new[]
            //        {
            //            new InlineSegment
            //            {
            //                Text = string.Format(ModEntry.ModHelper.Translation.Get(x.Key), x.Count),
            //                TextColor = x.Color
            //            }
            //        };
            //    });

            var segments = TooltipBuildHelper.BuildInlineSegmentswithSeparators(
                new[]
                {
                    new { Type = "Achievement" },
                    new { Type = "KnownUnknown" }
                },
                x =>
                {
                    if (x.Type == "Achievement")
                        return achievementSegment;

                    if (x.Type == "KnownUnknown")
                        return knownUnknownCombined;

                    return Enumerable.Empty<InlineSegment>();
                }
            );

            // add the plate icon + segments
            list.Add(new TooltipElement
            {
                Icon = TooltipIcons.Get(IconKey.Plate),
                InlineSegments = segments
            });

            // Add mini-fridge count line
            int fridgeQty = InventoryHelper.CountOwnedInMainFarmhouseFridges(harvestId);

            if (fridgeQty > 0)
            {
                list.Add(new TooltipElement
                {
                    Icon = TooltipIcons.Get(IconKey.MiniFridge),
                    Text = string.Format(ModEntry.ModHelper.Translation.Get(TooltipKeys.Owned),
                    fridgeQty)
                });
            }


            return list;
        }
    }
}
