using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PlantingDay.Compatibility;
using PlantingDay.Helpers;
using PlantingDay.Models;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Objects;
using StardewValley.GameData;
using StardewValley.GameData.Buffs;
using StardewValley.GameData.Crops;
using StardewValley.GameData.FruitTrees;
using StardewValley.GameData.Objects;
using StardewValley.TerrainFeatures;
using System.Collections.Generic;
using System.Xml.Linq;
//using System.Linq;
//using System.Text.Json;

namespace PlantingDay
{

    public static class PlantDatabase
    {
        private static bool _initialized;
        private static readonly Dictionary<string, PlantInfo> _plants = new();
        public static IEnumerable<PlantInfo> AllPlants => _plants.Values;

        public static void Initialize()
        {
            ModEntry.Instance.Monitor.Log("PlantDatabase.Load CALLED", LogLevel.Alert);
            if (_initialized)
                return;

            _initialized = true;

            LoadCrops();
            LoadFruitTrees();
            //LoadBushes(); PAUSEBUSHES

            //IconBuilder.BuildIconsForAllPlants(_plants);
        }

        // Lookup by seed item ID (e.g. "O:472" for Parsnip Seeds)
        public static PlantInfo? LookupFromKey(string key)
        {
            return _plants.TryGetValue(key, out var info) ? info : null;
        }

        private static ItemInfo? FromObject(string objectId)
        {
            if (!Game1.objectData.TryGetValue(objectId, out var obj))
                return null;

            return new ItemInfo
            {
                Id = objectId,
                Name = obj.DisplayName,
                Description = obj.Description,
                Price = obj.Price,
                Category = obj.Category,
                Edibility = obj.Edibility,
                Type = obj.Type
            };
        }

        private static void LoadCrops()
        {
            foreach (var (seedId, cropData) in Game1.cropData)
            {
                
                var info = FromCrop(seedId, cropData);
                if (info == null)
                    continue;

                // Seed object data
                info.Seed = FromObject(seedId);

                // Harvest object data
                if (!string.IsNullOrEmpty(info.HarvestId))
                    info.Harvest = FromObject(info.HarvestId);

                // Store
                _plants["O:" + seedId] = info;
                ModEntry.Instance.Monitor.Log($"Created PlantInfo for ", LogLevel.Alert);

            }
        }

        //-------
        // Crops
        //-------
        public static PlantInfo FromCrop(string id, CropData crop)
        {
            return new PlantInfo
            {
                Id = id,
                PlantType = PlantType.Crop,

                Seasons = crop.Seasons?
                    .Select(s => Enum.Parse<SeasonId>(s.ToString(), ignoreCase: true))
                    .ToList()
                    ?? new List<SeasonId>(),

                DaysToProduce = crop.DaysInPhase?.Sum() ?? 0,
                RegrowDays = crop.RegrowDays > 0 ? crop.RegrowDays : null,

                Trellis = crop.IsRaised,
                Paddy = crop.IsPaddyCrop,
                MultiSprite = crop.TintColors?.Count ?? 0,
                NeedsWatering = crop.NeedsWatering,
                Scythe = crop.HarvestMethod,

                Harvest = new ItemInfo { Id = crop.HarvestItemId ?? "" },

                Drops = new List<PlantInfo.DropInfo>
        {
            new PlantInfo.DropInfo
            {
                ItemId = crop.HarvestItemId ?? "",
                MinStack = crop.HarvestMinStack,
                MaxStack = crop.HarvestMaxStack,
                ExtraHarvestChance = crop.ExtraHarvestChance
            }
        }
            };
        }

        //-----
        //Fruit Trees
        //-------
        private static void LoadFruitTrees()
        {
            foreach (var (saplingId, treeData) in Game1.fruitTreeData)
            {
                var info = FromFruitTree(saplingId, treeData);
                if (info == null)
                    continue;

                // Sapling object data
                info.Seed = FromObject(saplingId);

                // Fruit object data
                if (!string.IsNullOrEmpty(info.Harvest?.Id))
                    info.Harvest = FromObject(info.Harvest.Id);

                _plants["O:" + saplingId] = info;
            }
        }

