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
using SDVCommon.Helpers;
using SDVData;
using PlantingDay.Helpers.Icons;
using PlantingDay.Services;


namespace PlantingDay
{
    public class ModEntry : Mod
    {
        public static ModEntry Instance { get; private set; } = null!;
        public static IModHelper ModHelper { get; private set; } = null!;
        public static IMonitor ModMonitor { get; private set; } = null!;

        public override void Entry(IModHelper helper)
        {
            Instance = this;
            ModHelper = helper;
            ModEntry.ModMonitor = base.Monitor;

            SDVCommonLog.Initialize(this.Monitor);


            //helper.Events.GameLoop.GameLaunched += OnGameLaunched;

            helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;

            helper.Events.Display.RenderedActiveMenu += OnRenderedActiveMenu;

            helper.Events.Input.ButtonPressed += OnButtonPressed;
        }


        private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
        {

        }

        private void OnSaveLoaded(object? sender, SaveLoadedEventArgs e)
        {
            Initializer.InitializeAll(ModHelper);

        


            //string dataPath = ModEntry.ModHelper.DirectoryPath;

            //foreach (string file in Directory.GetFiles(dataPath, "shop_*.json"))
            //{
            //    string vendorId = Path.GetFileNameWithoutExtension(file);
            //    //ModEntry.Instance.Monitor.Log($"[Planting Day] Found mod shop: {vendorId}", LogLevel.Info);
            //}

        }

        [EventPriority(EventPriority.Low-1)]
        private void OnRenderedActiveMenu(object? sender, RenderedActiveMenuEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            Item? hovered = HoveredItem.GetFromAnyMenu();
            if (hovered is not StardewValley.Object obj)
                return;


            // Use the correct key format O:#### to see if the item is in the plant library
            string lookupKey = obj.ItemId;

            var plant = PlantInfoBuilder.LookupFromKey(lookupKey);

            if (plant is null)
                return;
            //ModEntry.Instance.Monitor.Log($"{plant.Data.Seed?.Name} = {plant.Data.Location}", LogLevel.Info);


            var elements = TooltipBuilder.BuildTooltip(plant);

            TooltipRenderer.DrawLeftOfCursor(e.SpriteBatch, elements);

        }






        private void OnButtonPressed(object? sender, StardewModdingAPI.Events.ButtonPressedEventArgs e)
        {
            // Only run when the player presses F5
            if (e.Button != SButton.F5)
                return;
            PlantInfoBuilder.Reset();
            HarvestInfoBuilder.Reset();

            Initializer.InitializeAll(ModHelper);





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


            //ModEntry.Instance.Monitor.Log($"[{DateTime.Now:HH:mm:ss}] RAN BUTTON PRESS", LogLevel.Alert);



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



    }
}












