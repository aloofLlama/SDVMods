using GiftDiscovery.GameData.Static;
using GiftDiscovery.ModData;
using GiftDiscovery.Models;
using GiftDiscovery.Services;
using SDVCommon;
using SDVCommon.Helpers;
using SDVCommon.Services;
using StardewModdingAPI;
using StardewValley;
using System.Timers;

namespace GiftDiscovery.Helpers
{
    internal static class LearnedGiftsHelper
    {



        // ---------------------------------------------------------
        // ITEM → NPC (Known)
        // ---------------------------------------------------------
        // Tip: Use TasteSourceMode 'All' to get the full list of NPCs
        public static IEnumerable<NPC> GetKnownFor(
            string qualifiedItemId,
            GiftTaste taste,
            TasteSourceMode mode)
        {
            return NPCGiftStatus.GetAllGiftableNPCs()
                .Select(NPCGiftStatus.GetNPCGiftStatus)
                //.Where(c => c.IsAvailable) //with this included e.g. Sandy wont show up until met
                .Where((c) =>
                {
                    var canonical = TasteMap.GetTasteForNPCItemPair(qualifiedItemId, c.NPC);
                    if (canonical != taste)
                        return false;

                    return IsKnown(qualifiedItemId, c.NPC, mode);
                })
                .Select((c) => c.NPC)
                .OrderBy(( npc) => npc.displayName);
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

            foreach (var c in NPCGiftStatus.GetAllGiftableNPCs()
                .Select(NPCGiftStatus.GetNPCGiftStatus))
            {
                var canonical = TasteMap.GetTasteForNPCItemPair(qualifiedItemId, c.NPC);
                if (canonical != taste)
                    continue;

                if (IsUnknown(qualifiedItemId, c.NPC, mode))
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

            foreach (var c in NPCGiftStatus.GetAllGiftableNPCs()
                .Select(NPCGiftStatus.GetNPCGiftStatus))
            {
                var canonical = TasteMap.GetTasteForNPCItemPair(qualifiedItemId, c.NPC);
                if (canonical == null)
                    continue;

                if (IsUnknown(qualifiedItemId, c.NPC, mode))
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
            //string timer = "Get Known Gifts for NPC";  // Logs {name} took {ms} ms
            //LogLevel logLevel = LogHelper.DebugWarn;
            //SDVCommonServices.PerfBegin(timer);

            foreach (string qId in GiftableObjectList.GetAllGiftableIds())
            {

                var canonical = TasteMap.GetTasteForNPCItemPair(qId, npc);
                if (canonical != taste)
                    continue;

                if (IsKnown(qId, npc, mode))
                    yield return qId;
            }

            //SDVCommonServices.PerfEnd(timer, npc.displayName, 0, logLevel);

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
            string timer = "Has Discovered All Loves Likes for NPC";
            LogLevel logLevel = LogHelper.DebugOrTrace;
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

            SDVCommonServices.PerfEnd(timer, 10, logLevel);

            return discoveredTotal >= canonicalTotal;
        }

        // ---------------------------------------------------------
        // MODE-FILTERED KNOWN / UNKNOWN
        // ---------------------------------------------------------
        private static bool IsKnown(string itemId, NPC npc, TasteSourceMode mode)
        {
            switch (mode)
            {
                case TasteSourceMode.All:
                    return TasteMap.GetTasteForNPCItemPair(itemId, npc) != null;

                case TasteSourceMode.Global:
                    return TasteLearning.IsKnownGlobal(itemId, npc);

                case TasteSourceMode.Local:
                    return TasteLearning.IsKnownLocal(itemId, npc);

                default:
                    return false;
            }
        }

        private static bool IsUnknown(string itemId, NPC npc, TasteSourceMode mode)
        {
            return !IsKnown(itemId, npc, mode);
        }

    }
}



