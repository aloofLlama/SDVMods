using SDVCommon.Helpers;
using SDVCommon.Models.Runtime;
using SDVCommon.Models.Wrappers;
using SDVData;
using StardewModdingAPI;
using StardewValley;

namespace SDVCommon.Models.Builders
{
    public static class CookingInfoBuilder
    {
        private static readonly Dictionary<string, CookingInfo> _recipes = new();

        public static IEnumerable<CookingInfo> AllRecipes => _recipes.Values;

        public static void BuildAll()
        {
            _recipes.Clear();

            foreach (var pair in CraftingRecipe.cookingRecipes)
            {
                string recipeName = pair.Key;

                var recipe = new CraftingRecipe(recipeName, isCookingRecipe: true);

                var info = Build(recipe);
                _recipes[recipeName] = info;
            }

            //foreach (var r in _recipes.Values)
            //{
            //    foreach (var ing in r.Data.Ingredients)
            //    {
            //        SDVCommonLog.Log(
            //            $"RECIPE {r.Data.RecipeName} uses ingredient {ing.IngredientId}",
            //            LogLevel.Info
            //        );
            //    }
            //}

        }

        public static CookingInfo? Lookup(string recipeName)
        {
            _recipes.TryGetValue(recipeName, out var info);
            return info;
        }

        private static CookingInfo Build(CraftingRecipe recipe)
        {
            return new CookingInfo
            {
                RecipeName = recipe.name,
                OutputDisplayName = recipe.DisplayName,
                OutputId = IdHelper.ToItemId(recipe.itemToProduce.First()),
                OutputCount = recipe.numberProducedPerCraft,
                Ingredients = recipe.recipeList
                    .Select(kvp => new RecipeIngredient
                    {
                        IngredientId = IdHelper.ToItemId(kvp.Key),
                        Count = kvp.Value
                    })
                    .ToList()
            };

        }
    }
}