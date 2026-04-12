using Microsoft.Xna.Framework.Graphics;
using PlantingDay.Compatibility;
using PlantingDay.Helpers;
using PlantingDay.Helpers.SeedSource;
using PlantingDay.Models;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Framework.ModLoading;
using StardewValley;
using SDVCommon;
using SDVCommon.Icons;
using SDVData;
using PlantingDay.Helpers.Icons;


namespace PlantingDay
{
    public class ModEntry : Mod
    {
        public static ModEntry Instance { get; private set; } = null!;
        public static ICustomBushApi? CustomBushApi { get; private set; }
        public static IModHelper ModHelper { get; private set; } = null!;
        public static IMonitor ModMonitor { get; private set; } = null!;

        public override void Entry(IModHelper helper)
        {
            Instance = this;
            ModHelper = helper;
            ModEntry.ModMonitor = base.Monitor;



            //helper.Events.GameLoop.GameLaunched += OnGameLaunched;

            helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;

            helper.Events.Display.RenderedActiveMenu += OnRenderedActiveMenu;

            helper.Events.Input.ButtonPressed += OnButtonPressed;
        }


        private void OnGameLaunched(object? sender, StardewModdingAPI.Events.GameLaunchedEventArgs e)
        {

        }

        private void OnSaveLoaded(object? sender, SaveLoadedEventArgs e)
        {
            InitializeAll();

        


            //string dataPath = ModEntry.ModHelper.DirectoryPath;

            //foreach (string file in Directory.GetFiles(dataPath, "shop_*.json"))
            //{
            //    string vendorId = Path.GetFileNameWithoutExtension(file);
            //    //ModEntry.Instance.Monitor.Log($"[Planting Day] Found mod shop: {vendorId}", LogLevel.Info);
            //}

        }

        [EventPriority(EventPriority.Low - 1)]
        private void OnRenderedActiveMenu(object? sender, RenderedActiveMenuEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            Item? hovered = SDVCommon.TooltipRenderer.GetHoveredItemFromAnyMenu();
            if (hovered is not StardewValley.Object obj)
                return;



            // Use the correct key format O:#### to see if the item is in the plant library
            string lookupKey = obj.ItemId;

            var plant = PlantInfoBuilder.LookupFromKey(lookupKey);

            if (plant is null)
                return;
            //ModEntry.Instance.Monitor.Log($"Harvest ID for {plant.Data.Seed?.Name} = {plant.Data.Harvest?.Id}", LogLevel.Info);


            var elements = TooltipBuilder.BuildTooltip(plant);

            SDVCommon.TooltipRenderer.DrawTooltip(e.SpriteBatch, elements);

            //SDVFramework.TooltipRenderer.DrawMenu(e.SpriteBatch);

            //if (Game1.activeClickableMenu is ShopMenu shop)
            //{
            //    Monitor.Log("=== PIERRE SHOP DUMP ===", LogLevel.Warn);

            //    foreach (var salable in shop.forSale)
            //    {
            //        var item = salable as Item;
            //        if (item == null)
            //            continue;

            //        // 1. Get the stock info (price, stock, etc.)
            //        if (!shop.itemPriceAndStock.TryGetValue(salable, out var stockInfo))
            //            continue;

            //        int price = stockInfo.Price;   // <-- THIS is the final price Pierre is charging

            //        Monitor.Log(
            //            $"{item.Name} (ID: {item.ParentSheetIndex}) - Price: {price}",
            //            LogLevel.Warn
            //        );
            //    }
            //}
        }






        private void OnButtonPressed(object? sender, StardewModdingAPI.Events.ButtonPressedEventArgs e)
        {
            // Only run when the player presses F5
            if (e.Button != SButton.F5)
                return;
            PlantInfoBuilder.Reset();
            HarvestInfoBuilder.Reset();

            InitializeAll();






            //KEEP Debug to output desired database variable from a list
            //foreach (var plant in PlantInfoBuilder.AllPlants)
            //{
            //    foreach (var option in plant.Data.PurchaseOptions)
            //    {
            //        ModEntry.Instance.Monitor.Log(
            //            $"Seed: {plant.Data.SeedId} Vendor: {option.VendorName} Price: {option.GoldPrice}",
            //            LogLevel.Warn
            //        );
            //    }
            //}


            ModEntry.Instance.Monitor.Log($"[{DateTime.Now:HH:mm:ss}] RAN BUTTON PRESS", LogLevel.Alert);



            // KEEP Shows all game shops available. Useful when needing to fix mod shops
            /*
             var shops = Game1.content.Load<Dictionary<string, ShopData>>("Data/Shops");
            foreach (var shopId in shops.Keys)
            {
                ModEntry.Instance.Monitor.Log($"[Planting Day] Found shop: {shopId}", LogLevel.Info);
            }
            foreach (var pack in Helper.ContentPacks.GetOwned())
            {
                foreach (string file in Directory.GetFiles(pack.DirectoryPath, "shop_*.json", SearchOption.AllDirectories))
                {
                    Monitor.Log($"[Planting Day] Found mod shop file: {file}", LogLevel.Info);
                }
            }
            */



        }

        private static void InitializeAll()
        {
            TooltipIcons.Initialize();
            MonsterDropLoader.Initialize();
            PlantInfoBuilder.Initialize();
            HarvestInfoBuilder.Initialize();


            foreach (var plant in PlantInfoBuilder.AllPlants)
            {
                PlantIconInitializer.InitializeIcons(plant);
            }
            foreach (var harvest in HarvestInfoBuilder.AllHarvests)
            {
                HarvestIconInitializer.InitializeIcons(harvest);
            }

            CacheForTesting.DumpPlantInfoToJson();

        }


    }
}

