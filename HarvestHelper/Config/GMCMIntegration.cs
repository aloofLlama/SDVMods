using GenericModConfigMenu;
using StardewModdingAPI;
using SDVCommon.Helpers;

namespace HarvestHelper.Config
{
    internal static class GMCMIntegration
    {
        public static void Register(IModHelper helper, IManifest manifest)
        {
            var gmcm = helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (gmcm == null)
                return;


            gmcm.Register(
                mod: manifest,
                reset: () =>
                {
                    ModEntry.ModConfig = new ModConfig();
                    helper.WriteConfig(ModEntry.ModConfig);
                },
                save: () =>
                {
                    helper.WriteConfig(ModEntry.ModConfig);
                }
            );

            gmcm.AddSectionTitle(
                ModEntry.Instance.ModManifest,
                () => "Cooking"
                );

            gmcm.AddParagraph(
                ModEntry.Instance.ModManifest,
                () => "Price thresholds for displaying cooking for 'item (Any)' recipes. Items above the " +
                "threshold will not show the cooking section."
                );

            gmcm.AddParagraph(
                ModEntry.Instance.ModManifest,
                () => "Select '-10' for default set to a representative item."
                );


            gmcm.AddParagraph(
    ModEntry.Instance.ModManifest,
                () => "Select '0' to hide all the 'item (Any)' recipes"
                );


            gmcm.AddNumberOption(
                mod: manifest,
                name: () => "'Any Fish' price threshold",
                tooltip: () => "Default is silver qualify snail.",
                getValue: () => ModEntry.ModConfig.AnyFishPriceThreshold,
                setValue: value => ModEntry.ModConfig.AnyFishPriceThreshold = value,
                min: -10,
                max: 1000,
                interval: 10
            );

            gmcm.AddNumberOption(
                mod: manifest,
                name: () => "'Any Egg' price threshold",
                tooltip: () => "Default is iridium large egg.",
                getValue: () => ModEntry.ModConfig.AnyEggPriceThreshold,
                setValue: value => ModEntry.ModConfig.AnyEggPriceThreshold = value,
                min: -10,
                max: 1000,
                interval: 10
            );

            gmcm.AddNumberOption(
                mod: manifest,
                name: () => "'Any Milk' price threshold",
                tooltip: () => "Default is iridium large milk.",
                getValue: () => ModEntry.ModConfig.AnyMilkPriceThreshold,
                setValue: value => ModEntry.ModConfig.AnyMilkPriceThreshold = value,
                min: -10,
                max: 1000,
                interval: 10
            );

            gmcm.AddNumberOption(
                mod: manifest,
                name: () => "'Any Vegetable' price threshold",
                tooltip: () => "Default is gold eggplant.",
                getValue: () => ModEntry.ModConfig.AnyVeggiePriceThreshold,
                setValue: value => ModEntry.ModConfig.AnyVeggiePriceThreshold = value,
                min: -10,
                max: 1000,
                interval: 10
            );

            gmcm.AddNumberOption(
                mod: manifest,
                name: () => "'Any Fruit' price threshold",
                tooltip: () => "Default is gold cranberry.",
                getValue: () => ModEntry.ModConfig.AnyFruitPriceThreshold,
                setValue: value => ModEntry.ModConfig.AnyFruitPriceThreshold = value,
                min: -10,
                max: 1000,
                interval: 10
            );


            //gmcm.AddBoolOption(
            //    mod: manifest,
            //    name: () => "Show Loves",
            //    tooltip: () => "Show NPCs who love this item.",
            //    getValue: () => ModEntry.ModConfig.ShowLoves,
            //    setValue: value => ModEntry.ModConfig.ShowLoves = value
            //);



        }
    }
}
