using GenericModConfigMenu;
using GiftDiscovery.Models;
using StardewModdingAPI;

namespace GiftDiscovery.Config
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

            gmcm.AddKeybind(
                mod: manifest,
                name: () => "Toggle Tooltip",
                tooltip: () => "Show or hide the gift tooltip.",
                getValue: () => ModEntry.ModConfig.ToggleTooltipKey,
                setValue: value => ModEntry.ModConfig.ToggleTooltipKey = value
            );

            gmcm.AddTextOption(
                mod: manifest,
                name: () => "Taste Source Mode",
                tooltip: () => "Choose whether to show all tastes, global learned tastes across all farms, or local learned tastes for this farm only.",
                getValue: () => ModEntry.ModConfig.TasteSourceMode.ToString(),
                setValue: value => ModEntry.ModConfig.TasteSourceMode = Enum.Parse<TasteSourceMode>(value),
                allowedValues: Enum.GetNames(typeof(TasteSourceMode)),
                formatAllowedValue: v => v
            );

            gmcm.AddBoolOption(
                mod: manifest,
                name: () => "Highlight Friendship",
                tooltip: () => "If enabled, NPCs who are not at max hearts will be highlighted in the tooltip.",
                getValue: () => ModEntry.ModConfig.HighlightNotMaxFriendship,
                setValue: value => ModEntry.ModConfig.HighlightNotMaxFriendship = value
            );

            gmcm.AddBoolOption(
                mod: manifest,
                name: () => "Deemphasize Already Gifted",
                tooltip: () => "If enabled, NPCs who can not receive a gift today will be deemphasized.",
                getValue: () => ModEntry.ModConfig.DeemphasizeAlreadyGifted,
                setValue: value => ModEntry.ModConfig.DeemphasizeAlreadyGifted = value
            );

            gmcm.AddBoolOption(
                mod: manifest,
                name: () => "Emphasize Nearby NPCs",
                tooltip: () => "If enabled, NPCs within range of the player will be emphasized in the tooltip.",
                getValue: () => ModEntry.ModConfig.EmphasizeNearbyNPCs,
                setValue: value => ModEntry.ModConfig.EmphasizeNearbyNPCs = value
            );

            gmcm.AddNumberOption(
                mod: manifest,
                name: () => "Nearby NPC Range (tiles)",
                tooltip: () => "NPCs within this many tiles of the player will be emphasized.",
                getValue: () => ModEntry.ModConfig.NearbyRangeTilesGiftTooltip,
                setValue: value => ModEntry.ModConfig.NearbyRangeTilesGiftTooltip = value,
                min: 1,
                max: 20
            );

            gmcm.AddNumberOption(
                mod: manifest,
                name: () => "Line Wrapping",
                tooltip: () => "Wrap the list to the next line after this many names.",
                getValue: () => ModEntry.ModConfig.WrapSizeGift,
                setValue: value => ModEntry.ModConfig.WrapSizeGift = value,
                min: 3,
                max: 6
            );


            gmcm.AddBoolOption(
                mod: manifest,
                name: () => "Show Loves",
                tooltip: () => "Show NPCs who love this item.",
                getValue: () => ModEntry.ModConfig.ShowLoves,
                setValue: value => ModEntry.ModConfig.ShowLoves = value
            );

            gmcm.AddBoolOption(
                mod: manifest,
                name: () => "Show Likes",
                tooltip: () => "Show NPCs who like this item.",
                getValue: () => ModEntry.ModConfig.ShowLikes,
                setValue: value => ModEntry.ModConfig.ShowLikes = value
            );

            gmcm.AddBoolOption(
                mod: manifest,
                name: () => "Show Neutral",
                tooltip: () => "Show NPCs who feel neutral about this item.",
                getValue: () => ModEntry.ModConfig.ShowNeutral,
                setValue: value => ModEntry.ModConfig.ShowNeutral = value
            );

            gmcm.AddBoolOption(
                mod: manifest,
                name: () => "Show Dislikes",
                tooltip: () => "Show NPCs who dislike this item.",
                getValue: () => ModEntry.ModConfig.ShowDislikes,
                setValue: value => ModEntry.ModConfig.ShowDislikes = value
            );

            gmcm.AddBoolOption(
                mod: manifest,
                name: () => "Show Hates",
                tooltip: () => "Show NPCs who hate this item.",
                getValue: () => ModEntry.ModConfig.ShowHates,
                setValue: value => ModEntry.ModConfig.ShowHates = value
            );

            gmcm.AddBoolOption(
                mod: manifest,
                name: () => "Show Undiscovered",
                tooltip: () => "Show NPCs whose gift tastes for this item are unknown.",
                getValue: () => ModEntry.ModConfig.ShowUndiscovered,
                setValue: value => ModEntry.ModConfig.ShowUndiscovered = value
            );

            //-------------------
            //NPC tooltip bottom right
            //-------------------

            gmcm.AddNumberOption(
                mod: manifest,
                name: () => "NPC Tooltip Range (tiles)",
                tooltip: () => "NPC's gift tastes will be shown when standing this close to the NPC",
                getValue: () => ModEntry.ModConfig.NearbyRangeTilesNPCTooltip,
                setValue: value => ModEntry.ModConfig.NearbyRangeTilesNPCTooltip = value,
                min: 1,
                max: 3
            );

            gmcm.AddNumberOption(
                mod: manifest,
                name: () => "Line Wrapping",
                tooltip: () => "Wrap the list to the next line after this many names.",
                getValue: () => ModEntry.ModConfig.WrapSizeNPC,
                setValue: value => ModEntry.ModConfig.WrapSizeNPC = value,
                min: 3,
                max: 20
                );



        }
    }
}
