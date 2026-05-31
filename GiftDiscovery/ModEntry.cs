using GiftDiscovery.Compatibility;
using GiftDiscovery.Config;
using GiftDiscovery.GameData;
using GiftDiscovery.Helpers;
using GiftDiscovery.Services;
using GiftDiscovery.Tooltip;
using HarmonyLib;
using SDVCommon.GameData;
using SDVCommon.Helpers;
using SDVCommon.Models.Builders;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;


namespace GiftDiscovery
{
    public class ModEntry : Mod
    {
        public static ModEntry Instance { get; private set; } = null!;
        public static IModHelper ModHelper { get; private set; } = null!;
        public static IMonitor ModMonitor { get; private set; } = null!;
        public static ModConfig ModConfig { get; internal set; } = null!;

        private bool _showTooltip = false;
        internal static bool IsInMenuTooltip = false;


        public override void Entry(IModHelper helper)
        {
            Instance = this;
            ModHelper = helper;
            ModEntry.ModMonitor = base.Monitor;

            SDVCommonLog.Initialize(this.Monitor);
            ModConfig = helper.ReadConfig<ModConfig>();

            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
            helper.Events.Display.RenderedActiveMenu += OnRenderedActiveMenu;
            helper.Events.Display.RenderedHud += OnRenderedHud;
            helper.Events.Input.ButtonPressed += OnButtonPressed;
            helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;



            GiftKnowledgeService.InitializeGlobal(helper);

            // Harmony patch for gift detection
            var harmony = new Harmony(ModManifest.UniqueID);
            harmony.Patch(
                original: AccessTools.Method(typeof(NPC), nameof(NPC.receiveGift)),
                postfix: new HarmonyMethod(typeof(GiftPatch), nameof(GiftPatch.Postfix))
            );

        }

        private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
        {
            GMCMIntegration.Register(ModHelper, ModManifest);
        }

        private void OnSaveLoaded(object? sender, SaveLoadedEventArgs e)
        {

            Initializer.InitializeAll(ModHelper);
        }

        //private void OnRenderedActiveMenu(object? sender, RenderedActiveMenuEventArgs e)
        //{
        //    //Show tooltip when toggled on, holding a giftable item that has loves and/or likes
        //    //
        //    if (!Context.IsWorldReady
        //        || !_showTooltip)
        //        return;

        //    //must be an object
        //    if (HoveredItem.GetFromAnyMenu() is not StardewValley.Object obj)
        //        return;

        //    GiftTooltipBuilder.DrawTooltip(e.SpriteBatch, obj);

        //}
        private void OnRenderedActiveMenu(object? sender, RenderedActiveMenuEventArgs e)
        {
            if (!Context.IsWorldReady || !_showTooltip)
                return;

            var hover = HoveredItem.Get();
            if (!hover.HasValue)
                return;

            ModEntry.IsInMenuTooltip = true;
            bool drewNPCMenuTooltip = false;

            // Apply config based on hover.Source
            switch (hover.Source)
            {
                case HoverSource.CollectionsPage:
                    if (!ModEntry.ModConfig.ShowInCollectionsMenu)
                        return;
                    break;

                case HoverSource.CookingPage:
                    if (!ModEntry.ModConfig.ShowInCoookingMenu)
                        return;
                    break;

                case HoverSource.CraftingPage:
                    if (!ModEntry.ModConfig.ShowInCraftingMenu)
                        return;
                    break;
            }

            switch (hover)
            {
                // Social Menu
                case { NPC: not null }:
                    NPCGiftTooltipBuilder.DrawTooltip(e.SpriteBatch, hover.NPC!);
                    drewNPCMenuTooltip = true;
                    break;

                case { Item: StardewValley.Object obj }:
                    GiftTooltipBuilder.DrawTooltip(e.SpriteBatch, obj);
                    break;
            }

            // NPC proximity tooltip (only if not already showing a tooltip for an NPC in the social menu)
            if (drewNPCMenuTooltip == false)
            {
                NPC? nearest = GiftableNPC.GetClosestNearbyNPC(ModEntry.ModConfig.NearbyRangeTilesNPCTooltip);
                if (nearest != null)
                {
                    NPCGiftTooltipBuilder.DrawTooltip(e.SpriteBatch, nearest);
                }
            }

        }

        private void OnRenderedHud(object? sender, RenderedHudEventArgs e)
        {
            //Show tooltip when toggled on, holding a giftable item that has loves and/or likes
            //
            if (!Context.IsWorldReady
                || !_showTooltip
                || Game1.eventUp)   //hide during cutscenes, festivals, heart events, movies
                return;

            if (Game1.activeClickableMenu != null)
                return;

            ModEntry.IsInMenuTooltip = false;

            // NPC proximity tooltip
            NPC? nearest = GiftableNPC.GetClosestNearbyNPC(ModEntry.ModConfig.NearbyRangeTilesNPCTooltip);
            if (nearest != null)
            {
                NPCGiftTooltipBuilder.DrawTooltip(e.SpriteBatch, nearest);
            }

            // Gift item tooltip (only if holding a giftable item)
            if (Game1.player.CurrentItem is StardewValley.Object obj)
            {
                GiftTooltipBuilder.DrawTooltip(e.SpriteBatch, obj);
            }
        }



        private void OnButtonPressed(object? sender, StardewModdingAPI.Events.ButtonPressedEventArgs e)
        {
            if (e.Button == ModConfig.ToggleTooltipKey)
            {
                _showTooltip = !_showTooltip;
                ModEntry.IncrementToggleVersion();
            }


            // Reinitialize for debug
#if DEBUG
            if (e.Button == SButton.F5)
            {
                Initializer.ResetAll();
                GiftKnowledgeService.InitializeGlobal(ModHelper);
                Initializer.InitializeAll(ModHelper);
                ModEntry.Instance.Monitor.Log($"[{DateTime.Now:HH:mm:ss}]", LogLevel.Warn);
            }

#endif
        }

        public static bool MenuStateChanged { get; private set; }
        private static bool _lastHudVisible;
        private static bool _lastMenuVisible;

        private void OnUpdateTicked(object? sender, UpdateTickedEventArgs e)
        {
            bool hud = Game1.displayHUD;
            bool menu = Game1.activeClickableMenu != null;

            MenuStateChanged = hud != _lastHudVisible || menu != _lastMenuVisible;

            _lastHudVisible = hud;
            _lastMenuVisible = menu;
        }


        private static int _toggleVersion = 0;
        public static int ToggleVersion => _toggleVersion;
        public static void IncrementToggleVersion()
        {
            _toggleVersion++;
        }



    }
}





