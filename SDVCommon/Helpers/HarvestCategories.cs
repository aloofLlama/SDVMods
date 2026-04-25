using System;
using System.Collections.Generic;
using System.Text;
using SObject = StardewValley.Object;


namespace SDVCommon.Helpers
{
    internal class HarvestCategories
    {
        public static bool IsDesiredCategory(int category)
        {
            return category is
                SObject.FruitsCategory
                or SObject.VegetableCategory
                or SObject.flowersCategory
                or SObject.GreensCategory
                or SObject.GemCategory

                or SObject.FishCategory
                or SObject.EggCategory
                or SObject.MilkCategory
                or SObject.CookingCategory
                or SObject.CraftingCategory

                //or SObject.SeedsCategory
                or SObject.mineralsCategory
                or SObject.meatCategory
                or SObject.metalResources
                or SObject.buildingResources

                or SObject.fertilizerCategory
                or SObject.junkCategory
                or SObject.baitCategory
                or SObject.tackleCategory

                //or SObject.ingredientsCategory
                or SObject.artisanGoodsCategory
                or SObject.syrupCategory
                or SObject.monsterLootCategory

                or SObject.trinketCategory
                or SObject.booksCategory
                or SObject.skillBooksCategory

                or SObject.sellAtPierres
                or SObject.sellAtPierresAndMarnies;
        }

    }
}