        private static PlantInfo FromFruitTree(string saplingId, FruitTreeData data)
        {
            // FruitTreeData.Fruit is a list; take the first entry
            var fruitEntry = data.Fruit.FirstOrDefault();

            // Normalize fruit ID (your helper)
            string fruitId = ItemIDtoNumber(fruitEntry?.ItemId ?? "");

            return new PlantInfo
            {
                Id = saplingId,
                PlantType = PlantType.FruitTree,

                // Seed (sapling) and Harvest (fruit) will be filled in LoadFruitTrees()
                Seed = new ItemInfo { Id = saplingId },
                Harvest = new ItemInfo { Id = fruitId },

                Seasons = data.Seasons?
                    .Select(s => Enum.Parse<SeasonId>(s.ToString(), ignoreCase: true))
                    .ToList()
                    ?? new List<SeasonId>(),

                // Fruit trees always take 28 days to mature
                DaysToProduce = 28,

                // After maturity, they produce daily
                RegrowDays = 1,

                Drops = new List<PlantInfo.DropInfo>
        {
            new PlantInfo.DropInfo
            {
                ItemId = fruitId,
                MinStack = fruitEntry?.MinStack ?? 1,
                MaxStack = fruitEntry?.MaxStack ?? 1,
                ExtraHarvestChance = 0f // fruit trees don't use this
            }
        }
            };
        }







































        // -------------------------
        //  LOADERS
        // -------------------------

        //private static void LoadCrops()
        //{
        //    foreach (var (seedId, cropData) in Game1.cropData)
        //    {
        //        var info = FromCrop(seedId, cropData);
        //        if (info == null)
        //            continue;

        //        string key = "O:" + seedId;

        //        //ModEntry.Instance.Monitor.Log(
        //        //    //$"[Seed Info] STORE: seedId='{seedId}', key='{key}', harvest='{cropData.HarvestItemId}'",
        //        //    //LogLevel.Info
        //        //);

        //        _plants[key] = info;
        //    }

        //    //ModEntry.Instance.Monitor.Log(
        //    //    //$"[Seed Info] DONE LOADING: {_plants.Count} entries in _plants",
        //    //    //LogLevel.Info
        //    //);
        //}



        //private static void LoadFruitTrees()
        //{
        //    foreach (var (saplingId, treeData) in Game1.fruitTreeData)
        //    {
        //        var info = FromFruitTree(saplingId, treeData);
        //        if (info == null)
        //            continue;

        //        string key = "O:" + saplingId;

        //        //ModEntry.Instance.Monitor.Log(
        //        //    $"[Seed Info] STORE FRUIT TREE: saplingId='{saplingId}', key='{key}', fruit='{treeData.Fruit}'",
        //        //    LogLevel.Info
        //        //);

        //        _plants[key] = info;
        //    }
        //}

        //private static Dictionary<string, ICustomBushData> _customBushes = new();

        //private static IModHelper Helper;


        /* PAUSEBUSHES
                private static void LoadBushes()
                {
                    try
                    {
                        var data = ModEntry.SHelper.GameContent.Load<Dictionary<string, object>>("furyx639.CustomBush/Data");

                        foreach (var key in data.Keys)
                        {
                            ModEntry.Instance.Monitor.Log($"Bush key: {key}", LogLevel.Warn);
                        }
                    }
                    catch (Exception ex)
                    {
                        ModEntry.Instance.Monitor.Log($"Failed to load Custom Bush data: {ex}", LogLevel.Error);
                    }

                }
        */

        // -------------------------
        //  LOOKUP - Get all the data
        // -------------------------

        //public static PlantInfo FromCrop(string id, CropData crop)
        //{
        //    // Create the seed item to get name/description
        //    var seedItem = ItemRegistry.Create(id);
        //    string seedName = seedItem.DisplayName;
        //    string? seedDescription = seedItem.getDescription();

        //    // Calculate total days to first harvest
        //    int daysToHarvest = crop.DaysInPhase?.Sum() ?? 0;

        //    // Harvest item
        //    string harvestId = crop.HarvestItemId ?? "";
        //    var harvestItem = ItemRegistry.Create(harvestId);
        //    string harvestName = harvestItem.DisplayName;
        //    string? harvestDescription = harvestItem.getDescription();

        //    var drops = new List<PlantInfo.DropInfo>();
        //    if (!string.IsNullOrEmpty(harvestId))
        //    {
        //        drops.Add(new PlantInfo.DropInfo
        //        {
        //            ItemId = harvestId,
        //            Chance = 1f,
        //            MinStack = 1,
        //            MaxStack = 1
        //        });
        //    }

