using SDVCommon.Compatibility;
using StardewModdingAPI;
using StardewValley;
using StardewValley.TerrainFeatures;
using SDVCommon.Helpers;
using SDVCommon.Services;


namespace SDVCommon.GameData
{
    public static class Planted
    {
        public static int CountPlanted(string id)
        {
            string itemId = IdHelper.ToUnqualifiedItemId(id);

            int total = 0;

            SDVCommonLog.Log(
                $"[PlantedHelper] CountPlanted for itemId={itemId}");

            bool IsAccessible(GameLocation loc)
            {
                foreach (var l in Game1.locations)
                {
                    foreach (var warp in l.warps)
                    {
                        if (warp.TargetName == loc.Name)
                            return true;
                    }
                }

                return false;
            }


            foreach (var location in Inventory.GetAllLocations())
            {

                // Skip inaccessible or placeholder locations (eg SBV Ripley Farm)
                if (!IsAccessible(location))
                    continue;

                // 1. Crops + Fruit Trees
                foreach (var tf in location.terrainFeatures.Values)
                {
                    if (tf is HoeDirt hd && hd.crop is Crop crop)
                    {
                        if (crop.indexOfHarvest?.Value != null)
                        {
                            // Convert numeric harvest ID → Qualified ID → Canonical ID
                            var data = ItemRegistry.GetData(crop.indexOfHarvest.Value);
                            if (data != null)
                            {
                                string harvestId = data.QualifiedItemId; // e.g. "(O)Cornucopia_Shiitake"
                                string canonical = IdHelper.ToUnqualifiedItemId(harvestId);

                                if (canonical == itemId)
                                    total++;
                                SDVCommonLog.Log(
                                                        $"[PlantedHelper] MATCH CROP in {location.Name}: harvest={harvestId}");
                            }
                        }
                    }

                    if (tf is FruitTree ft)
                    {
                        if (Game1.fruitTreeData.TryGetValue(ft.treeId.Value, out var ftData))
                        {
                            var fruitEntry = ftData.Fruit.FirstOrDefault();
                            if (fruitEntry != null)
                            {
                                string fruitId = fruitEntry.ItemId;
                                string canonical = IdHelper.ToUnqualifiedItemId(fruitId);

                                if (canonical == itemId)
                                {
                                    total++;
                                    SDVCommonLog.Log(
                                                                $"[PlantedHelper] MATCH FRUIT TREE in {location.Name}: fruit={canonical}");
                                }
                            }
                        }
                    }
                }

                //TODO CUSTOM BUSH

                //// 3. Bushes (Custom Bush)
                //foreach (var tf in location.terrainFeatures.Values)
                //{
                //    if (tf is Bush bush)
                //    {
                //        var api = CustomBushCompat.Api;
                //        if (api != null &&
                //            api.TryGetBush(bush, out var bushData, out string? bushId))
                //        {
                //            //custom bush gives the planted bush the seed Id
                //            string seedId = IdHelper.ToItemId(bushId);

                //            var plant = ;

                //            string harvestId = 
                //            // NEW: map seed → harvest
                //            if (PlantDatabase.TryGetBySeedId(canon, out var plantInfo))
                //            {
                //                canon = IdHelper.ToItemId(plantInfo.HarvestId);
                //            }

                //            if (canon == itemId)
                //            {
                //                total++; total++;

                //            SDVCommonLog.Log(
                //                                                    $"[PlantedHelper] MATCH BUSH in {location.Name}: bushId={bushId} canon={canon}");
                //            }
                //            else
                //            {
                //                SDVCommonLog.Log(
                //                                            $"[PlantedHelper] NO MATCH BUSH in {location.Name}: bushId={bushId} canon={canon} vs target={itemId}");
                //            }
                //        }
                //    }
            //}

        }

            SDVCommonLog.Log(
                    $"[PlantedHelper] DONE for itemId={itemId}, total={total}");

            return total;
        }


    }
}