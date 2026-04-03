using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PlantingDay.Compatibility;
using PlantingDay.Helpers;
using PlantingDay.Models;
using StardewModdingAPI;
using StardewValley;
using StardewValley.GameData;
using StardewValley.GameData.Buffs;
using StardewValley.GameData.Crops;
using StardewValley.GameData.FruitTrees;
using StardewValley.GameData.Objects;
using StardewValley.GameData.Shops;
using StardewValley.Objects;
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
                info.PurchaseOptions = GetPurchaseInfo(seedId);

                _plants["O:" + seedId] = info;
            }
        }

        //-------
        // Crops
        //-------
        public static PlantInfo FromCrop(string id, CropData crop)
        {
            var seedInfo = FromObject(id);

            var harvestId = crop.HarvestItemId ?? "";
            var harvestInfo = FromObject(harvestId);

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

                Seed = seedInfo,
                Harvest = harvestInfo,
                HarvestPrice = harvestInfo?.Price ?? 0,

                Drops = new List<PlantInfo.DropInfo>
        {
            new PlantInfo.DropInfo
            {
                ItemId = harvestId,
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
                info.PurchaseOptions = GetPurchaseInfo(saplingId);


                _plants["O:" + saplingId] = info;
            }
        }

        private static PlantInfo FromFruitTree(string saplingId, FruitTreeData data)
        {
            // Fruit tree always has exactly one fruit entry
            var fruitEntry = data.Fruit.FirstOrDefault();
            string fruitId = NormalizeItemId(fruitEntry?.ItemId ?? "");

            // Load seed (sapling) and harvest (fruit) using your existing FromObject()
            var seedInfo = FromObject(saplingId);
            var harvestInfo = FromObject(fruitId);

            return new PlantInfo
            {
                Id = saplingId,
                PlantType = PlantType.FruitTree,

                Seed = seedInfo,
                Harvest = harvestInfo,
                HarvestPrice = harvestInfo?.Price ?? 0,

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
                ExtraHarvestChance = 0f
            }
        }
            };
        }










        public static string NormalizeItemId(string raw)
        {
            if (string.IsNullOrWhiteSpace(raw))
                return "";

            raw = raw.Trim();

            // Strip (X)### → ###
            if (raw.StartsWith("(") && raw.Contains(")"))
            {
                int close = raw.IndexOf(')');
                return raw.Substring(close + 1);
            }

            // Strip X:### → ###
            int colonIndex = raw.IndexOf(':');
            if (colonIndex > 0)
                return raw.Substring(colonIndex + 1);

            // Already numeric
            if (int.TryParse(raw, out _))
                return raw;

            return raw;
        }

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

        //------
        // Economics
        //-------

        private static List<PurchaseInfo> GetPurchaseInfo(string itemId)
        {
            var results = new List<PurchaseInfo>();

            var shops = Game1.content.Load<Dictionary<string, ShopData>>("Data/Shops");


            foreach (var (shopId, shop) in shops)
            {
                if (BlacklistedShops.Contains(shopId))
                    continue;

                foreach (var entry in shop.Items)
                {
                    if (NormalizeItemId(entry.ItemId) != NormalizeItemId(itemId))
                        continue;

                    var info = new PurchaseInfo
                    {
                        VendorId = shopId,
                        VendorName = GetVendorName(shopId),
                        Condition = entry.Condition
                    };

                    // Trade
                    if (entry.TradeItemId != null)
                    {
                        info.TradeItemId = entry.TradeItemId;
                        info.TradeAmount = entry.TradeItemAmount;
                    }
                    else
                    {
                        // Gold price
                        if (entry.Price >= 0)
                            info.GoldPrice = entry.Price;
                        else
                            info.GoldPrice = GetDefaultShopPrice(shopId, itemId);
                    }

                    results.Add(info);
                }
            }

            // If Pierre doesn't appear in the shop data, generate a default price
            //bool hasPierre = results.Any(r => r.VendorId == "SeedShop");

            //if (!hasPierre)
            //{
            //    int defaultPrice = GetDefaultShopPrice("SeedShop", itemId);

            //    if (defaultPrice > 0)
            //    {
            //        results.Add(new PurchaseInfo
            //        {
            //            VendorId = "SeedShop",
            //            VendorName = GetVendorName(shopId),
            //            GoldPrice = defaultPrice,
            //            Condition = null
            //        });

            //        ModEntry.Instance.Monitor.Log(
            //            $"[Planting Day] Added default Pierre price {defaultPrice} for {itemId}",
            //            LogLevel.Info
            //        );
            //    }
            //}

            return results;
        }

        private static string GetVendorName(string shopId)
        {
            return shopId switch
            {
                "SeedShop" => "Pierre",
                "Joja" => "Joja",
                "Sandy" => "Oasis",

                _ => shopId // fallback for modded shops
            };
        }

        private static int GetDefaultShopPrice(string shopId, string itemId)
        {
            var obj = ItemRegistry.Create(itemId) as StardewValley.Object;
            if (obj == null)
                return 0;

            int sellPrice = obj.sellToStorePrice();
            ModEntry.Instance.Monitor.Log($"sell price: {sellPrice}", LogLevel.Info);

            return shopId switch
            {
                "SeedShop" => sellPrice * 2,
                "Joja" => sellPrice * 3,
                "Sandy" => sellPrice * 2,
                //"Dwarf" => sellPrice * 2,
                _ => sellPrice * 2
            };
        }

        // Shops to not display price from
        private static readonly HashSet<string> BlacklistedShops = new()
            {
                "Joja"
                // add more as needed
            };


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