        //    return new PlantInfo
        //    {
        //        Id = id,

        //        SeedName = seedName,
        //        SeedDescription = seedDescription,
        //        PlantType = PlantType.Crop,

        //        HarvestId = harvestId,
        //        HarvestName = harvestName,
        //        HarvestDescription = harvestDescription,

        //        // List of season as strings.
        //        Seasons = crop.Seasons?
        //            .Select(s => s.ToString().ToLowerInvariant())
        //            .ToList()
        //            ?? new List<string>(),

        //        DaysToProduce = daysToHarvest,
        //        RegrowDays = crop.RegrowDays > 0 ? crop.RegrowDays : null,

        //        Drops = drops,


        //        // Flags
        //        Trellis = crop.IsRaised,
        //        Paddy = crop.IsPaddyCrop,
        //        MultiSprite = crop.TintColors?.Count ?? 0,
        //        NeedsWatering = crop.NeedsWatering,
        //        Scythe = crop.HarvestMethod,

        //        //Prices
        //        SeedPrice = seedItem.Price,
        //        HarvestPrice = harvestItem.Price
        //    };
        //}

        //private static PlantInfo FromFruitTree(string saplingId, FruitTreeData data)
        //{
        //    // Extract fruit entry (vanilla + modded both use this)
        //    var fruit = data.Fruit.FirstOrDefault();

        //    // Normalize the ItemId using your helper
        //    string fruitId = ItemIDtoNumber(fruit?.ItemId ?? "");


        //    return new PlantInfo
        //    {
        //        Id = "O:" + saplingId,

        //        // Sapling identity
        //        //SeedName = data.DisplayName?.ToString() ?? "Unknown Sapling",
        //        //SeedDescription = null, // you can fill this later if you want
        //        PlantType = PlantType.FruitTree,

        //        // Fruit identity
        //        HarvestId = fruitId,
        //        //HarvestName = Game1.objectData.TryGetValue(fruitId.Replace("O:", ""), out var obj)
        //        //    ? obj.DisplayName
        //          //  : "Unknown Fruit",
        //        //HarvestDescription = null,

        //        Seasons = data.Seasons
        //            .Select(s => s.ToString())
        //            .ToList(),
        //        // Production timing
        //        DaysToProduce = 28, // fruit trees always take 28 days
        //        RegrowDays = 1,     // produce daily once mature

        //        // Drops (fruit)
        //        Drops = new List<PlantInfo.DropInfo>
        //        {
        //            new PlantInfo.DropInfo
        //            {
        //            ItemId = fruitId,
        //            Chance = fruit?.Chance ?? 1f,
        //            MinStack = fruit?.MinStack ?? 1,
        //            MaxStack = fruit?.MaxStack ?? 1
        //            }

        //        },
        //    };
        //}

        public static string ItemIDtoNumber(string raw)
        {
            if (string.IsNullOrWhiteSpace(raw))
                return "";

            raw = raw.Trim();

            // Vanilla format: (O)634 → 634
            if (raw.StartsWith("(O)"))
                return raw.Substring(3);

            // Mod format: O:634 → 634
            if (raw.StartsWith("O:") || raw.StartsWith("o:"))
                return raw.Substring(2);

            // Pure numeric → already correct
            if (int.TryParse(raw, out _))
                return raw;

            return raw; // fallback
        }

        //-------------------------
        // Build the Seed and Harvest Icons
        //-------------------------

        //private static void BuildSeedIcons()
        //{
        //    foreach (var plant in _plants.Values)
        //    {
        //        if (plant.Id is null)
        //            continue;

        //        var item = ItemRegistry.Create(plant.Id);
        //        if (item is null)
        //            continue;

        //        // Use your helper so modded seeds work automatically
        //        plant.SeedIcon = IconHelper.FromItem(item, scale: 1f);
        //    }
        //}

        //public static class IconHelper
        //{

        //    public static IconRef FromItem(Item item, float scale = 1f)
        //    {
        //        Rectangle src = Game1.getSourceRectForStandardTileSheet(
        //            Game1.objectSpriteSheet,
        //            item.ParentSheetIndex,
        //            16,
        //            16
        //        );

        //        return new IconRef(Game1.objectSpriteSheet, src, 16, scale);
        //    }

        //}


        //private static void BuildHarvestIcons()
        //{
        //    foreach (var plant in _plants.Values)
        //    {
        //        if (plant.HarvestId is null)
        //            continue;

