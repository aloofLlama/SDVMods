using GiftDiscovery.Compatibility;
using Microsoft.Xna.Framework;
using SDVCommon.Services;
using StardewValley;
using StardewValley.TokenizableStrings;
using StardewModdingAPI;



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

            return GiftableNPCList.GetAllGiftableNPCs()
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
            //PERF - this runs a lot
            //SDVCommonLog.Log($"{DateTime.Now:HH:mm:ss} Get Nearby NPC", LogLevel.Warn);

            Vector2 playerTile = Game1.player.Tile;
            NPC? closest = null;
            float bestDist = float.MaxValue;

            foreach (var npc in GiftableNPCList.GetAllGiftableNPCs())
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

            //SDVCommonLog.Log($"{DateTime.Now:HH:mm:ss} Nearby Done {closest}", LogLevel.Info);

            return closest;
        }

        public static string GetNPCLocation(NPC npc)
        {
            GameLocation? loc = npc.currentLocation;

            if (loc is null)
                return "???";

            string location = loc.Name;
            var data = loc.GetData();

            // map data DisplayName
            if (data != null)
            {
                string name = TokenParser.ParseText(data.DisplayName);
                if (!string.IsNullOrWhiteSpace(name))
                    return name;
            }

            //fallback
            return location;
        }

    }
}
