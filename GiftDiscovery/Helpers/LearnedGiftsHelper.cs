using GiftDiscovery.GameData;
using GiftDiscovery.Models;
using GiftDiscovery.Models.Builders;
using GiftDiscovery.Services;
using StardewValley;

namespace GiftDiscovery.Helpers
{
    internal static class LearnedGiftsHelper
    {
        // ---------------------------------------------------------
        // ITEM → NPC (Known)
        // ---------------------------------------------------------
        public static IEnumerable<NPC> GetKnownFor(
            string qualifiedItemId,
            GiftTaste taste,
            TasteSourceMode mode)
        {
            return GiftableNPC.GetAllGiftableNPCs()
                .Select(NPCGiftStatusBuilder.GiftStatus)
                .Where(c => c.IsAvailable)
                .Where(c =>
                {
                    var canonical = TasteResolver.GetCanonicalTaste(qualifiedItemId, c.NPC);
                    if (canonical != taste)
                        return false;

                    return TasteResolver.IsKnown(qualifiedItemId, c.NPC, mode);
                })
                .Select(c => c.NPC)
                .OrderBy(npc => npc.displayName);
        }

        // ---------------------------------------------------------
        // ITEM → NPC (Unknown)
        // ---------------------------------------------------------
        public static IEnumerable<NPC> GetUnknownFor(
            string qualifiedItemId,
            GiftTaste taste,
            TasteSourceMode mode)
        {
            if (mode == TasteSourceMode.All)
                yield break;

            foreach (var c in GiftableNPC.GetAllGiftableNPCs()
                .Select(NPCGiftStatusBuilder.GiftStatus))
            {
                var canonical = TasteResolver.GetCanonicalTaste(qualifiedItemId, c.NPC);
                if (canonical != taste)
                    continue;

                if (TasteResolver.IsUnknown(qualifiedItemId, c.NPC, mode))
                    yield return c.NPC;
            }
        }

        // ---------------------------------------------------------
        // ITEM → NPC (Undiscovered, all tastes)
        // ---------------------------------------------------------
        public static IEnumerable<NPC> GetUndiscoveredBy(
            string qualifiedItemId,
            TasteSourceMode mode)
        {
            if (mode == TasteSourceMode.All)
                yield break;

            foreach (var c in GiftableNPC.GetAllGiftableNPCs()
                .Select(NPCGiftStatusBuilder.GiftStatus))
            {
                var canonical = TasteResolver.GetCanonicalTaste(qualifiedItemId, c.NPC);
                if (canonical == null)
                    continue;

                if (TasteResolver.IsUnknown(qualifiedItemId, c.NPC, mode))
                    yield return c.NPC;
            }
        }

        // ---------------------------------------------------------
        // NPC → ITEM (Known)
        // ---------------------------------------------------------
        public static IEnumerable<string> GetKnownGiftsForNPC(
            NPC npc,
            GiftTaste taste,
            TasteSourceMode mode)
        {
            foreach (var obj in GiftableObjectList.AllGiftable)
            {
                string id = obj.QualifiedItemId;

                var canonical = TasteResolver.GetCanonicalTaste(id, npc);
                if (canonical != taste)
                    continue;

                if (TasteResolver.IsKnown(id, npc, mode))
                    yield return id;
            }
        }

        // ---------------------------------------------------------
        // NPC → ITEM (Unknown)
        // ---------------------------------------------------------
        public static IEnumerable<string> GetUnknownGiftsForNPC(
            NPC npc,
            GiftTaste taste,
            TasteSourceMode mode)
        {
            if (mode == TasteSourceMode.All)
                yield break;

            foreach (var obj in GiftableObjectList.AllGiftable)
            {
                string id = obj.QualifiedItemId;

                var canonical = TasteResolver.GetCanonicalTaste(id, npc);
                if (canonical != taste)
                    continue;

                if (TasteResolver.IsUnknown(id, npc, mode))
                    yield return id;
            }
        }

