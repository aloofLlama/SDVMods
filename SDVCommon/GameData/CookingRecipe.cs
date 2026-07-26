using SDVData;
using StardewValley;
using SDVCommon.Models.Builders;

namespace SDVCommon.GameData
{
    public static class CookingRecipe
    {
        public static IEnumerable<CookingInfo> GetKnownRecipesUsing(string ingredientId)
        {
            return GetRecipesUsing(ingredientId)
                .Where(r => IsKnown(r));
        }

        public static IEnumerable<CookingInfo> GetUnknownRecipesUsing(string ingredientId)
        {
            return GetRecipesUsing(ingredientId)
                .Where(r => !IsKnown(r));
        }

        public static IEnumerable<CookingInfo> GetCookedRecipesUsing(string ingredientId)
        {
            return GetRecipesUsing(ingredientId)
                .Where(r => HasCooked(r));
        }

        public static IEnumerable<CookingInfo> GetUncookedRecipesUsing(string ingredientId)
        {
            return GetRecipesUsing(ingredientId)
                .Where(r => !HasCooked(r));
        }

        /// <summary>
        /// Returns all CookingInfo entries whose ingredients include this ingredient.
        /// </summary>
        private static IEnumerable<CookingInfo> GetRecipesUsing(string ingredientId)
        {
            return CookingInfoBuilder.AllRecipes
                .Where(r => r.Ingredients.Any(i => i.IngredientId == ingredientId));
        }

        private static bool IsKnown(CookingInfo recipe)
        {
            return Game1.player.cookingRecipes.ContainsKey(recipe.RecipeName);
        }

        private static bool HasCooked(CookingInfo recipe)
        {
            string cookedKey = recipe.OutputId;

            return Game1.player.recipesCooked.TryGetValue(cookedKey, out int count)
                   && count > 0;
        }


    }
}

