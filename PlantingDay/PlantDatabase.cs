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
using StardewValley.Locations;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using System.Collections.Generic;
using System.Xml.Linq;
using SObject = StardewValley.Object;
//using System.Linq;
//using System.Text.Json;

namespace PlantingDay
{

    public static class PlantDatabase
    {
        private static bool _initialized;
        private static readonly Dictionary<string, PlantInfo> _plants = new();
        public static IEnumerable<PlantInfo> AllPlants => _plants.Values;

        private static Dictionary<string, List<(int itemId, float chance)>> _monsterDropTable = new();

        public static void Initialize()
        {
            ModEntry.Instance.Monitor.Log("PlantDatabase.Load CALLED", LogLevel.Alert);
            //if (_initialized)
            //    return;

            _initialized = true;
            LoadMonsterDropTable();

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
                var plant = FromCrop(seedId, cropData);
                if (plant == null)
                    continue;
                plant.PurchaseOptions = GetPurchaseInfo(seedId);
                plant.MonsterDrops = GetMonsterDropsForItem(seedId);

                PlantDatabase.InitializeIcons(plant);


                _plants["O:" + seedId] = plant;
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

                Drops = new List<DropInfo>
                    {
                    new DropInfo
                    {
                        ItemId = harvestId,
                        MinStack = crop.HarvestMinStack,
                        MaxStack = crop.HarvestMaxStack,
                        ExtraHarvestChance = crop.ExtraHarvestChance
                    }
                }.AsReadOnly(),

            };
        }

        //-----
        //Fruit Trees
        //-------
        private static void LoadFruitTrees()
        {
            foreach (var (saplingId, treeData) in Game1.fruitTreeData)
            {
                var plant = FromFruitTree(saplingId, treeData);
                if (plant == null)
                    continue;
                plant.PurchaseOptions = GetPurchaseInfo(saplingId);
                plant.MonsterDrops = GetMonsterDropsForItem(saplingId);

                PlantDatabase.InitializeIcons(plant);


                _plants["O:" + saplingId] = plant;
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

                Drops = new List<DropInfo>
                    {
                    new DropInfo
                    {
                        ItemId = fruitId,
                        MinStack = fruitEntry?.MinStack ?? 1,
                        MaxStack = fruitEntry?.MaxStack ?? 1,
                        ExtraHarvestChance = 0f
                    }
                
                }.AsReadOnly(),

            };
        }










