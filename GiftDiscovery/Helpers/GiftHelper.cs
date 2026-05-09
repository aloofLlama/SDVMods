using StardewValley;
using Microsoft.Xna.Framework;
using GiftDiscovery.Services;
using GiftDiscovery.Models;
using GiftDiscovery.Compatibility;


namespace GiftDiscovery.Helpers
{

    public static class GiftHelper
    {
        /// <summary>
        /// NPCs that can receive gifts at some point in the game. 
        /// NPCs may be currently available or not (e.g. Leo/Sandy)
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<NPC> GetAllGiftableNPCs()
        {
            return Utility.getAllCharacters()
                .OfType<NPC>()
                .Where(npc =>
                {
                    string name = npc.Name;

                    // must have canonical gift taste data
                    if (!Game1.NPCGiftTastes.ContainsKey(name))
                        return false;

                    // skip explicitly non-giftable NPCs
                    if (ModCompat.GiftOverrides.NonGiftableNPCs.Contains(name))
                        return false;

                    return true;
                });
        }


        /// <summary>
        /// NPCs that like/love/etc a specific item
        /// Which the player knows about according to the mode (all/global/local)
        /// For available NPCs only (e.g. excludes Leo/Sandy until they are met)
        /// </summary>
        public static IEnumerable<NPC> GetKnownBy(
            StardewValley.Object obj,
            GiftTaste taste,
            TasteSourceMode mode)
        {
            string itemId = obj.QualifiedItemId;

            return GetAllGiftableNPCs()
                .Select(NpcGiftClassificationBuilder.Classify)
                .Where(c => c.IsAvailable)
                .Where(c =>
                    GetCanonicalTaste(obj, c.Npc) == taste &&
                    IsTasteKnown(itemId, c.Name, taste, mode))
                .Select(c => c.Npc)
                .OrderBy(npc => npc.displayName);
        }

        /// <summary>
        /// NPCs that like/love/etc a specific item
        /// Which the player does NOT know about according to the mode (all/global/local)
        /// Includes available and unavailable NPCs (e.g. includes Leo/Sandy even if not yet met)
        /// </summary>
        public static IEnumerable<NPC> GetUnknownBy(
            StardewValley.Object obj,
            GiftTaste taste,
            TasteSourceMode mode)
        {
            string itemId = obj.QualifiedItemId;

            return GetAllGiftableNPCs()
                .Select(NpcGiftClassificationBuilder.Classify)
                .Where(c =>
                    GetCanonicalTaste(obj, c.Npc) == taste &&
                    !IsTasteKnown(itemId, c.Name, taste, mode))
                .Select(c => c.Npc)
                .OrderBy(npc => npc.displayName);
        }

        /// <summary>
        /// NPCs that where the player does not know the taste for a specific item
        /// Includes available and unavailable NPCs (e.g. includes Leo/Sandy even if not yet met)

        /// </summary>
        public static IEnumerable<NPC> GetUndiscoveredBy(
            StardewValley.Object obj,
            TasteSourceMode mode)
        {
            string itemId = obj.QualifiedItemId;

            return GetAllGiftableNPCs()
                .Select(NpcGiftClassificationBuilder.Classify)
                .Where(c =>
                {
                    // canonical taste must exist
                    var canonical = GetCanonicalTaste(obj, c.Npc);
                    if (canonical == null)
                        return false;

                    // unknown = NOT IsTasteKnown(...)
                    return !IsTasteKnown(itemId, c.Name, canonical.Value, mode);
                })
                .Select(c => c.Npc)
                .OrderBy(npc => npc.displayName);
        }



        //Get the gift taste for an NPC and item according to the game's logic
        private static GiftTaste? GetCanonicalTaste(StardewValley.Object obj, NPC npc)
        {
            try
            {
                return (GiftTaste)npc.getGiftTasteForThisItem(obj);
            }
            catch
            {
                return null;
            }
        }

        public static bool IsTasteKnown(
            string itemId,
            string npcName,
            GiftTaste taste,
            TasteSourceMode mode)
        {
            return mode switch
            {
                TasteSourceMode.All =>
                    true, // everything is considered known

                TasteSourceMode.Global =>
                    GiftKnowledgeService.TryGetGlobalKnownTaste(itemId, npcName, out var t)
                    && t == taste,

                TasteSourceMode.Local =>
                    GiftKnowledgeService.TryGetLocalKnownTaste(itemId, npcName, out var t)
                    && t == taste,

                _ => false
            };
        }


        public static bool IsUnmetNPC(string npcName)
        {
            var npc = Utility.getAllCharacters().FirstOrDefault(n => n.Name == npcName);
            if (npc == null)
                return false;

            var c = NpcGiftClassificationBuilder.Classify(npc);
            return c.IsUnmet;
        }


        public static bool IsNpcNearby(NPC npc, int rangeTiles)
        {
            if (npc.currentLocation != Game1.player.currentLocation)
                return false;

            Vector2 npcTile = npc.Tile;
            Vector2 playerTile = Game1.player.Tile;

            float distance = Vector2.Distance(npcTile, playerTile);
            return distance <= rangeTiles;
        }

        public static bool HasDiscoveredAllLovesLikes(
            StardewValley.Object obj,
            TasteSourceMode mode)
        {
            // In ALL mode, everything is always known
            if (mode == TasteSourceMode.All)
                return true;

            // canonical game totals (LOVE + LIKE)
            int canonicalTotal =
                GiftHelper.GetKnownBy(obj, GiftTaste.Love, TasteSourceMode.All).Count()
                + GiftHelper.GetKnownBy(obj, GiftTaste.Like, TasteSourceMode.All).Count();

            // discovered totals (LOVE + LIKE)
            int discoveredTotal =
                GiftHelper.GetKnownBy(obj, GiftTaste.Love, mode).Count()
                + GiftHelper.GetKnownBy(obj, GiftTaste.Like, mode).Count();

            return discoveredTotal >= canonicalTotal;
        }


    }
}