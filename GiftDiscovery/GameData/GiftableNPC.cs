using GiftDiscovery.Compatibility;
using GiftDiscovery.Models.Builders;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.TokenizableStrings;
using static GiftDiscovery.Helpers.DisplayHelper;


namespace GiftDiscovery.GameData
{
    internal class GiftableNPC
    {
        /// NPCs that can receive gifts at some point in the game. 
        /// NPCs may be currently available or not (e.g. Leo/Sandy)
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

        public static bool IsUnmetNPC(string npcName)
        {
            var npc = Utility.getAllCharacters().FirstOrDefault(n => n.Name == npcName);
            if (npc == null)
                return false;

            var c = NPCGiftStatusBuilder.GiftStatus(npc);
            return c.IsUnmet;
        }

        // ---------------------------------------------------------
        // Nearby NPC Cache
        // ---------------------------------------------------------
        private static HashSet<string> _nearbyNPCNames = new();
        private static readonly Dictionary<string, Vector2> _lastNPCTiles = new();

        public static HashSet<string> GetNearbyNPCNames(int range)
        {
            var playerTile = Game1.player.Tile;
            var location = Game1.currentLocation;

            return GetAllGiftableNPCs()
                .Where(npc =>
                    npc.currentLocation == location &&
                    Vector2.Distance(npc.Tile, playerTile) <= range)
                .Select(npc => npc.Name)
                .ToHashSet();
        }


        public static bool IsNPCNearby(StardewValley.NPC npc, int rangeTiles)
        {
            if (npc.currentLocation != Game1.player.currentLocation)
                return false;

            Vector2 npcTile = npc.Tile;
            Vector2 playerTile = Game1.player.Tile;

            float distance = Vector2.Distance(npcTile, playerTile);
            return distance <= rangeTiles;
        }

        public static NPC? GetClosestNearbyNPC(int rangeTiles)
        {
            Vector2 playerTile = Game1.player.Tile;
            NPC? closest = null;
            float bestDist = float.MaxValue;

            foreach (var npc in GiftableNPC.GetAllGiftableNPCs())
            {
                // ⭐ REQUIRED: filter by location
                if (npc.currentLocation != Game1.currentLocation)
                    continue;

                Vector2 npcTile = npc.Tile;
                float dist = Vector2.Distance(npcTile, playerTile);

                if (dist <= rangeTiles && dist < bestDist)
                {
                    bestDist = dist;
                    closest = npc;
                }
            }

            return closest;
        }

        public static string GetNPCLocation(NPC npc)
        {
            GameLocation? loc = npc.currentLocation;

            if (loc is null)
                return "???";

            string id = loc.Name;
            var data = loc.GetData();

            // map data DisplayName
            if (data != null)
            {
                string name = TokenParser.ParseText(data.DisplayName);
                if (!string.IsNullOrWhiteSpace(name))
                    return name;
            }

            //fallback
            return id;
        }

    }
}
