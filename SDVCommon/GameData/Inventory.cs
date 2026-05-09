using StardewValley;
using StardewValley.Buildings;
using StardewValley.Objects;
using StardewValley.Locations;
using SDVCommon.Helpers;


namespace SDVCommon.GameData
{
    public class Inventory
    {

        public static int CountOwned(string canonicalId)
        {
            int total = 0;
            HashSet<Item> seen = new();

            // 1. Inventory
            ScanItem(Game1.player, Game1.player.Items, canonicalId, ref total, seen);

            // 2. All locations
            foreach (var location in GetAllLocations())
            {
                //Vanilla farmhouse fridge
                if (location is FarmHouse fh && fh.fridge.Value is Chest vanillaFridge)
                    ScanItem(location, vanillaFridge, canonicalId, ref total, seen);

                //Island farmhouse fridge
                if (location is IslandFarmHouse ifh && ifh.fridge.Value is Chest islandFridge)
                    ScanItem(location, islandFridge, canonicalId, ref total, seen);

                //placed fridges (SBV, modded, mini-fridges)
                foreach (var obj in location.objects.Values)
                {
                    if (obj is Chest chest && chest.fridge.Value)
                        ScanItem(location, chest, canonicalId, ref total, seen);
                }

                // building chests + Junimo huts
                foreach (var building in location.buildings)
                {
                    foreach (var chest in building.buildingChests)
                        ScanItem(building, chest, canonicalId, ref total, seen);

                    if (building is JunimoHut hut)
                        ScanItem(hut, hut.GetOutputChest(), canonicalId, ref total, seen);
                }

                // placed chests + non‑spawned objects
                foreach (var obj in location.objects.Values)
                {
                    if (obj is Chest chest)
                        ScanItem(location, chest, canonicalId, ref total, seen);
                    else if (!IsWorldItem(obj))
                        ScanItem(location, obj, canonicalId, ref total, seen);
                }
            }

            // 3. Hay
            if (canonicalId == "178")
            {
                int hayCount = Game1.getFarm()?.piecesOfHay.Value ?? 0;
                total += hayCount;
            }

            return total;
        }

        public static int CountOwnedInMainFarmhouseFridges(string canonicalId)
        {
            int total = 0;
            HashSet<Item> seen = new(); // reference equality

            // Get the MAIN farmhouse (not island, not modded)
            var farmhouse = Game1.getLocationFromName("FarmHouse") as FarmHouse;
            if (farmhouse == null)
                return 0;

            // 1. Main farmhouse built‑in fridge
            if (farmhouse.fridge?.Value is Chest mainFridge)
                ScanItem(farmhouse, mainFridge, canonicalId, ref total, seen);

            // 2. Mini‑fridges placed INSIDE the main farmhouse
            foreach (var obj in farmhouse.objects.Values)
            {
                if (obj is Chest chest && chest.fridge.Value)
                    ScanItem(farmhouse, chest, canonicalId, ref total, seen);
            }

            return total;
        }


        private static void ScanItem(object parent, IEnumerable<Item> roots, string canonicalId, ref int total, HashSet<Item> seen)
        {
            foreach (var item in roots)
                ScanItem(parent, item, canonicalId, ref total, seen);
        }

        private static void ScanItem(object parent, Item root, string canonicalId, ref int total, HashSet<Item> seen)
        {
            if (root == null || seen.Contains(root))
                return;

            seen.Add(root);

            // Count only StardewValley.Object
            if (root is StardewValley.Object obj && !obj.bigCraftable.Value)
            {
                string id = IdHelper.CanonicalItemId(obj.QualifiedItemId);
                if (id == canonicalId)
                    total += obj.Stack;
            }

            // Recurse into chests
            if (root is Chest chest)
            {
                foreach (var item in chest.Items)
                    ScanItem(chest, item, canonicalId, ref total, seen);
            }

            // Recurse into storage furniture (dressers, fridges)
            if (root is StorageFurniture sf)
            {
                foreach (var item in sf.heldItems)
                    ScanItem(sf, item, canonicalId, ref total, seen);
            }
        }

        private static bool IsWorldItem(StardewValley.Object obj)
        {
            return obj is not Chest;
        }

        public static IEnumerable<GameLocation> GetAllLocations()
        {
            // base locations
            foreach (var location in Game1.locations)
                yield return location;

            // nested building interiors (barns, coops, sheds, cabins, etc.)
            foreach (var location in Game1.locations)
            {
                // Only locations that actually HAVE buildings
                if (location.buildings != null)
                {
                    foreach (var building in location.buildings)
                    {
                        if (building.indoors.Value != null)
                            yield return building.indoors.Value;
                    }
                }
            }
        }



    }
}