        //        var item = ItemRegistry.Create(plant.HarvestId);
        //        if (item is null)
        //            continue;

        //        Texture2D icon = RenderItemIcon(item, 16);

        //        plant.HarvestIcon = new IconRef(icon, new Rectangle(0, 0, 16, 16));
        //    }
        //}

        //        public static Texture2D NormalizeItemIcon(Item item, int size = 32)
        //        {
        //            ModEntry.Instance.Monitor.Log($"NormalizeItemIcon called", LogLevel.Debug);
        //            var device = Game1.graphics.GraphicsDevice;

        //            const int bufferSize = 64;
        //            var buffer = new RenderTarget2D(device, bufferSize, bufferSize);

        //            device.SetRenderTarget(buffer);
        //            device.Clear(Color.Transparent);

        //            SpriteBatch batch = new SpriteBatch(device);
        //            batch.Begin(
        //                SpriteSortMode.Deferred,
        //                BlendState.AlphaBlend,
        //                SamplerState.PointClamp,
        //                null,
        //                null,
        //                null,
        //                Matrix.Identity   
        //            );

        //            // ⭐ Compute centered position
        //            Vector2 pos = new Vector2(
        //                (bufferSize - 16 * 4) / 2,   // 16px icon × 4 scale
        //                (bufferSize - 16 * 4) / 2
        //            );

        //            // ⭐ Draw centered at scale 4
        //            item.drawInMenu(
        //                batch,
        //                pos,
        //                4f,
        //                1f,
        //                1f,
        //                StackDrawType.Hide,
        //                Color.White,
        //                false
        //            );
        //            ModEntry.Instance.Monitor.Log(
        //    $"Buffer has data? {buffer.Bounds.Width}x{buffer.Bounds.Height}",
        //    LogLevel.Alert
        //);

        //            batch.End();
        //            device.SetRenderTarget(null);

        //            // Downscale to final size
        //            var final = new RenderTarget2D(device, size, size);

        //            device.SetRenderTarget(final);
        //            device.Clear(Color.Transparent);

        //            batch.Begin(
        //                SpriteSortMode.Deferred,
        //                BlendState.AlphaBlend,
        //                SamplerState.PointClamp,
        //                null,
        //                null,
        //                null,
        //                Matrix.Identity   
        //            );

        //            float scale = size / (float)bufferSize;
        //            batch.Draw(buffer, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);

        //            batch.End();
        //            device.SetRenderTarget(null);

        //            //add some debug below this
        //            Color[] pixels = new Color[size * size];
        //            final.GetData(pixels);

        //            bool anyOpaque = pixels.Any(p => p.A > 0);

        //            ModEntry.Instance.Monitor.Log(
        //                $"Final icon has opaque pixels: {anyOpaque}",
        //                LogLevel.Alert
        //            );

        //            return final;
        //        }

        public static Texture2D RenderItemIcon(Item item, int size = 16)
        {
            var device = Game1.graphics.GraphicsDevice;

            // Step 1: draw into a 64x64 buffer (the size drawInMenu expects)
            const int bufferSize = 64;
            var buffer = new RenderTarget2D(device, bufferSize, bufferSize);

            var oldTargets = device.GetRenderTargets();

            device.SetRenderTarget(buffer);
            device.Clear(Color.Transparent);

            // ⭐ CRITICAL: use the game's spriteBatch, just like before
            SpriteBatch batch = Game1.spriteBatch;

            batch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp);

            item.drawInMenu(
                batch,
                Vector2.Zero,
                1f,            // natural scale (same as before)
                1f,
                1f,
                StackDrawType.Hide,
                Color.White,
                false
            );

            batch.End();
            device.SetRenderTarget(null);

            // Step 2: downscale the 64x64 buffer into your final 16x16 icon
            var final = new RenderTarget2D(device, size, size);

            device.SetRenderTarget(final);
            device.Clear(Color.Transparent);

            batch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp);

            float scale = size / (float)bufferSize;
            batch.Draw(buffer, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);

            batch.End();
            device.SetRenderTarget(null);

