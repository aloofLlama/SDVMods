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

    }
}
    



