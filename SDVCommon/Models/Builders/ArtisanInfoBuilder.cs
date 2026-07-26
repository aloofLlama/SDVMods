using SDVCommon.Compatibility;
using SDVCommon.GameData;
using SDVCommon.Helpers;
using SDVCommon.Services;
using SDVData;
using StardewModdingAPI;
using StardewValley;
using StardewValley.GameData.Machines;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SDVCommon.Models.Builders
{
    public static class ArtisanInfoBuilder
    {
        private static readonly Dictionary<string, ArtisanInfo> _artisan = new();

        public static IEnumerable<ArtisanInfo> AllArtisanGoods => _artisan.Values;

        public static void Initialize()
        {
            _artisan.Clear();

            var machines = Game1.content.Load<Dictionary<string, MachineData>>("Data/Machines");

            foreach (var pair in machines)
            {
                string machineId = IdHelper.ToItemId(pair.Key);
                MachineData data = pair.Value;

                if (data.OutputRules == null)
                    continue;

                foreach (var rule in data.OutputRules)
                {
                    if (rule.OutputItem == null)
                        continue;

                    foreach (var output in rule.OutputItem)
                    {
                        var info = Build(machineId, data, rule, output);
                        _artisan[info.OutputId] = info;
                    }
                }
            }


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

        private static ArtisanInfo Build(
            string machineId,
            MachineData machine,
            MachineOutputRule rule,
            MachineItemOutput output)
        {
            string machineName = NameHelper.GetBigCraftableName(machineId);

            // Input info from the trigger
            var trigger = rule.Triggers?.FirstOrDefault();

            string inputQualified = trigger?.RequiredItemId ?? "";
            string inputId = IdHelper.ToItemId(inputQualified);

            string inputName = NameHelper.GetObjectName(inputId);
            int inputCount = trigger?.RequiredCount ?? 0;


            // Output info
            string outputId = IdHelper.ToItemId(output.ItemId);

            string outputName = NameHelper.GetObjectName(outputId);
            int outputCount = ResolveOutputCount(output);


            SDVCommonLog.Log($"Artisan Builder: {machineName} | {inputName} x{inputCount}   →   {outputName} x{outputCount}", LogHelper.DebugInfo);


            return new ArtisanInfo
            {
                MachineId = machineId,
                MachineName = machineName,

                InputId = inputId,
                InputName = NameHelper.GetObjectName(inputId),
                InputCount = inputCount,

                OutputId = outputId,
                OutputName = NameHelper.GetObjectName(outputId),
                OutputCount = outputCount
            };
        }

        private static int ResolveOutputCount(MachineItemOutput output)
        {
            if (output.MinStack > 0)
                return output.MinStack;

            return 1;
        }


        private static List<RecipeIngredient> BuildIngredients(CraftingRecipe recipe)
        {
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

    }
}