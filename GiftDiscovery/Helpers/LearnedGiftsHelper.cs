using GiftDiscovery.GameData.Static;
using GiftDiscovery.ModData;
using GiftDiscovery.Models;
using GiftDiscovery.Services;
using SDVCommon;
using StardewValley;

namespace GiftDiscovery.Helpers
{
    internal static class LearnedGiftsHelper
    {
        // ---------------------------------------------------------
        // ITEM → NPC (Known)
        // ---------------------------------------------------------
        // Tip: Use TasteSourceMode 'All' to get the full list of NPCs
        internal static IEnumerable<NPC> GetKnownFor(
            string qId,
            GiftTaste taste,
            TasteSourceMode mode)
        {
            return NPCGiftStatus.GetAllGiftableNPCs()
                .Select(NPCGiftStatus.GetNPCGiftStatus)
                //.Where(c => c.IsAvailable) //with this included e.g. Sandy wont show up until met
                .Where((c) =>
                {
                    var canonical = TasteMap.GetTasteForNPCItemPair(qId, c.NPC);
                    if (canonical != taste)
                        return false;

                    return IsKnown(qId, c.NPC, mode);
                })
                .Select((c) => c.NPC)
                .OrderBy(( npc) => npc.displayName);
        }

        // ---------------------------------------------------------
        // ITEM → NPC (Unknown)
        // ---------------------------------------------------------
        internal static IEnumerable<NPC> GetUnknownFor(
            string qId,
            GiftTaste taste,
            TasteSourceMode mode)
        {
            if (mode == TasteSourceMode.All)
                yield break;

            foreach (var c in NPCGiftStatus.GetAllGiftableNPCs()
                .Select(NPCGiftStatus.GetNPCGiftStatus))
            {
                var canonical = TasteMap.GetTasteForNPCItemPair(qId, c.NPC);
                if (canonical != taste)
                    continue;

                if (IsUnknown(qId, c.NPC, mode))
                    yield return c.NPC;
            }
        }

        // ---------------------------------------------------------
        // ITEM → NPC (Undiscovered, all tastes)
        // ---------------------------------------------------------
        internal static IEnumerable<NPC> GetUndiscoveredBy(
            string qId,
            TasteSourceMode mode)
        {
            if (mode == TasteSourceMode.All)
                yield break;

            foreach (var c in NPCGiftStatus.GetAllGiftableNPCs()
                .Select(NPCGiftStatus.GetNPCGiftStatus))
            {
                var canonical = TasteMap.GetTasteForNPCItemPair(qId, c.NPC);
                if (canonical == null)
                    continue;

                if (IsUnknown(qId, c.NPC, mode))
                    yield return c.NPC;
            }
        }


        // ---------------------------------------------------------
        // NPC → ITEM (Known)
        // ---------------------------------------------------------
        internal static IEnumerable<string> GetKnownGiftsForNPC(
            NPC npc,
            GiftTaste taste,
            TasteSourceMode mode)
        {
            foreach (string qId in GiftableObjectList.GetAllGiftableIds())
            {

                var canonical = TasteMap.GetTasteForNPCItemPair(qId, npc);
                if (canonical != taste)
                    continue;

                if (IsKnown(qId, npc, mode))
                    yield return qId;
            }

        }

        // ---------------------------------------------------------
        // NPC → ITEM (Unknown)
        // ---------------------------------------------------------
        internal static IEnumerable<string> GetUnknownGiftsForNPC(
            NPC npc,
            GiftTaste taste,
            TasteSourceMode mode)
        {
            if (mode == TasteSourceMode.All)
                yield break;

            string timer = "Get Unknown Gifts for NPC";  // Logs {name} took {ms} ms
            SDVCommonServices.PerfBegin(timer);
            int cnt = 0;

            foreach (var obj in GiftableObjectList.GetAllGiftableObjects())
            {
                string qId = obj.QualifiedItemId;
                cnt++;
                var canonical = TasteMap.GetTasteForNPCItemPair(qId, npc);
                if (canonical != taste)
                    continue;

                if (IsUnknown(qId, npc, mode))
                    yield return qId;
            }
            SDVCommonServices.PerfEnd(timer, 10);
        }

        // ---------------------------------------------------------
        // Has the player discovered ALL Loves + Likes for this item?
        // ---------------------------------------------------------
        internal static bool HasDiscoveredAllLovesLikesforItem(
            string qId,
            TasteSourceMode mode)
        {
            if (mode == TasteSourceMode.All)
                return true;

            int canonicalTotal =
                GetKnownFor(qId, GiftTaste.Love, TasteSourceMode.All).Count()
                + GetKnownFor(qId, GiftTaste.Like, TasteSourceMode.All).Count();

            int discoveredTotal =
                GetKnownFor(qId, GiftTaste.Love, mode).Count()
                + GetKnownFor(qId, GiftTaste.Like, mode).Count();

            return discoveredTotal >= canonicalTotal;

        }

        // ---------------------------------------------------------
        // Has the player discovered ALL Loves + Likes for this NPC?
        // ---------------------------------------------------------
        internal static bool HasDiscoveredAllLovesLikesForNPC(
            NPC npc,
            TasteSourceMode mode)
        {
            string timer = "Has Discovered All Loves Likes for NPC";
            SDVCommonServices.PerfBegin(timer);

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

            SDVCommonServices.PerfEnd(timer, 10);

            return discoveredTotal >= canonicalTotal;
        }

        // ---------------------------------------------------------
        // MODE-FILTERED KNOWN / UNKNOWN
        // ---------------------------------------------------------
        private static bool IsKnown(string qId, NPC npc, TasteSourceMode mode)
        {
            return mode switch
            {
                TasteSourceMode.All => TasteMap.GetTasteForNPCItemPair(qId, npc) != null,
                TasteSourceMode.Global => TasteLearning.IsKnownGlobal(qId, npc),
                TasteSourceMode.Local => TasteLearning.IsKnownLocal(qId, npc),
                _ => false,
            };
        }

        private static bool IsUnknown(string qId, NPC npc, TasteSourceMode mode)
        {
            return !IsKnown(qId, npc, mode);
        }

    }
}



