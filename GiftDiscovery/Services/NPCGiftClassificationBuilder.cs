using GiftDiscovery.Compatibility;
using GiftDiscovery.Helpers;
using GiftDiscovery.Models;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GiftDiscovery.Services
{
    internal class NpcGiftClassificationBuilder
    {
        public static NpcGiftClassification Classify(NPC npc)
        {
            var name = npc.Name;

            bool isOverrideBlocked = ModCompat.GiftOverrides.NonGiftableNPCs.Contains(name);
            bool hasGiftTastes = Game1.NPCGiftTastes.ContainsKey(name);

            bool isGiftable = hasGiftTastes && !isOverrideBlocked;

            bool isAvailable =
                isGiftable &&
                npc.CanSocialize &&
                npc.CanReceiveGifts() &&
                npc.currentLocation != null;

            bool isMet =
                isAvailable &&
                Game1.player.friendshipData.ContainsKey(name);

            bool canGiftToday = isAvailable && !MaxGiftsReached(npc);
            bool isMaxHeart = isAvailable && isMet && IsMaxHearts(npc);

            return new NpcGiftClassification
            {
                Npc = npc,
                Name = name,
                IsGiftable = isGiftable,
                IsAvailable = isAvailable,
                IsMet = isMet,
                CanGiftToday = canGiftToday,
                IsMaxHeart = isMaxHeart
            };
        }

        private static bool MaxGiftsReached(NPC npc)
        {
            if (!Game1.player.friendshipData.TryGetValue(npc.Name, out var f))
                return false;

            // Daily limit (everyone)
            if (f.GiftsToday >= 1)
                return true;

            // Weekly limit bypass for spouse or birthday
            bool isRealSpouse = f.IsMarried() && !f.RoommateMarriage;
            bool isBirthday = npc.isBirthday();

            if (isRealSpouse || isBirthday)
                return false;

            // Weekly limit (everyone else)
            if (f.GiftsThisWeek >= 2)
                return true;

            return false;
        }

        private static bool IsMaxHearts(NPC npc)
        {
            // get friendship entry
            if (!Game1.player.friendshipData.TryGetValue(npc.Name, out Friendship f))
                return false;

            int maxHearts = GetMaxHearts(npc);
            int maxPoints = maxHearts * 250;

            return f.Points >= maxPoints;
        }

        private static int GetMaxHearts(NPC npc)
        {
            // cannot socialize → 0
            if (!npc.CanSocialize)
                return 0;

            // get friendship entry
            if (!Game1.player.friendshipData.TryGetValue(npc.Name, out Friendship f))
                return 0;

            // marriage and roommate → 14 (roommate is marrier + a roommatemarriage flag)
            if (f.Status == FriendshipStatus.Married)
                return 14;

            // dating or engaged → 10
            if (f.Status == FriendshipStatus.Dating || f.Status == FriendshipStatus.Engaged)
                return 10;

            // romanceable but not dating yet → 10
            if (npc.GetData()?.CanBeRomanced == true)
                return 8;

            // everyone else
            return 10;
        }


    }
}
