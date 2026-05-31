using SDVData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static GiftDiscovery.Helpers.DisplayHelper;

namespace GiftDiscovery.Compatibility
{
    public static class ModCompat
    {

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
                    I18n.Get("modsource.adventure_guild"),

                // Crops / cooking / goods / etc
                ModSource.Slimerrain_UncleIrohTea =>
                    I18n.Get("modsource.uncleirohtea"),

                ModSource.Slimerain_GrainsOverhull =>
                    I18n.Get("modsource.grainsoverhull"),

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

        public static class GiftOverrides
        {
            /// <summary>
            /// NPCs that should be treated as NOT giftable,
            /// even if they appear in Utility.getAllCharacters().
            /// </summary>
            public static readonly HashSet<string> NonGiftableNPCs = new()
            {
                // Sunberry Valley placeholders in game data but not used
                "NadiaSBV",
                "PanSBV"

            };
        }



    }
}
