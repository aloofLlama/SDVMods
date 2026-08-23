using GiftDiscovery.Compatibility;
using GiftDiscovery.Config;
using GiftDiscovery.GameData;
using GiftDiscovery.Helpers;
using GiftDiscovery.Tooltip;
using HarmonyLib;
using SDVCommon;
using SDVCommon.GameData;
using SDVCommon.Services;
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

            SDVCommonServices.Initialize(helper, Monitor);
            ModConfig = helper.ReadConfig<ModConfig>();

            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
            helper.Events.Display.RenderedActiveMenu += OnRenderedActiveMenu;
            helper.Events.Display.RenderedHud += OnRenderedHud;
            helper.Events.Input.ButtonPressed += OnButtonPressed;
            helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;


            // Harmony patch for gift detection of modded items
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
                    if (GiftableNPCList.IsGiftableNPC(hover.NPC))
                    {
                        NPCGiftTooltipBuilder.DrawTooltip(e.SpriteBatch, hover.NPC!);
                        drewNPCMenuTooltip = true;
                    }
                    break;

                case { Item: StardewValley.Object obj }:
                    if (GiftableObjectList.IsGiftableObject(obj.QualifiedItemId))
                        GiftTooltipBuilder.DrawTooltip(e.SpriteBatch, obj);
                    break;
            }

            // NPC proximity tooltip (only if not already showing a tooltip for an NPC in the social menu)
            if (drewNPCMenuTooltip == false)
            {

                NPC? nearest = NPCLocation.GetClosestNearbyNPC(ModEntry.ModConfig.NearbyRangeTilesNPCTooltip);
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

            // NPC proximity tooltip (only for giftable NPCs)
            NPC? nearest = NPCLocation.GetClosestNearbyNPC(ModEntry.ModConfig.NearbyRangeTilesNPCTooltip);
            if (nearest != null && GiftableNPCList.IsGiftableNPC(nearest))
            {
                NPCGiftTooltipBuilder.DrawTooltip(e.SpriteBatch, nearest);

            }

            // Gift item tooltip (only if holding a giftable item)
            if (Game1.player.CurrentItem is StardewValley.Object obj &&
                GiftableObjectList.IsGiftableObject(obj.QualifiedItemId))
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

#if DEBUG
            // Debug outputs

            if (e.Button == SButton.F6)
            {
                string timer = "Button Press F6";  // Logs {name} took {ms} ms
                SDVCommonServices.PerfBegin(timer);
                SDVCommonLog.TimestampLog($"{timer} start", LogHelper.AlertOrTrace);

                // Put debug content here
                // *

                GameObject.DumpObjectInfo("(O)74");

                // *

                SDVCommonServices.PerfEnd(timer, 0, LogHelper.DebugOrTrace);
            }

            // Reinitialize for debug
            if (e.Button == SButton.F5)
            {
                bool GDReinit = false;

                if (GDReinit == true)
                {
                    string timer = "Reinitialize";  // Logs {name} took {ms} ms
                    SDVCommonServices.PerfBegin(timer);
                    SDVCommonLog.TimestampLog($"{timer} start", LogHelper.AlertOrTrace);

                    Initializer.ResetAll();
                    Initializer.InitializeAll(ModHelper);

                    SDVCommonServices.PerfEnd(timer, 0, LogHelper.AlertOrTrace);
                }
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





