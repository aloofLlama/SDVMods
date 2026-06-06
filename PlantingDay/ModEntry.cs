using PlantingDay.Helpers;
using PlantingDay.Services;
using SDVCommon.GameData;
using SDVCommon.Helpers;
using SDVCommon.Models.Builders;
using SDVCommon.Models.Tooltip;
using SDVCommon.Rendering;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;


namespace PlantingDay
{
    public class ModEntry : Mod
    {
        public static ModEntry Instance { get; private set; } = null!;
        public static IModHelper ModHelper { get; private set; } = null!;
        public static IMonitor ModMonitor { get; private set; } = null!;

        private StardewValley.Object? _cachedObj;
        private List<TooltipElement>? _cachedTooltip;


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

       
        }

        [EventPriority(EventPriority.Low-1)]
        private void OnRenderedActiveMenu(object? sender, RenderedActiveMenuEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            var hover = HoveredItem.Get();
            if (hover.Item is not StardewValley.Object obj)
                return;

            // Only rebuild when hovered item changes
            if (!ReferenceEquals(_cachedObj, obj))
            {
                _cachedObj = obj;

                string ItemId = obj.ItemId;

                var plant = PlantInfoBuilder.LookupFromKey(ItemId);

                if (plant is null)
                {
                    _cachedTooltip = null;
                    return;
                }

                _cachedTooltip = TooltipBuilder.BuildTooltip(plant);
            }

                //Temp move above cursor to work with both HH and PD same time on seed items that are both
                //TooltipRenderer.DrawLeftOfCursor(e.SpriteBatch, elements);
                if (_cachedTooltip != null)
                    TooltipRenderer.DrawLeftandAboveCursor(e.SpriteBatch, _cachedTooltip);

            }






        private void OnButtonPressed(object? sender, StardewModdingAPI.Events.ButtonPressedEventArgs e)
        {
            // Only run when the player presses F5
#if DEBUG
            if (e.Button != SButton.F5)
                return;
            PlantInfoBuilder.Reset();
            HarvestInfoBuilder.Reset();

            Initializer.InitializeAll(ModHelper);
            ModEntry.Instance.Monitor.Log($"[{DateTime.Now:HH:mm:ss}]", LogLevel.Warn);

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

#endif


        }



    }
}












