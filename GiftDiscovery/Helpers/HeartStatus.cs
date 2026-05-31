using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GiftDiscovery.Helpers
{
    internal class HeartStatus
    {
        public static bool IsMaxHearts(NPC npc)
        {
            // get friendship entry
            if (!Game1.player.friendshipData.TryGetValue(npc.Name, out Friendship f))
                return false;

            int maxHearts = GetMaxHearts(npc);
            int maxPoints = maxHearts * 250;

            return f.Points >= maxPoints;
        }

        public static int GetCurrentHearts(NPC npc)
        {
            if (!Game1.player.friendshipData.TryGetValue(npc.Name, out Friendship f))
                return 0;

            return f.Points / 250;
        }

        public static int GetMaxHearts(NPC npc)
        {
            // cannot socialize → 0
            if (!npc.CanSocialize)
                return 0;

            // get friendship entry
            if (!Game1.player.friendshipData.TryGetValue(npc.Name, out Friendship f))
                return 0;

            // marriage and roommate → 14 (roommate is married + a roommate marriage flag)
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
