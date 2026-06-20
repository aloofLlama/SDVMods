using GiftDiscovery.Compatibility;
using GiftDiscovery.Models;
using GiftDiscovery.Helpers;
using StardewValley;

namespace GiftDiscovery.Models.Builders
{
    internal class NPCGiftStatusBuilder
    {
        public static NPCGiftStatus GiftStatus(NPC npc)
        {
            var name = npc.Name;

            //bool isOverrideBlocked = ModCompat.GiftOverrides.NonGiftableNPCs.Contains(name);
            //bool hasGiftTastes = Game1.NPCGiftTastes.ContainsKey(name);

            //bool isGiftable = hasGiftTastes && !isOverrideBlocked;

            bool isAvailable =
                //isGiftable &&
                npc.CanSocialize &&
                npc.CanReceiveGifts() &&
                npc.currentLocation != null;

            bool isMet =
                isAvailable &&
                Game1.player.friendshipData.ContainsKey(name);

            bool canGiftToday = isAvailable && !MaxGiftsReached(npc);
            bool isMaxHeart = isAvailable && isMet && HeartStatus.IsMaxHearts(npc);

            return new NPCGiftStatus
            {
                NPC = npc,
                Name = name,
                //IsGiftable = isGiftable,
                IsAvailable = isAvailable,
                IsMet = isMet,
                CanGiftToday = canGiftToday,
                IsMaxHeart = isMaxHeart
            };
        }

        public static bool IsUnmetNPC(string npcName)
        {
            var npc = Utility.getAllCharacters().FirstOrDefault(n => n.Name == npcName);
            if (npc == null)
                return false;

            var c = NPCGiftStatusBuilder.GiftStatus(npc);
            return c.IsUnmet;
        }

        private static bool MaxGiftsReached(NPC npc)
        {
            if (!Game1.player.friendshipData.TryGetValue(npc.Name, out var f))
                return false;

            // Daily limit (everyone)
            if (f.GiftsToday >= 1)
                return true;

            // Weekly limit bypass for spouse, roommate or birthday
            bool isMovedIn = f.IsMarried(); //roommate is also flagged as married
            bool isBirthday = npc.isBirthday();

            if (isMovedIn || isBirthday)
                return false;

            // Weekly limit (everyone else)
            if (f.GiftsThisWeek >= 2)
                return true;

            return false;
        }



    }
}
