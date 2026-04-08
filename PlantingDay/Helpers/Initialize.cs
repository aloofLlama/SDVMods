using PlantingDay.Compatibility;
using PlantingDay.Helpers;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Framework.ModLoading;
using StardewValley;
using StardewValley.GameData.Shops;
using StardewValley.Internal;
using StardewValley.Menus;
using System.Runtime.CompilerServices;
using static PlantingDay.Helpers.SeedSourceAggregator;



namespace PlantingDay.Helpers
{
    public class Initialize
    {
        public static void ForRuntime(IModHelper helper)
        {
            IContentHelper content = helper.Content;

            TooltipIcons.Initialize();
            PlantDatabase.Initialize(content);
            MonsterDropLoader.Initialize();

            foreach (var plant in PlantDatabase.AllPlants)
            {
                SeedSourceAggregator.AddSeedSourcesToPlant(plant);
                IconRenderer_plants.InitializeIcons(plant);
            }
        }

        public static void ForTests(IContentHelper content)
        {
            PlantDatabase.Initialize(content);
            MonsterDropLoader.Initialize();

            foreach (var plant in PlantDatabase.AllPlants)
            {
                SeedSourceAggregator.AddSeedSourcesToPlant(plant);
                // no icons in tests
            }
        }



    }
}
