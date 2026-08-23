using SDVCommon.Helpers;
using SDVCommon.Services;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Locations;
using StardewValley.Objects;


namespace SDVCommon.GameData
{
    public class Inventory
    {
        public static int CountOwned(string qId)
        {
            int total = 0;
            HashSet<Item> seen = new();

            // 1. Inventory
            ScanItem(Game1.player, Game1.player.Items, qId, ref total, seen);

            // 2. All locations
            foreach (var location in GetAllLocations())
            {
                //Vanilla farmhouse fridge
                if (location is FarmHouse fh && fh.fridge.Value is Chest vanillaFridge)
                    ScanItem(location, vanillaFridge, qId, ref total, seen);

                //Island farmhouse fridge
                if (location is IslandFarmHouse ifh && ifh.fridge.Value is Chest islandFridge)
                    ScanItem(location, islandFridge, qId, ref total, seen);

                //placed fridges (SBV, modded, mini-fridges)
                foreach (var obj in location.objects.Values)
                {
                    if (obj is Chest chest && chest.fridge.Value)
                        ScanItem(location, chest, qId, ref total, seen);
                }

                // building chests + Junimo huts
                foreach (var building in location.buildings)
                {
                    foreach (var chest in building.buildingChests)
                        ScanItem(building, chest, qId, ref total, seen);

                    if (building is JunimoHut hut)
                        ScanItem(hut, hut.GetOutputChest(), qId, ref total, seen);
                }

                // placed chests + non‑spawned objects
                foreach (var obj in location.objects.Values)
                {
                    if (obj is Chest chest)
                        ScanItem(location, chest, qId, ref total, seen);
                    else if (!IsWorldItem(obj))
                        ScanItem(location, obj, qId, ref total, seen);
                }
            }

            // 3. Hay
            if (qId == "(O)178")
            {
                int hayCount = Game1.getFarm()?.piecesOfHay.Value ?? 0;
                total += hayCount;
            }

            return total;
        }
        public static int CountOwnedInMainFarmhouseFridges(string qId)
        {
            int total = 0;
            HashSet<Item> seen = new(); // reference equality

            // Get the MAIN farmhouse (not island, not modded)
            var farmhouse = Game1.getLocationFromName("FarmHouse") as FarmHouse;
            if (farmhouse == null)
                return 0;

            // 1. Main farmhouse built‑in fridge
            if (farmhouse.fridge?.Value is Chest mainFridge)
                ScanItem(farmhouse, mainFridge, qId, ref total, seen);

            // 2. Mini‑fridges placed INSIDE the main farmhouse
            foreach (var obj in farmhouse.objects.Values)
            {
                if (obj is Chest chest && chest.fridge.Value)
                    ScanItem(farmhouse, chest, qId, ref total, seen);
            }

            return total;
        }

        public static int CountOwnedInMainFarmhouseFridgesByCategory(int categoryId)
        {
            int total = 0;
            HashSet<Item> seen = new(); // reference equality

            // Get the MAIN farmhouse (not island, not modded)
            var farmhouse = Game1.getLocationFromName("FarmHouse") as FarmHouse;
            if (farmhouse == null)
                return 0;

            // 1. Main farmhouse built‑in fridge
            if (farmhouse.fridge?.Value is Chest mainFridge)
                ScanItembyCategory(farmhouse, mainFridge, categoryId, ref total, seen);

            // 2. Mini‑fridges placed INSIDE the main farmhouse
            foreach (var obj in farmhouse.objects.Values)
            {
                if (obj is Chest chest && chest.fridge.Value)
                    ScanItembyCategory(farmhouse, chest, categoryId, ref total, seen);
            }

            return total;
        }


        public static bool IsInBackpack(string qId)
        {
            foreach (var item in Game1.player.Items)
            {
                if (item is not null &&
                    item.QualifiedItemId
                        .Equals(qId, StringComparison.OrdinalIgnoreCase))
                    return true;
            }

            return false;
        }

        private static void ScanItem(object parent, IEnumerable<Item> roots, string qId, ref int total, HashSet<Item> seen)
        {
            foreach (var item in roots)
                ScanItem(parent, item, qId, ref total, seen);
        }

        private static void ScanItem(object parent, Item root, string qId, ref int total, HashSet<Item> seen)
        {
            if (root == null || seen.Contains(root))
                return;

            seen.Add(root);

            // Count only StardewValley.Object
            if (root is StardewValley.Object obj && !obj.bigCraftable.Value)
            {
                if (obj.QualifiedItemId == qId)
                    total += obj.Stack;
            }

            // Recurse into chests
            if (root is Chest chest)
            {
                foreach (var item in chest.Items)
                    ScanItem(chest, item, qId, ref total, seen);
            }

            // Recurse into storage furniture (dressers, fridges)
            if (root is StorageFurniture sf)
            {
                foreach (var item in sf.heldItems)
                    ScanItem(sf, item, qId, ref total, seen);
            }
        }
        private static void ScanItembyCategory(object parent, IEnumerable<Item> roots, int categoryId, ref int total, HashSet<Item> seen)
        {
            foreach (var item in roots)
                ScanItembyCategory(parent, item, categoryId, ref total, seen);
        }

        private static void ScanItembyCategory(object parent, Item root, int categoryId, ref int total, HashSet<Item> seen)
        {
            if (root == null || seen.Contains(root))
                return;

            seen.Add(root);

            // Count only StardewValley.Object
            if (root is StardewValley.Object obj && !obj.bigCraftable.Value)
            {
                int category = obj.Category;
                if (category == categoryId)
                    total += obj.Stack;
            }

            // Recurse into chests
            if (root is Chest chest)
            {
                foreach (var item in chest.Items)
                    ScanItembyCategory(chest, item, categoryId, ref total, seen);
            }

            // Recurse into storage furniture (dressers, fridges)
            if (root is StorageFurniture sf)
            {
                foreach (var item in sf.heldItems)
                    ScanItembyCategory(sf, item, categoryId, ref total, seen);
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
