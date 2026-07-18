using HarmonyLib;
using HarvestHelper.Compatibility;
using HarvestHelper.Helpers;
using HarvestHelper.Services;
using SDVCommon;
using SDVCommon.Compatibility;
using SDVCommon.GameData;
using SDVCommon.Helpers;
using SDVCommon.Models.Builders;
using SDVCommon.Models.Tooltip;
using SDVCommon.OBSGift;
using SDVCommon.Rendering;
using SDVCommon.Services;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Objects;
using System.Collections;
using System.Reflection;



namespace HarvestHelper
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

            SDVCommonServices.Initialize(helper, Monitor);

            helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
            helper.Events.Display.RenderedActiveMenu += OnRenderedActiveMenu;

            helper.Events.Input.ButtonPressed += OnButtonPressed;

            // Initialize shared gift knowledge
            GiftKnowledgeServiceOLD.Initialize(helper);

            // Harmony patch for modded gift detection
            var harmony = new Harmony(ModManifest.UniqueID);
            harmony.Patch(
                original: AccessTools.Method(typeof(NPC), nameof(NPC.receiveGift)),
                postfix: new HarmonyMethod(typeof(GiftPatch), nameof(GiftPatch.Postfix))
            );

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

            var hover = HoveredItem.Get();

            //must be an object
            if (hover.Item is not StardewValley.Object obj)
                return;

            //skip recipes and big crafting
            if (obj.IsRecipe ||
                obj.Category == StardewValley.Object.BigCraftableCategory ||
                obj is Furniture ||
                obj is Wallpaper)
                return;

            // Only rebuild when hovered item changes
            if (!ReferenceEquals(_cachedObj, obj))
            {
                _cachedObj = obj;

                string itemId = obj.ItemId;

                var harvest = HarvestInfoBuilder.LookupFromKey(itemId);

                if (harvest is null)
                {
                    _cachedTooltip = null;
                    return;
                }

                if (!Game1.objectData.TryGetValue(itemId, out var data))
                {
                    _cachedTooltip = null;
                    return;
                }

                _cachedTooltip = TooltipBuilder.BuildTooltip(harvest, obj);
            }
            //Temp move above cursor to work with both HH and PD same time on seed items that are both
            //TooltipRenderer.DrawLeftOfCursor(e.SpriteBatch, elements);
            if (_cachedTooltip != null)
                TooltipRenderer.DrawLeftandAboveCursor(e.SpriteBatch, _cachedTooltip);
        }

        private void OnButtonPressed(object? sender, StardewModdingAPI.Events.ButtonPressedEventArgs e)
        {
#if DEBUG
            // Only run when the player presses F5
            if (e.Button == SButton.F5)
            {
                HarvestInfoBuilder.Reset();
                Initializer.InitializeAll(ModHelper);
                ModEntry.Instance.Monitor.Log($"[{DateTime.Now:HH:mm:ss}]", LogLevel.Warn);
            }

            if (e.Button == SButton.F7)
            {
                SDVCommonLog.Log($"[{DateTime.Now:HH:mm:ss}]", LogLevel.Alert);

               //var recipe = new CraftingRecipe("HeyKatu.CulinaryDelight_chocolate_cupcake", isCookingRecipe: true);

               // var info = CookingInfoBuilder.BuildIngredientfs(recipe);

               // foreach (var ing in info)
               // {
               //     SDVCommonLog.Log(
               //         $"ingredient {ing.IngredientId}",
               //         LogHelper.DebugOrTrace
               //     );
               // }

                SDVCommonLog.Log($"DONE", LogLevel.Alert);

            }

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


#endif

        }




    }
}




