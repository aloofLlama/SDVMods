using HarvestHelper.Helpers;
using SDVCommon.Helpers;
using SDVCommon.Icons;
using SDVCommon.Models.Wrappers;
using SDVCommon.GameData;
using SDVCommon.Helpers.Tooltip;
using SDVCommon.Models.Tooltip;


namespace HarvestHelper.TooltipSections
{
    public static class CookingSection
    {
        public static List<TooltipElement> Build(HarvestInfo harvest)
        {
            var list = new List<TooltipElement>();

            string harvestId = harvest.Data.HarvestId;
            var (known, unknown) = CookingRecipe.CountRecipesUsing(harvestId);
            var (cooked, uncooked) = CookingRecipe.CountCookedRecipesUsing(harvestId);
            int total = known + unknown;

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

            var knownIconSegments = CookingRecipe
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

            int fridgeQty = Inventory.CountOwnedInMainFarmhouseFridges(harvestId);

            var fridgeSegment = new[]
            {
                new InlineSegment
                {
                    Icon = TooltipIcons.GetIconForGameObject("(BC)216", 1.2f), //MiniFridge
                    Text = string.Format(ModEntry.ModHelper.Translation.Get(TooltipKeys.Owned), 
                        fridgeQty)
                }
            };



            var segments = TooltipBuildHelper.BuildInlineSegmentswithSeparators(
                new[]
                {
                    new { Type = "Achievement" },
                    new { Type = "KnownUnknown" },
                    new { Type = "Fridge" }
                },
                x =>
                {
                    if (x.Type == "Achievement")
                        return achievementSegment;

                    if (x.Type == "KnownUnknown")
                        return knownUnknownCombined;

                    if (x.Type == "Fridge")
                        return fridgeSegment;

                    return Enumerable.Empty<InlineSegment>();
                }
            );

            // add the plate icon + segments
            list.Add(new TooltipElement
            {
                Icon = TooltipIcons.Get(IconKey.Plate),
                InlineSegments = segments
            });

            return list;
        }
    }
}
