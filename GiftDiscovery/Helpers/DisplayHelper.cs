using StardewValley;
using Microsoft.Xna.Framework;


namespace GiftDiscovery.Helpers
{
    public class DisplayHelper
    {
        // ---------------------------------------------------------
        // Nearby NPC Cache
        // ---------------------------------------------------------
        private static HashSet<string> _nearbyNpcNames = new();
        private static readonly Dictionary<string, Vector2> _lastNpcTiles = new();

        public static HashSet<string> GetNearbyNpcNames(int range)
        {
            var playerTile = Game1.player.Tile;
            var location = Game1.currentLocation;

            return GiftHelper.GetAllGiftableNPCs()
                .Where(npc =>
                    npc.currentLocation == location &&
                    Vector2.Distance(npc.Tile, playerTile) <= range)
                .Select(npc => npc.Name)
                .ToHashSet();
        }

    }
}
