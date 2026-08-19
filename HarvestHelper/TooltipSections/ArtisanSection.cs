//using SDVCommon.GameData;
//using SDVCommon.Helpers.Tooltip;
//using SDVCommon.Icons;
//using SDVCommon.Models.Tooltip;
//using SDVData;
//using StardewValley;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace HarvestHelper.TooltipSections
//{
//    internal class ArtisanSection
//    {
//        //Artisan section using the specific ingredient (e.g. not including 'Any fruit')
//        public static List<TooltipElement> BuildSpecific(HarvestInfo harvest)
//        {
//            var list = new List<TooltipElement>();

//            string harvestId = harvest.HarvestId;

//            //var known = CookingRecipe.GetKnownRecipesUsing(harvestId).Count();
//            //var unknown = CookingRecipe.GetUnknownRecipesUsing(harvestId).Count();

//            //int total = known + unknown;

//            //if (known == 0 && unknown == 0)
//            //    return list;

//            var segments = TooltipBuildHelper.BuildInlineSegmentswithSeparators(
//                new[]
//                {
//                    //BuildKnownRecipeSegment(harvestId, known, unknown),
//                },
//                x => x
//            );


//            // add the plate icon + segments
//            list.Add(new TooltipElement
//            {
//                Icon = IconKey.Plate.GetIcon(),
//                InlineSegments = segments
//            });

//            return list;
//        }

//        //Cooking section using the ingredient category (e.g. Any egg)
//        public static List<TooltipElement> BuildGeneric(HarvestInfo harvest)
//        {
//            var list = new List<TooltipElement>();

//            if (harvest.Harvest == null)
//                return list;

//            int categoryId = harvest.Harvest.Category;

//            string category = $"{categoryId}";

//            var known = CookingRecipe.GetKnownRecipesUsing(category).Count();
//            var unknown = CookingRecipe.GetUnknownRecipesUsing(category).Count();
//            var cooked = CookingRecipe.GetCookedRecipesUsing(category).Count();
//            var uncooked = CookingRecipe.GetUncookedRecipesUsing(category).Count();

//            int total = known + unknown;

//            if (known == 0 && unknown == 0)
//                return list;

//            var headingSegment = new[]
//            {
//                new InlineSegment
//                {
//                    Text = "(Any)"
//                }
//            };

//            var segments = TooltipBuildHelper.BuildInlineSegmentswithSeparators(
//                new[]
//                {
//                    headingSegment,
//                    BuildAchievementSegment(cooked, total),
//                    BuildKnownRecipeSegment(category, known, unknown),
//                    BuildFridgeQuantitybyCategory(categoryId)
//                },
//                x => x
//            );

//            list.Add(new TooltipElement
//            {
//                Icon = IconKey.Plate.GetIcon(),
//                InlineSegments = segments
//            });

//            return list;
//        }

//    }
//}
