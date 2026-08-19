//using SDVData;
//using StardewValley;
//using SDVCommon.Models.Builders;

//namespace SDVCommon.GameData
//{
//    public static class ArtisanRules
//    {
//        public static IEnumerable<ArtisanInfo> GetKnownSpecificRulesUsing(string inputId)
//        {
//            return GetRecipesUsing(ingredientId)
//                .Where(r => IsKnown(r));
//        }

//        public static IEnumerable<CookingInfo> GetUnknownRecipesUsing(string ingredientId)
//        {
//            return GetRecipesUsing(ingredientId)
//                .Where(r => !IsKnown(r));
//        }

//        public static IEnumerable<CookingInfo> GetCookedRecipesUsing(string ingredientId)
//        {
//            return GetRecipesUsing(ingredientId)
//                .Where(r => HasCooked(r));
//        }

//        public static IEnumerable<CookingInfo> GetUncookedRecipesUsing(string ingredientId)
//        {
//            return GetRecipesUsing(ingredientId)
//                .Where(r => !HasCooked(r));
//        }

//        /// <summary>
//        /// Returns all ArtisanInfo entries whose rules include this input item.
//        /// </summary>
//        private static IEnumerable<ArtisanInfo> GetSpecificRulesUsing(string inputId)
//        {
//            return ArtisanInfoBuilder.AllArtisanGoods
//                .Where(r => r.Ingredients.Any(i => i.IngredientId == ingredientId));
//        }

//        private static bool IsMachineKnown(string machineId)
//        {
//            return Game1.player.craftingRecipes.ContainsKey(machineId);
//        }

//        private static bool HasCooked(CookingInfo recipe)
//        {
//            string cookedKey = recipe.OutputId;

//            return Game1.player.recipesCooked.TryGetValue(cookedKey, out int count)
//                   && count > 0;
//        }


//    }
//}