        public static string NormalizeItemId(string raw)
        {
            if (string.IsNullOrWhiteSpace(raw))
                return "";

            raw = raw.Trim();

            // Strip (X)### → ###
            if (raw.StartsWith("(") && raw.Contains(')'))
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

        public static void InitializeIcons(PlantInfo plant)
        {
            if (plant.Seed != null)
            {
                var item = ItemRegistry.Create(plant.Seed.Id);
                if (item != null)
                    plant.SeedIconTexture = PlantDatabase.RenderItemIcon(item, 16);

            }

            if (plant.Harvest != null)
            {
                var item = ItemRegistry.Create(plant.Harvest.Id);
                if (item != null)
                    plant.HarvestIconTexture = PlantDatabase.RenderItemIcon(item, 32);
            }
        }

        public static Texture2D RenderItemIcon(Item item, int size = 16)
        {
            var device = Game1.graphics.GraphicsDevice;

            // Step 1: draw into a 64x64 buffer (the size drawInMenu expects)
            const int bufferSize = 64;
            var buffer = new RenderTarget2D(device, bufferSize, bufferSize);

            //var oldTargets = device.GetRenderTargets();

            device.SetRenderTarget(buffer);
            device.Clear(Color.Transparent);

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

        //TODO - check crops from all mods. uncle iron sugarcane not working
        //TODO - check sunberry shops
        private static List<PurchaseInfo> GetPurchaseInfo(string itemId)
        {
            var results = new List<PurchaseInfo>();

            var shops = Game1.content.Load<Dictionary<string, ShopData>>("Data/Shops");

            //ModEntry.Instance.Monitor.Log($"Got purchase info:", LogLevel.Info);

            foreach (var (shopId, shop) in shops)
            {
                if (BlacklistedShops.Contains(shopId))
                    continue;

                foreach (var entry in shop.Items)
                {
                    bool directMatch = NormalizeItemId(entry.ItemId) == NormalizeItemId(itemId);
                    bool wildcardMatch = entry.ItemId == "ALL_ITEMS (O)" && ItemMatchesWildcard(itemId, entry);

                    if (!directMatch && !wildcardMatch)
                        continue;

                    var info = new PurchaseInfo
                    {
                        VendorId = shopId,
                        VendorName = GetVendorName(shopId),
                        Condition = entry.Condition
                    };

                    //
                    // TRADE CURRENCY (non-gold)
                    //
                    if (!string.IsNullOrEmpty(entry.TradeItemId))
                    {
                        info.TradeItemId = entry.TradeItemId;
                        info.TradeAmount = entry.TradeItemAmount;

                        // Assign correct icon tag
                        info.CurrencyIconRef = GetCurrencyIconRef(entry.TradeItemId);
                    }
                    else
                    {
                        //
                        // GOLD CURRENCY TODO FIX BOTH PIERRE AND OTHER SHOP PRICE
                        //

                        if (entry.Price >= 0)
                            info.GoldPrice = entry.Price;
                        else
                            info.GoldPrice = GetDefaultShopPrice(shopId, itemId);
                    }

                    results.Add(info);
                }

            }


            return results;
        }
        private static IconRef? GetCurrencyIconRef(string tradeItemId)
        {
            if (string.IsNullOrWhiteSpace(tradeItemId))
                return null;

            // Create the item
            Item item = ItemRegistry.Create(tradeItemId);
            if (item == null)
                return null;

            // Get the item data (this is where the sprite lives in 1.6)
            var data = ItemRegistry.GetData(item.QualifiedItemId);
            if (data == null)
                return null;

            // Texture and source rect come from ItemData
            Texture2D texture = data.GetTexture();
            Rectangle source = data.GetSourceRect();

            // Build your icon
            return new IconRef(texture, source, size: source.Width, scale: 2f);
        }

        private static bool ItemMatchesWildcard(string itemId, ShopItemData entry)
        {
            // Wildcard entries must have PerItemCondition
            if (string.IsNullOrWhiteSpace(entry.PerItemCondition))
                return false;

            // Create the item instance
            Item item = ItemRegistry.Create(itemId);
            if (item == null)
                return false;

            // Get the item's context tags
            var tags = item.GetContextTags();

            // Split the PerItemCondition into individual rules
            // Example:
            // "ITEM_CONTEXT_TAG Target cornucopia_shop_pierre, ITEM_CONTEXT_TAG Target cornucopia_season_spring, !ITEM_CONTEXT_TAG Target cornucopia_shop_pierre_generic_banned"
            var rules = entry.PerItemCondition
                .Split(',')
                .Select(r => r.Trim())
                .Where(r => r.Length > 0);

            foreach (var rule in rules)
            {
                bool negated = rule.StartsWith("!");
                string cleanRule = negated ? rule.Substring(1).Trim() : rule;

                // We only care about ITEM_CONTEXT_TAG rules
                if (!cleanRule.StartsWith("ITEM_CONTEXT_TAG", StringComparison.OrdinalIgnoreCase))
                    continue;

                // Extract the tag name (after "Target ")
                int idx = cleanRule.IndexOf("Target ", StringComparison.OrdinalIgnoreCase);
                if (idx < 0)
                    continue;

                string requiredTag = cleanRule.Substring(idx + "Target ".Length).Trim();

                bool hasTag = tags.Contains(requiredTag);

                // If rule is negated, item must NOT have the tag
                if (negated)
                {
                    if (hasTag)
                        return false;
                }
                else
                {
                    if (!hasTag)
                        return false;
                }
            }

            // Passed all rules
            return true;
        }

        private static string GetVendorName(string shopId)
        {
            return shopId switch
            {
                "SeedShop" => "Pierre",
                "Joja" => "Joja",
                "Sandy" => "Oasis",
                "AnimalShop" => "Marnie",
                "IslandTrade" => "Island Trader",
                "DesertTrade" => "Desert Trader",
                "Traveler" => "Traveling Cart",

                //festivals
                "Festival_Luau_Pierre" => "Luau",
                "Festival_EggFestival_Pierre" => "Egg Festival",
                "Festival_StardewValleyFair_StarTokens" => "Valley Fair",

                // Collapse Desert Festival
                _ when shopId.StartsWith("DesertFestival", StringComparison.OrdinalIgnoreCase)
                    => string.Join(", ",
                    shopId
                    .Split(',')
                    .Select(id => id.Replace("DesertFestival_", ""))
            ),


                //sunberry
                "skellady.SBVCP_AriMarket" => "Ari",
                "skellady.SBVCP_JumanaShop" => "Jumana",

                _ => shopId // fallback for modded shops
            };
        }

        private static int GetDefaultShopPrice(string shopId, string itemId)
        {
            var obj = ItemRegistry.Create(itemId) as StardewValley.Object;
            if (obj == null)
                return 0;

            int sellPrice = obj.sellToStorePrice();
            //ModEntry.Instance.Monitor.Log($"sell price: {sellPrice}", LogLevel.Info);

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

        //-------------
        // Monster Drops
        //--------------


        // Which seeds are monster drop loot
        public static List<MonsterDropInfo> GetMonsterDropsForItem(string itemId)
        {
            var result = new List<MonsterDropInfo>();

            // Convert string → int
            if (!int.TryParse(itemId, out int id))
                return result; // invalid ID, no drops

            foreach (var (monster, drops) in _monsterDropTable)
            {
                foreach (var (dropId, chance) in drops)
                {
                    if (dropId == id)
                    {
                        result.Add(new MonsterDropInfo
                        {
                            MonsterName = monster,
                            Chance = chance,
                            MonsterIcon = LoadMonsterIcon(monster)
                        });
                    }
                }
            }

            return result;
        }
        private static void LoadMonsterDropTable()
        {
            _monsterDropTable = new();

            var monsters = Game1.content.Load<Dictionary<string, string>>("Data/Monsters");

            foreach (var (monsterName, raw) in monsters)
            {
                string[] fields = raw.Split('/');

                if (fields.Length <= 6)
                    continue;

                string dropField = fields[6];
                var parts = dropField.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                var drops = new List<(int, float)>();

                for (int i = 0; i < parts.Length - 1; i += 2)
                {
                    if (int.TryParse(parts[i], out int id) &&
                        float.TryParse(parts[i + 1], out float chance))
                    {
                        drops.Add((id, chance));
                    }
                }

                if (drops.Count > 0)
                    _monsterDropTable[monsterName] = drops;
            }
        }

        private static IconRef? LoadMonsterIcon(string monsterName)
        {
            try
            {
                // Load monster metadata
                var monsters = Game1.content.Load<Dictionary<string, string>>("Data/Monsters");
                if (!monsters.TryGetValue(monsterName, out string? raw))
                    return null;

                string[] fields = raw.Split('/');

                // Field 0 = sprite path
                string texturePath = fields[0];

                // Field 1 = frame width
                int frameWidth = int.Parse(fields[1]);

                // Field 2 = frame height
                int frameHeight = int.Parse(fields[2]);

                // Load texture
                Texture2D tex = Game1.content.Load<Texture2D>(texturePath);

                // First frame is always at (0,0)
                Rectangle source = new Rectangle(0, 0, frameWidth, frameHeight);

                return new IconRef(tex, source, frameWidth, scale: 2f);
            }
            catch
            {
                return null;
            }
        }


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
public class ModShopEntry
{
    public int Price { get; set; }
    public int Stock { get; set; }
}



