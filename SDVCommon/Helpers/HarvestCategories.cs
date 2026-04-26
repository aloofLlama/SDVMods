using System;
using System.Collections.Generic;
using System.Text;
using StardewValley.GameData.Objects;
using SObject = StardewValley.Object;


namespace SDVCommon.Helpers
{
    internal class HarvestCategories
    {
        public static bool IsDesiredCategory(ObjectData obj)
        {
            // Ignore items with no sell price
            if (obj.Price <= 0)
                return false;

            int category = obj.Category;

            // CATEGORY-BASED MATCHES
            if (category is
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

                or SObject.mineralsCategory
                or SObject.meatCategory
                or SObject.metalResources
                or SObject.buildingResources

                or SObject.fertilizerCategory
                or SObject.junkCategory
                or SObject.baitCategory
                or SObject.tackleCategory
                or SObject.sellAtFishShopCategory //coral, etc

                or SObject.artisanGoodsCategory
                or SObject.syrupCategory
                or SObject.monsterLootCategory

                or SObject.trinketCategory
                or SObject.booksCategory
                or SObject.skillBooksCategory

                or SObject.sellAtPierres
                or SObject.sellAtPierresAndMarnies)
                return true;

            // CATEGORY 0 SPECIAL CASES
            if (category == 0)
            {
               if (obj.Type == "Arch" || // Artifacts (Bone Flute, Ancient Doll, Golden Mask, etc.)
                    obj.Type == "Basic" || //vinegar, pearl, etc
                    obj.Type == "Crafting" ||   //coffee, field snack
                    obj.Type == "0" //SDV Expanded items e.g. trasurechest, grampleton chicken
                    )
                    return true;
            }    
            
            // SPECIAL CASE: acorn, maple seed
            if (obj.Type == "Crafting" && category == SObject.SeedsCategory)
                return true;


            return false;
        }

    }
}