            return final;
        }

        //public static PlantInfo? FromItem(StardewValley.Object obj)
        //{
        //    string id = obj.QualifiedItemId;
        //    return _plants.TryGetValue(id, out var info) ? info : null;
        //}


        /* PAUSE BUSHES NOTHING BELOW HERE WORKS YET

                public static bool TryGetCornucopiaBushInfo(StardewValley.Object obj, out PlantInfo? info)
                {
                    info = null;

                    //pasting in
                    // The bush ID is literally the sapling's QualifiedItemId
                    string bushId = obj.QualifiedItemId;

                    // Get the Cornucopia Data from the json
                    string path = Path.Combine(
                        ModEntry.Instance.Helper.DirectoryPath,
                        "..",
                        "[CP] Cornucopia More Crops",
                        "data",
                        "teabushes.json"
                    );

                    path = Path.GetFullPath(path);

                    var json = File.ReadAllText(path);
                    var dict = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);

                    //foreach (var key in dict.Keys)
                    //{
                    //    ModEntry.Instance.Monitor.Log($"Bush key: {key}", LogLevel.Warn);
                    //}

                    if (dict.TryGetValue("(O)Cornucopia_RaspberrySeeds", out var raw))
                    {
                        var jo = raw as JObject;
                        if (jo != null)
                        {
                            int temp = jo["AgeToProduce"]?.Value<int>() ?? -1;
                            ModEntry.Instance.Monitor.Log($"TEST AgeToProduce = {temp}", LogLevel.Warn);
                        }
                        else
                        {
                            ModEntry.Instance.Monitor.Log("TEST: raw is not a JObject", LogLevel.Warn);
                        }
                    }
                    else
                    {
                        ModEntry.Instance.Monitor.Log("TEST: key not found in _bushes", LogLevel.Warn);
                    }


                    ModEntry.Instance.Monitor.Log($"QualifiedItemId: {bushId}", LogLevel.Warn);
                    //ModEntry.Instance.Monitor.Log($"Otherfield: {dict.DisplayName}", LogLevel.Warn);





                    var md = obj.modData;
                    ModEntry.Instance.Monitor.Log($"ModData: {md}", LogLevel.Warn);


                    if (!md.ContainsKey("Cornucopia.MoreCrops/AgeToProduce"))
                        return false; // not a Cornucopia bush sapling

                    // Age
                    int regrowDays = int.TryParse(md["Cornucopia.MoreCrops/AgeToProduce"], out var age)
                        ? age
                        : 0;

                    // Seasons
                    var seasons = md.TryGetValue("Cornucopia.MoreCrops/Seasons", out var seasonStr)
                        ? seasonStr.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList()
                        : new List<string>();

                    // Drops (JSON)
                    List<PlantInfo.DropInfo> drops = new();
                    if (md.TryGetValue("Cornucopia.MoreCrops/ItemsProduced", out var dropJson))
                    {
                        try
                        {
                            var parsed = JsonConvert.DeserializeObject<List<dynamic>>(dropJson);
                            foreach (var d in parsed)
                            {
                                drops.Add(new PlantInfo.DropInfo
                                {
                                    ItemId = d.ItemId?.ToString()?.Replace("(O)", "O:") ?? "O:0",
                                    Chance = (float?)d.Chance ?? 1f,
                                    MinStack = (int?)d.MinStack ?? 1,
                                    MaxStack = (int?)d.MaxStack ?? 1
                                });
                            }
                        }
                        catch { }
                    }


                info = new PlantInfo
                    {
                        Id = obj.QualifiedItemId,
                        SeedName = obj.DisplayName,
                        SeedDescription = obj.getDescription(),
                        Seasons = seasons,
                        RegrowDays = regrowDays,
                        Drops = drops
                    };

                    return true;
                }
        */



        //private static PlantInfo FromCustomBushSapling(string id, IList<ICustomBushDrop> drops)
        //{
        //    var convertedDrops = drops.Select(d => new PlantInfo.DropInfo
        //    {
        //        ItemId = d.ItemId.Replace("(O)", "O:"),
        //        Chance = d.Chance,
        //        MinStack = d.MinStack,
        //        MaxStack = d.MaxStack
        //    }).ToList();

        //    return new PlantInfo
        //    {
        //        Id = id,
        //        SeedName = Game1.objectData[id].DisplayName,
        //        SeedDescription = Game1.objectData[id].Description,

        //        Seasons = drops
        //            .Select(d => d.Season?.ToString().ToLower())
        //            .Where(s => s != null)
        //            .Distinct()
        //            .ToList()!,

        //        RegrowDays = null, // TBD — custom bushes vary

        //        Drops = convertedDrops
        //    };
        //}
    }
}



