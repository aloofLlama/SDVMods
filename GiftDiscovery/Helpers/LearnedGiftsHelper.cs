using GiftDiscovery.GameData;
using GiftDiscovery.Models;
using GiftDiscovery.Models.Builders;
using GiftDiscovery.Services;
using SDVCommon.Helpers;
using StardewModdingAPI;
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
                    var canonical = TasteResolver.GetCanonicalTasteForItem(qualifiedItemId, c.NPC);
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
                var canonical = TasteResolver.GetCanonicalTasteForItem(qualifiedItemId, c.NPC);
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
                var canonical = TasteResolver.GetCanonicalTasteForItem(qualifiedItemId, c.NPC);
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
                string qualifiedId = obj.QualifiedItemId;

                var canonical = TasteResolver.GetCanonicalTaste(qualifiedId, npc);
                if (canonical != taste)
                    continue;

                if (TasteResolver.IsKnown(qualifiedId, npc, mode))
                    yield return qualifiedId;
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
                string qualifiedItemId = obj.QualifiedItemId;

                var canonical = TasteResolver.GetCanonicalTaste(qualifiedItemId, npc);
                if (canonical != taste)
                    continue;

                if (TasteResolver.IsUnknown(qualifiedItemId, npc, mode))
                    yield return qualifiedItemId;
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



