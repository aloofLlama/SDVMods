using SDVCommon.Helpers;
using SDVCommon.Services;
using SDVCommon.Compatibility;
using SDVData;
using StardewModdingAPI;
using StardewValley;

namespace SDVCommon.Models.Builders
{
    public static class CookingInfoBuilder
    {
        private static bool _isInitialized;

        private static readonly Dictionary<string, CookingInfo> _recipes = new();

        public static IEnumerable<CookingInfo> AllRecipes => _recipes.Values;

        public static void Initialize()
        {
            if (_isInitialized)
                return;

            foreach (var pair in CraftingRecipe.cookingRecipes)
            {
                string recipeName = pair.Key;

                var recipe = new CraftingRecipe(recipeName, isCookingRecipe: true);

                var info = Build(recipe);
                _recipes[recipeName] = info;
            }
            _isInitialized = true;

            //foreach (var r in _recipes.Values)
            //{
            //if (r.RecipeName == "HeyKatu.CulinaryDelight_chocolate_cupcake")
            //{ 
            //    foreach (var ing in r.Ingredients)
            //    {
            //        SDVCommonLog.Log(
            //            $"RECIPE {r.RecipeName} uses ingredient {ing.IngredientId}",
            //            LogHelper.DebugOrTrace
            //        );
            //    }
            //}
            //}

        }
        public static void Reset()
        {
            _isInitialized = false;
            _recipes.Clear();
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
                OutputId = IdHelper.ToUnqualifiedItemId(recipe.itemToProduce.First()),
                OutputCount = recipe.numberProducedPerCraft,
                Ingredients = BuildIngredients(recipe)
            };

        }

        private static List<RecipeIngredient> BuildIngredients(CraftingRecipe recipe)
        {
            // 1. SpaceCore override JSON
            //var scOverride = SpaceCoreCompat.GetOverrideIngredients(recipe.name);
            //if (scOverride != null)
            //    return scOverride;

            // 2. Vanilla SDV ingredients
            var ingredients = new List<RecipeIngredient>();

            foreach (var kvp in recipe.recipeList)
            {
                ingredients.Add(new RecipeIngredient
                {
                    IngredientId = kvp.Key,
                    Count = kvp.Value
                });
            }

            return ingredients;
        }

        //public static List<RecipeIngredient> BuildIngredientfs(CraftingRecipe recipe)
        //{

        //    // 1. SpaceCore override JSON
        //    var scOverride = SpaceCoreCompat.GetOverrideIngredients(recipe.name);
        //    if (scOverride != null)
        //    {

        //        return scOverride;
        //    }

        //    // 2. Vanilla SDV ingredients
        //    var ingredients = new List<RecipeIngredient>();

        //    foreach (var kvp in recipe.recipeList)
        //    {
        //        ingredients.Add(new RecipeIngredient
        //        {
        //            IngredientId = kvp.Key,
        //            Count = kvp.Value
        //        });
        //    }

        //    return ingredients;
        //}

    }
}