//private static bool DebugMatchesWildcard(string itemId, ShopItemData entry)
//{
//    // Get the actual item object
//    Item item = ItemRegistry.Create(itemId);
//    if (item == null)
//        return false;

//    // Get its context tags
//    var tags = item.GetContextTags();

//    // Cornucopia uses tags like:
//    //   cornucopia_shop_pierre
//    //   cornucopia_season_spring
//    //   modid_cornucopia.morecrops

//    // So we check if the PerItemCondition mentions any required tags
//    string cond = entry.PerItemCondition ?? "";

//    bool requiresPierre = cond.Contains("cornucopia_shop_pierre");
//    bool requiresSpring = cond.Contains("cornucopia_season_spring");
//    bool requiresCornucopia = cond.Contains("modid_cornucopia.morecrops");

//    bool hasPierre = tags.Contains("cornucopia_shop_pierre");
//    bool hasSpring = tags.Contains("cornucopia_season_spring");
//    bool hasCornucopia = tags.Any(t => t.StartsWith("modid_cornucopia.morecrops"));

//    return (!requiresPierre || hasPierre)
//        && (!requiresSpring || hasSpring)
//        && (!requiresCornucopia || hasCornucopia);
//}
/*DEBUG STUFF NOTHING BELOW HERE

        if (ModEntry.Instance == null)
        {
            Monitor.Log("[Seed Info] ERROR: ModEntry.Instance is NULL!", LogLevel.Error);
            return;
        }


        if (!Context.IsWorldReady)
            return;

        Item item = Game1.player.CurrentItem;
        if (item == null)
            return;

        Monitor.Log($"[Seed Info] Held item type: {item.GetType().FullName}", LogLevel.Warn);

        if (item is not StardewValley.Object obj)
        {
            Monitor.Log("[Seed Info] Item is not a StardewValley.Object — skipping bush test.", LogLevel.Warn);
            return;
        }

        Monitor.Log($"[Seed Info] Running TryGetCornucopiaBushInfo on {obj.QualifiedItemId}", LogLevel.Warn);

        // Call your method
        if (PlantDatabase.TryGetCornucopiaBushInfo(obj, out PlantInfo? info))
        {
            Monitor.Log("[Seed Info] TryGetCornucopiaBushInfo returned TRUE.", LogLevel.Warn);

            if (info == null)
            {
                Monitor.Log("[Seed Info] But PlantInfo is NULL (unexpected).", LogLevel.Warn);
            }
            else
            {
                Monitor.Log($"[Seed Info] PlantInfo created:", LogLevel.Warn);
                //Monitor.Log($"  Name: {info.Name}", LogLevel.Warn);
                //Monitor.Log($"  Type: {info.Type}", LogLevel.Warn);
                //Monitor.Log($"  Seasons: {string.Join(", ", info.Seasons ?? new())}", LogLevel.Warn);
                //Monitor.Log($"  AgeToProduce: {info.AgeToProduce}", LogLevel.Warn);
                //Monitor.Log($"  Texture: {info.TexturePath}", LogLevel.Warn);
                //Monitor.Log($"  SpriteRow: {info.TextureSpriteRow}", LogLevel.Warn);
            }
        }
        else
        {
            Monitor.Log("[Seed Info] TryGetCornucopiaBushInfo returned FALSE.", LogLevel.Warn);
        }
    }

    // -------------------------
    // TEST 1: Load Custom Bush Data
    // -------------------------
    //Monitor.Log("=== Test 1: Loading furyx639.CustomBush/Data ===", LogLevel.Warn);

    //Dictionary<string, object>? bushDb = null;

    //try
    //{
    //    bushDb = Helper.GameContent.Load<Dictionary<string, object>>("furyx639.CustomBush/Data");
    //    Monitor.Log($"Loaded bush database with {bushDb.Count} entries.", LogLevel.Warn);
    //}
    //catch (Exception ex)
    //{
    //    Monitor.Log($"Failed to load bush data: {ex}", LogLevel.Error);
    //    return;
    //}

    // -------------------------
    // TEST 2: Does this sapling map to a bush?
    // -------------------------
    //Monitor.Log("=== Test 2: Checking sapling → bush mapping ===", LogLevel.Warn);

    //    if (bushDb.TryGetValue(saplingId, out var rawEntry))
    //    {
    //        Monitor.Log($"Bush entry FOUND for {saplingId}.", LogLevel.Warn);

    //        // Dump the raw JSON object (it will appear as a JsonElement or Dictionary)
    //        Monitor.Log($"Raw bush entry: {rawEntry}", LogLevel.Warn);
    //    }
    //    else
    //    {
    //        Monitor.Log($"No bush entry found for {saplingId}.", LogLevel.Warn);
    //    }

    //    // -------------------------
    //    // TEST 3: Check ObjectData.CustomFields
    //    // -------------------------
    //    Monitor.Log("=== Test 3: ObjectData.CustomFields ===", LogLevel.Warn);

    //    if (Game1.objectData.TryGetValue(obj.ItemId, out var od))
    //    {
    //        if (od.CustomFields == null || od.CustomFields.Count == 0)
    //        {
    //            Monitor.Log("No CustomFields found.", LogLevel.Warn);
    //        }
    //        else
    //        {
    //            foreach (var kvp in od.CustomFields)
    //                Monitor.Log($"{kvp.Key} = {kvp.Value}", LogLevel.Warn);
    //        }
    //    }
    //    else
    //    {
    //        Monitor.Log("ObjectData entry not found.", LogLevel.Warn);
    //    }
    //}
*/





//private void OnRenderedHud(object? sender, RenderedHudEventArgs e)
//{

//    TooltipRenderer.DrawHud(e.SpriteBatch);
//}

//    //ModEntry.Instance.Monitor.Log("HERE", LogLevel.Info);













