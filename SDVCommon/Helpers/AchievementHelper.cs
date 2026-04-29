using SDVCommon.Models.Wrappers;
using StardewValley;
using StardewValley.GameData.Objects;
using SObject = StardewValley.Object;



namespace SDVCommon.Helpers
{
    public static class AchievementHelper
    {
        //Determines whether the player has completed an achievement

        //---
        // Shipping
        //---

        public static bool NeedsShippedOne(HarvestInfo harvest)
        {

            if (!harvest.Data.ShipOne)
                return false; // not eligible

            if (Game1.player.basicShipped.TryGetValue(harvest.Data.HarvestId, out int count))
                return count == 0;

            return true; // never shipped
        }

        public static bool NeedsMonoCultureShipped(HarvestInfo harvest)
        {
            // Not a monoculture crop
            if (!harvest.Data.ShipMonoCulture)
                return false;

            // Achievement already earned
            if (Game1.player.achievements.Contains(32))
                return false;

            return true;
        }

        public static bool NeedsPolyCultureShipped(HarvestInfo harvest)
        {
            if (!harvest.Data.ShipPolyCulture)
                return false; // not eligible

            if (Game1.player.basicShipped.TryGetValue(harvest.Data.HarvestId, out int count))
                return count < 15;

            return true; // never shipped
        }


    }
}
