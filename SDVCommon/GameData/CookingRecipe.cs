using SDVData;
using StardewValley;
using SDVCommon.Models.Builders;
using SDVCommon.Helpers;

namespace SDVCommon.GameData
{
    public static class CookingRecipe
    {
        public static IEnumerable<CookingInfo> GetKnownRecipesUsing(string ingredientQId)
        {
            return GetRecipesUsing(ingredientQId)
                .Where(r => IsKnown(r));
        }

        public static IEnumerable<CookingInfo> GetUnknownRecipesUsing(string ingredientQId)
        {
            return GetRecipesUsing(ingredientQId)
                .Where(r => !IsKnown(r));
        }

        public static IEnumerable<CookingInfo> GetCookedRecipesUsing(string ingredientQId)
        {
            return GetRecipesUsing(ingredientQId)
                .Where(r => HasCooked(r));
        }

        public static IEnumerable<CookingInfo> GetUncookedRecipesUsing(string ingredientQId)
        {
            return GetRecipesUsing(ingredientQId)
                .Where(r => !HasCooked(r));
        }

        /// <summary>
        /// Returns all CookingInfo entries whose ingredients include this ingredient.
        /// </summary>
        private static IEnumerable<CookingInfo> GetRecipesUsing(string ingredientQId)
        {
            string unqualifiedIngredientId = IdHelper.ToUnqualifiedItemId(ingredientQId);

            return CookingInfoBuilder.AllRecipes
                .Where(r => r.Ingredients.Any(i => i.IngredientId == unqualifiedIngredientId));
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

