using SDVData;
using StardewValley;
using SDVCommon.Models.Builders;

namespace SDVCommon.GameData
{
    public static class CookingRecipe
    {
        /// <summary>
        /// Returns all CookingInfo entries whose ingredients include this ingredient.
        /// </summary>
        public static IEnumerable<CookingInfo> GetRecipesUsing(string ingredientId)
        {
            return CookingInfoBuilder.AllRecipes
                .Where(r => r.Ingredients.Any(i => i.IngredientId == ingredientId));
        }

        public static (int known, int unknown) CountRecipesUsing(string ingredientId)
        {
            int known = GetKnownRecipesUsing(ingredientId).Count();
            int unknown = GetUnknownRecipesUsing(ingredientId).Count();
            return (known, unknown);
        }

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

        public static bool IsKnown(CookingInfo recipe)
        {
            return Game1.player.cookingRecipes.ContainsKey(recipe.RecipeName);
        }

        public static bool HasCooked(CookingInfo recipe)
        {
            string cookedKey = recipe.OutputId; // e.g. "612"

            return Game1.player.recipesCooked.TryGetValue(cookedKey, out int count)
                   && count > 0;
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

        public static (int cooked, int uncooked) CountCookedRecipesUsing(string ingredientId)
        {
            int cooked = GetCookedRecipesUsing(ingredientId).Count();
            int uncooked = GetUncookedRecipesUsing(ingredientId).Count();
            return (cooked, uncooked);
        }


    }
}

