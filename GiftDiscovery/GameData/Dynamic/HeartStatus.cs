using StardewValley;

namespace GiftDiscovery.GameData.Dynamic
{
    internal class HeartStatus
    {
        internal static bool IsMaxHearts(NPC npc)
        {
            int currentHearts = GetCurrentHearts(npc);
            int maxHearts = GetMaxHearts(npc);

            return currentHearts >= maxHearts;
        }

        internal static int GetCurrentHearts(NPC npc)
        {
            if (!Game1.player.friendshipData.TryGetValue(npc.Name, out Friendship f))
                return 0;

            return f.Points / 250;
        }

        internal static int GetMaxHearts(NPC npc)
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