        // ---------------------------------------------------------
        // Has the player discovered ALL Loves + Likes for this item?
        // ---------------------------------------------------------
        public static bool HasDiscoveredAllLovesLikesforItem(
            string qualifiedItemId,
            TasteSourceMode mode)
        {
            if (mode == TasteSourceMode.All)
                return true;

            int canonicalTotal =
                GetKnownFor(qualifiedItemId, GiftTaste.Love, TasteSourceMode.All).Count()
                + GetKnownFor(qualifiedItemId, GiftTaste.Like, TasteSourceMode.All).Count();

            int discoveredTotal =
                GetKnownFor(qualifiedItemId, GiftTaste.Love, mode).Count()
                + GetKnownFor(qualifiedItemId, GiftTaste.Like, mode).Count();

            return discoveredTotal >= canonicalTotal;
        }

        // ---------------------------------------------------------
        // Has the player discovered ALL Loves + Likes for this NPC?
        // ---------------------------------------------------------
        public static bool HasDiscoveredAllLovesLikesForNPC(
            NPC npc,
            TasteSourceMode mode)
        {
            if (mode == TasteSourceMode.All)
                return true;

            // canonical totals (LOVE + LIKE)
            int canonicalTotal =
                LearnedGiftsHelper.GetKnownGiftsForNPC(npc, GiftTaste.Love, TasteSourceMode.All).Count()
                + LearnedGiftsHelper.GetKnownGiftsForNPC(npc, GiftTaste.Like, TasteSourceMode.All).Count();

            // discovered totals (LOVE + LIKE)
            int discoveredTotal =
                LearnedGiftsHelper.GetKnownGiftsForNPC(npc, GiftTaste.Love, mode).Count()
                + LearnedGiftsHelper.GetKnownGiftsForNPC(npc, GiftTaste.Like, mode).Count();

            return discoveredTotal >= canonicalTotal;
        }

    }
}



//using GiftDiscovery.GameData;
//using GiftDiscovery.Models;
//using GiftDiscovery.Models.Builders;
//using GiftDiscovery.Services;
//using StardewValley;

//namespace GiftDiscovery.Helpers
//{
//    internal class LearnedGiftsHelper
//    {
//        // The player knows this gift's taste for which NPCs?
//        // E.g. Who do I know loves parsnips?
//        public static IEnumerable<NPC> GetKnownFor(
//            StardewValley.Object obj,
//            GiftTaste taste,
//            TasteSourceMode mode)
//        {
//            string itemId = obj.QualifiedItemId;

//            return GiftableNPC.GetAllGiftableNPCs()
//                .Select(NPCGiftStatusBuilder.GiftStatus)
//                .Where(c => c.IsAvailable)
//                .Where(c =>
//                    GetCanonicalTaste(obj, c.NPC) == taste &&
//                    IsTasteKnown(itemId, c.Name, taste, mode))
//                .Select(c => c.NPC)
//                .OrderBy(npc => npc.displayName);
//        }

//        // The player does NOT know this gift's taste for which NPCs?
//        // E.g. Who do I not know loves parsnips?
//        public static IEnumerable<NPC> GetUnknownFor(
//            StardewValley.Object obj,
//            GiftTaste taste,
//            TasteSourceMode mode)
//        {
//            string itemId = obj.QualifiedItemId;

//            return GiftableNPC.GetAllGiftableNPCs()
//                .Select(NPCGiftStatusBuilder.GiftStatus)
//                .Where(c =>
//                    GetCanonicalTaste(obj, c.NPC) == taste &&
//                    !IsTasteKnown(itemId, c.Name, taste, mode))
//                .Select(c => c.NPC)
//                .OrderBy(npc => npc.displayName);
//        }

//        /// NPCs where the player does not know the taste for a specific item
//        /// Includes available and unavailable NPCs (e.g. includes Leo/Sandy even if not yet met)
//        public static IEnumerable<NPC> GetUndiscoveredBy(
//            StardewValley.Object obj,
//            TasteSourceMode mode)
//        {
//            string itemId = obj.QualifiedItemId;

