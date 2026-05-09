//using SDVCommon.Helpers;
//using SDVCommon.Models.Runtime;
//using SDVCommon.Services;
//using StardewModdingAPI;
//using StardewValley;
//using StardewValley.GameData.Characters;

//namespace SDVCommon.Helpers.Specific
//{

//    public static class GiftHelperOLD
//    {
//        public static IEnumerable<NPC> GetLovedBy(Item item)
//        {
//            foreach (var npc in Utility.getAllCharacters())
//            {

//                // Skip monsters, pets, horses, festival NPCs, etc.
//                if (npc is not NPC realNpc)
//                    continue;

//                var giftTastes = Game1.NPCGiftTastes; // Dictionary<string, GiftTaste[]>

//                // Skip NPCs that do not have gift taste data or are overridden to be nongiftable
//                if (!giftTastes.ContainsKey(realNpc.Name) ||
//                    GiftOverrides.NonGiftableNPCs.Contains(realNpc.Name))
//                    continue;

//                GiftTaste? taste = null;

//                try
//                {
//                    taste = (GiftTaste)realNpc.getGiftTasteForThisItem(item);
//                }
//                catch
//                {
//                    // ignored
//                }

//                if (taste == GiftTaste.Love)
//                    yield return realNpc;
//            }
//        }

//        public static IEnumerable<NPC> GetKnownLovedBy(Item item)
//        {
//            string itemId = item.QualifiedItemId;

//            return GetLovedBy(item)
//                .Where(npc =>
//                    GiftKnowledgeServiceOLD.TryGetKnownTaste(itemId, npc.Name, out var taste)
//                    && taste == GiftTaste.Love)
//                .OrderBy(npc => npc.displayName); // alphabetical
//        }

//        public static IEnumerable<NPC> GetUnknownLovedBy(Item item)
//        {
//            string itemId = item.QualifiedItemId;

//            foreach (var npc in GetLovedBy(item))
//            {
//                if (!GiftKnowledgeServiceOLD.TryGetKnownTaste(itemId, npc.Name, out var taste)
//                    || taste != GiftTaste.Love)
//                    yield return npc;
//            }
//        }

//        internal static class GiftOverrides
//        {
//            /// <summary>
//            /// NPCs that should be treated as NOT giftable,
//            /// even if they appear in Utility.getAllCharacters().
//            /// </summary>
//            public static readonly HashSet<string> NonGiftableNPCs = new()
//        {
//            // Sunberry Valley placeholders in game data but not used
//            "NadiaSBV",
//            "PanSBV"

//        };
//        }

//        public static bool IsMaxHearts(NPC npc)
//        {
//            // get friendship entry
//            if (!Game1.player.friendshipData.TryGetValue(npc.Name, out Friendship f))
//                return false;

//            int maxHearts = GetMaxHearts(npc);
//            int maxPoints = maxHearts * 250;

//            return f.Points >= maxPoints;
//        }



//        public static int GetMaxHearts(NPC npc)
//        {
//            // cannot socialize → 0
//            if (!npc.CanSocialize)
//                return 0;

//            // get friendship entry
//            if (!Game1.player.friendshipData.TryGetValue(npc.Name, out Friendship f))
//                return 0;

//            // marriage and roommate → 14 (roommate is marrier + a roommatemarriage flag)
//            if (f.Status == FriendshipStatus.Married)
//                return 14;

//            // dating or engaged → 10
//            if (f.Status == FriendshipStatus.Dating || f.Status == FriendshipStatus.Engaged)
//                return 10;

//            // romanceable but not dating yet → 10
//            if (npc.GetData()?.CanBeRomanced == true)
//                return 8;

//            // everyone else
//            return 10;
//        }

//    }
//}
