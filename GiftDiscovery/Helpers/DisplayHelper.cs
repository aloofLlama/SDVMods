using GiftDiscovery.GameData;
using GiftDiscovery.Models;
using GiftDiscovery.Models.Builders;
using Microsoft.Xna.Framework;
using SDVCommon.Helpers.Tooltip;
using SDVData;
using StardewValley;


namespace GiftDiscovery.Helpers
{
    public class DisplayHelper
    {
        public static class I18n
        {
            public static string Get(string key)
                => ModEntry.ModHelper.Translation.Get(key) ?? key;
        }

        public static Color GetNPCNameColor(NPC npc)
        {
            NPCGiftStatus status = NPCGiftStatusBuilder.GiftStatus(npc);

            if (ModEntry.ModConfig.DeemphasizeAlreadyGifted &&
                !status.CanGiftToday)
                return TooltipColors.Muted;

            if (ModEntry.ModConfig.HighlightNotMaxFriendship)
                return status.IsMaxHeart ? TooltipColors.Normal : TooltipColors.Perfection;

            return TooltipColors.Normal;
        }

        public static string? ModToText(ModSource source)
        {
            return source switch
            {
                ModSource.Stardew => null,

                ModSource.Unknown =>
                    I18n.Get("modsource.unknown"),

                // Expansions
                ModSource.Sunberry =>
                    I18n.Get("modsource.sunberry"),

                ModSource.Expanded =>
                    I18n.Get("modsource.expanded"),

                ModSource.AdventureGuild =>
                    I18n.Get("modsource.adventureguild"),

                // Crops / cooking / goods / etc
                ModSource.Slimerrain_UncleIrohTea =>
                    I18n.Get("modsource.slimerrain_uncleirohtea"),

                ModSource.Slimerain_GrainsOverhull =>
                    I18n.Get("modsource.slimerrain_grainsoverhull"),

                ModSource.Cornucopia =>
                    I18n.Get("modsource.cornucopia"),

                ModSource.CulinaryDelight =>
                    I18n.Get("modsource.culinarydelight"),

                ModSource.SimpleTea =>
                    I18n.Get("modsource.simpletea"),

                ModSource.MoreNewFish =>
                    I18n.Get("modsource.morenewfish"),

                ModSource.NatureInTheValley =>
                    I18n.Get("modsource.natureinthevalley"),

                _ =>
                    I18n.Get("modsource.unknown")
            };
        }
    }
}
    