//            return GiftableNPC.GetAllGiftableNPCs()
//                .Select(NPCGiftStatusBuilder.GiftStatus)
//                .Where(c =>
//                {
//                    // canonical taste must exist
//                    var canonical = GetCanonicalTaste(obj, c.NPC);
//                    if (canonical == null)
//                        return false;

//                    return !IsTasteKnown(itemId, c.Name, canonical.Value, mode);
//                })
//                .Select(c => c.NPC)
//                .OrderBy(npc => npc.displayName);
//        }

//        //Get the gift taste for an NPC and item according to the game's logic
//        public static GiftTaste? GetCanonicalTaste(StardewValley.Object obj, NPC npc)
//        {
//            var map = GiftKnowledgeService.GetCanonicalTasteMap(npc);

//            if (map.TryGetValue(obj.QualifiedItemId, out var taste))
//                return taste;

//            return null;
//        }

//        public static bool IsTasteKnown(
//            string itemId,
//            string npcName,
//            GiftTaste taste,
//            TasteSourceMode mode)
//        {
//            return mode switch
//            {
//                TasteSourceMode.All =>
//                    true, // everything is considered known

//                TasteSourceMode.Global =>
//                    GiftKnowledgeService.TryGetGlobalKnownTaste(itemId, npcName, out var t)
//                    && t == taste,

//                TasteSourceMode.Local =>
//                    GiftKnowledgeService.TryGetLocalKnownTaste(itemId, npcName, out var t)
//                    && t == taste,

//                _ => false
//            };
//        }

//        public static bool HasDiscoveredAllLovesLikes(
//            StardewValley.Object obj,
//            TasteSourceMode mode)
//        {
//            // In ALL mode, everything is always known
//            if (mode == TasteSourceMode.All)
//                return true;

//            // canonical game totals (LOVE + LIKE)
//            int canonicalTotal =
//                GetKnownFor(obj, GiftTaste.Love, TasteSourceMode.All).Count()
//                + GetKnownFor(obj, GiftTaste.Like, TasteSourceMode.All).Count();

//            // discovered totals (LOVE + LIKE)
//            int discoveredTotal =
//                GetKnownFor(obj, GiftTaste.Love, mode).Count()
//                + GetKnownFor(obj, GiftTaste.Like, mode).Count();

//            return discoveredTotal >= canonicalTotal;
//        }

//        public static IEnumerable<string> GetKnownGiftsForNPC(
//            NPC npc,
//            GiftTaste taste,
//            TasteSourceMode mode)
//        {
//            string npcName = npc.Name;

//            foreach (var obj in GiftableObjectList.AllGiftable)
//            {
//                string itemId = obj.QualifiedItemId;

//                // Canonical taste must match
//                var canonical = GetCanonicalTaste(obj, npc);
//                if (canonical != taste)
//                    continue;

//                // ALL mode → everything is known
//                if (mode == TasteSourceMode.All)
//                {
//                    yield return itemId;
//                    continue;
//                }

//                // Otherwise → only if learned
//                if (LearnedGiftsHelper.IsTasteKnown(itemId, npcName, taste, mode))
//                    yield return itemId;
//            }
//        }

//        public static IEnumerable<string> GetUnknownGiftsForNPC(
//            NPC npc,
//            GiftTaste taste,
//            TasteSourceMode mode)
//        {
//            string npcName = npc.Name;

//            // ALL mode → nothing is unknown
//            if (mode == TasteSourceMode.All)
//                yield break;

//            foreach (var obj in GiftableObjectList.AllGiftable)
//            {
//                string itemId = obj.QualifiedItemId;

//                // Canonical taste must match
//                var canonical = GetCanonicalTaste(obj, npc);
//                if (canonical != taste)
//                    continue;

//                // If learned → skip
//                if (LearnedGiftsHelper.IsTasteKnown(itemId, npcName, taste, mode))
//                    continue;

//                yield return itemId;
//            }
//        }


//    }
//}
