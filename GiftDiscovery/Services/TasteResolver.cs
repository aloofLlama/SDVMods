using GiftDiscovery.Models;
using StardewValley;

namespace GiftDiscovery.Services
{
    internal static class TasteResolver
    {
        // ---------------------------------------------------------
        // CANONICAL TASTE (NPC → Item)
        // ---------------------------------------------------------
        internal static GiftTaste? GetCanonicalTaste(string qualifiedItemId, NPC npc)
        {

            var map = GiftKnowledgeService.GetCanonicalTasteMap(npc);

            if (map.TryGetValue(qualifiedItemId, out var taste))
                return taste;

            return null;
        }

        // ---------------------------------------------------------
        // TASTE (Item → NPC)
        // ---------------------------------------------------------
        internal static GiftTaste? GetCanonicalTasteForItem(string qualifiedItemId, NPC npc)
        {
            return GiftKnowledgeService.GetCanonicalTasteForItem(qualifiedItemId, npc);
        }

        internal static GiftTaste? GetLearnedTasteGlobal(string itemId, NPC npc)
        {
            if (GiftKnowledgeService.TryGetGlobalKnownTaste(itemId, npc.Name, out var t))
                return t;

            return null;
        }

        internal static GiftTaste? GetLearnedTasteLocal(string itemId, NPC npc)
        {
            // Uses your existing local learned dictionary
            if (GiftKnowledgeService.TryGetLocalKnownTaste(itemId, npc.Name, out var t))
                return t;

            return null;
        }

        // ---------------------------------------------------------
        // MODE-FILTERED KNOWN / UNKNOWN
        // ---------------------------------------------------------
        internal static bool IsKnown(string itemId, NPC npc, TasteSourceMode mode)
        {
            switch (mode)
            {
                case TasteSourceMode.All:
                    return GetCanonicalTasteForItem(itemId, npc) != null;

                case TasteSourceMode.Global:
                    return GetLearnedTasteGlobal(itemId, npc) != null;

                case TasteSourceMode.Local:
                    return GetLearnedTasteLocal(itemId, npc) != null;

                default:
                    return false;
            }
        }

        internal static bool IsUnknown(string itemId, NPC npc, TasteSourceMode mode)
        {
            // ALL mode → nothing is unknown
            if (mode == TasteSourceMode.All)
                return false;

            // Must have a canonical taste to be unknown
            var canonical = TasteResolver.GetCanonicalTasteForItem(itemId, npc);
            if (canonical == null)
                return false;

            // Unknown = canonical exists but not learned
            return !IsKnown(itemId, npc, mode);
        }
    }
}
