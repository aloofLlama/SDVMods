using HarvestHelper.Helpers;
using SDVCommon.GameData;
using SDVCommon.Helpers;
using SDVCommon.Helpers.Tooltip;
using SDVCommon.Icons;
using SDVCommon.Models.Tooltip;
using SDVData;
using System.Net;


namespace HarvestHelper.TooltipSections
{
    public static class CookingSection
    {
        //Cooking section using the specific ingredient
        public static List<TooltipElement> BuildSpecific(HarvestInfo harvest)
        {
            var list = new List<TooltipElement>();

            string harvestId = harvest.HarvestId;
            var (known, unknown) = CookingRecipe.CountRecipesUsing(harvestId);
            var (cooked, uncooked) = CookingRecipe.CountCookedRecipesUsing(harvestId);
            int total = known + unknown;

            if (known == 0 && unknown == 0)
                return list;

            var segments = TooltipBuildHelper.BuildInlineSegmentswithSeparators(
                new[]
                {
                    BuildAchievementSegment(cooked, total),
                    BuildKnownRecipeSegment(harvestId, known, unknown),
                    BuildFridgeQuantity(harvestId)
                },
                x => x
            );


            // add the plate icon + segments
            list.Add(new TooltipElement
            {
                Icon = IconKey.Plate.GetIcon(),
                InlineSegments = segments
            });

            return list;
        }

        //Cooking section using the ingredient category (e.g. Any egg)
        public static List<TooltipElement> BuildGeneric(HarvestInfo harvest)
        {
            var list = new List<TooltipElement>();

            if (harvest.Harvest == null)
                return list;

            int categoryId = harvest.Harvest.Category;

            string category = $"{categoryId}";
            SDVCommonLog.Log($"{category}", LogHelper.DebugOrTrace);

            var (known, unknown) = CookingRecipe.CountRecipesUsing(category);
            var (cooked, uncooked) = CookingRecipe.CountCookedRecipesUsing(category);
            int total = known + unknown;

            if (known == 0 && unknown == 0)
                return list;

            var segments = TooltipBuildHelper.BuildInlineSegmentswithSeparators(
                new[]
                {
                    BuildAchievementSegment(cooked, total),
                    BuildKnownRecipeSegment(category, known, unknown),
                    BuildFridgeQuantitybyCategory(categoryId)
                },
                x => x
            );

            list.Add(new TooltipElement
            {
                //Icon = IconKey.Plate.GetIcon(),
                InlineSegments = segments
            });

            return list;
        }



        private static InlineSegment[] BuildAchievementSegment(int cooked, int total)
        {
            if (cooked == total)
                return Array.Empty<InlineSegment>();

            return new[]
            {
                new InlineSegment
                {
                    Text = $"{cooked}/{total}",
                    TextColor = TooltipColors.Perfection
                }
            };

        }

        private static InlineSegment[] BuildFridgeQuantity(string harvestId)
        {
            int fridgeQty = Inventory.CountOwnedInMainFarmhouseFridges(harvestId);

            var fridgeSegment = new[]
            {
                new InlineSegment
                {
                    Icon = IconRegistry.GetIcon("(BC)216")?.WithScale(1.2f), //MiniFridge
                    Text = string.Format(ModEntry.ModHelper.Translation.Get(TooltipKeys.Owned),
                        fridgeQty)
                }
            };

            return fridgeSegment;
        }

        private static InlineSegment[] BuildFridgeQuantitybyCategory(int categoryId)
        {
            int fridgeQty = Inventory.CountOwnedInMainFarmhouseFridgesByCategory(categoryId);

            var fridgeSegment = new[]
            {
                new InlineSegment
                {
                    Icon = IconRegistry.GetIcon("(BC)216")?.WithScale(1.2f), //MiniFridge
                    Text = string.Format(ModEntry.ModHelper.Translation.Get(TooltipKeys.Owned),
                        fridgeQty)
                }
            };

            return fridgeSegment;
        }




        private static InlineSegment[] BuildKnownRecipeSegment(string ingredientId, int known, int unknown)
        {
            List<InlineSegment> knownUnknownCombined;

            var knownIconSegments = CookingRecipe
                .GetKnownRecipesUsing(ingredientId)
                .Select(r => new InlineSegment
                {
                    Icon = IconRegistry.GetIcon(r.OutputId)
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
            return knownUnknownCombined.ToArray();

        }

    }

}
