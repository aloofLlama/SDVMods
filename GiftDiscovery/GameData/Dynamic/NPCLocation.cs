using GiftDiscovery.GameData.Static;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.TokenizableStrings;


namespace GiftDiscovery.GameData.Dynamic
{
    internal static class NPCLocation
    {
        // Get the location of a NPC e.g. Carpenter's Shop, Pierre's General Store, etc.
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

            return location;
        }


        internal static HashSet<string> GetNearbyNPCNames(int range)
        {
            return NPCGiftStatus.GetAllGiftableNPCs()
                .Select(npc => new { npc, dist = GetPlayerToNPCDistance(npc) })
                .Where(x => x.dist.HasValue && x.dist.Value <= range)
                .Select(x => x.npc.Name)
                .ToHashSet();
        }


        internal static bool IsNPCNearby(NPC npc, int range)
        {
            float? distance = GetPlayerToNPCDistance(npc);

            if (distance is null)
                return false;

            return distance <= range;
        }

        internal static NPC? GetClosestNearbyNPC(int range)
        {
            NPC? closest = null;
            float bestDist = float.MaxValue;

            foreach (var npc in NPCGiftStatus.GetAllGiftableNPCs())
            {
                float? distance = GetPlayerToNPCDistance(npc);
                if (distance is null)
                    continue;

                if (distance <= range && distance < bestDist)
                {
                    bestDist = (float)distance;
                    closest = npc;
                }
            }

            return closest;
        }

        private static float? GetPlayerToNPCDistance(NPC npc)
        {
            if (npc.currentLocation != Game1.player.currentLocation)
                return null;

            Vector2 playerTile = Game1.player.Tile;
            Vector2 npcTile = npc.Tile;

            return (float)Vector2.Distance(playerTile, npcTile);
        }

    }
}
