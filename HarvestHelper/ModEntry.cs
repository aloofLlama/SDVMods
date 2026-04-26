using HarvestHelper.Helpers;
using HarvestHelper.Services;
using Microsoft.Xna.Framework.Graphics;
using SDVCommon;
using SDVCommon.Helpers;
using SDVCommon.Icons;
using SDVCommon.Models.Wrappers;
using SDVCommon.Services;
using SDVData;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Framework.ModLoading;
using StardewValley;
using SDVCommon.Models.Runtime;


namespace HarvestHelper
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

            helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
            helper.Events.Display.RenderedActiveMenu += OnRenderedActiveMenu;

            helper.Events.Input.ButtonPressed += GiftDetection.OnButtonPressed;
            helper.Events.Input.ButtonReleased += GiftDetection.OnButtonReleased;

            helper.Events.Input.ButtonPressed += OnButtonPressed;

        }


        private void OnSaveLoaded(object? sender, SaveLoadedEventArgs e)
        {
            Initializer.InitializeAll(ModHelper);
        }



        [EventPriority(EventPriority.Low - 1)]
        private void OnRenderedActiveMenu(object? sender, RenderedActiveMenuEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            Item? hovered = HoveredItem.GetFromInventory();
            if (hovered is not StardewValley.Object obj)
                return;


            string lookupKey = obj.ItemId;
            //ModEntry.Instance.Monitor.Log($"CHECK ID {lookupKey}", LogLevel.Info);

            var harvest = HarvestInfoBuilder.LookupFromKey(lookupKey);
            string canonicalId = IdHelper.CanonicalItemId(obj.QualifiedItemId);

            if (!Game1.objectData.TryGetValue(canonicalId, out var data))
            {
                return;
            }

            //var data = Game1.objectData[obj.ItemId];

            if (harvest is null ||
                !HarvestCategories.IsDesiredCategory(data)
                )
                return;

            //ModEntry.Instance.Monitor.Log($"CHECK ID {harvest.Data.HarvestId} price ", LogLevel.Info);


            var elements = TooltipBuilder.BuildTooltip(harvest, obj);

            TooltipRenderer.DrawTooltip(e.SpriteBatch, elements);

        }

        private void OnButtonPressed(object? sender, StardewModdingAPI.Events.ButtonPressedEventArgs e)
        {
            // Only run when the player presses F5
            if (e.Button != SButton.F5)
                return;
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



        }




    }
}




