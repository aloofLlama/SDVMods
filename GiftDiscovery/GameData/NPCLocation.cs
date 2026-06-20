using GiftDiscovery.Compatibility;
using GiftDiscovery.Models.Builders;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.TokenizableStrings;


namespace GiftDiscovery.GameData
{
    internal class NPCLocation
    {

        // ---------------------------------------------------------
        // Nearby NPC Cache
        // ---------------------------------------------------------
        private static HashSet<string> _nearbyNPCNames = new();
        private static readonly Dictionary<string, Vector2> _lastNPCTiles = new();

        public static HashSet<string> GetNearbyNPCNames(int range)
        {
            var playerTile = Game1.player.Tile;
            var location = Game1.currentLocation;

            return GiftableNPC.GetAllGiftableNPCs()
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
                // filter by location
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
