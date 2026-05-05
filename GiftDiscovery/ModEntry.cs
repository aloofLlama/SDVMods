using GiftDiscovery.Compatibility;
using GiftDiscovery.Config;
using GiftDiscovery.Helpers;
using GiftDiscovery.Models;
using GiftDiscovery.Services;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SDVCommon;
using SDVCommon.Helpers;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Framework.ModLoading;
using StardewValley;
using StardewValley.Menus;
using System.Runtime.CompilerServices;


namespace GiftDiscovery
{
    public class ModEntry : Mod
    {
        public static ModEntry Instance { get; private set; } = null!;
        public static IModHelper ModHelper { get; private set; } = null!;
        public static IMonitor ModMonitor { get; private set; } = null!;
        public static ModConfig ModConfig { get; internal set; } = null!;

        private bool _showTooltip = false;
        //public static bool IsHudVisible { get; private set; }
        //public static bool IsActiveMenuVisible { get; private set; }
        public static int ToggleVersion = 0; //used for cache update



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

        private void OnRenderedActiveMenu(object? sender, RenderedActiveMenuEventArgs e)
        {
            //Show tooltip when toggled on, holding a giftable item that has loves and/or likes
            //
            if (!Context.IsWorldReady
                || !_showTooltip)
                return;

            //must be an object
            if (HoveredItem.GetFromAnyMenu() is not StardewValley.Object obj)
                return;

            TooltipBuilder.DrawTooltip(e.SpriteBatch, obj);

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

            if (Game1.player.CurrentItem is not StardewValley.Object obj)
                return;

            TooltipBuilder.DrawTooltip(e.SpriteBatch, obj);
        }



        private void OnButtonPressed(object? sender, StardewModdingAPI.Events.ButtonPressedEventArgs e)
        {
            if (e.Button == ModConfig.ToggleTooltipKey)
            {
                _showTooltip = !_showTooltip;
                ToggleVersion++; //used to refresh tooltip
            }


            // Reinitialize for debug
            if (e.Button == SButton.F5)
            {
                LogAllNpcClassifications();
                GiftKnowledgeService.InitializeGlobal(ModHelper);
                Initializer.InitializeAll(ModHelper);
            }
        }

        public static void LogAllNpcClassifications()
        {
            // Build classifications using YOUR classify function
            var list = GiftHelper.GetAllGiftableNPCs()
                .Select(npc => NpcGiftClassificationBuilder.Classify(npc))
                .OrderBy(c => c.Name, StringComparer.OrdinalIgnoreCase)
                .ToList();

            // Log each one
            foreach (var c in list)
            {
                var npc = c.Npc;

                string loc = npc?.currentLocation?.Name ?? "null";
                string tile = npc != null ? npc.Tile.ToString() : "null";

                int hearts = 0;
                if (npc != null && Game1.player.friendshipData.TryGetValue(c.Name, out var f))
                    hearts = f.Points;

                ModEntry.Instance.Monitor.Log(
                    $"[CLASSIFY] {c.Name} | Giftable={c.IsGiftable}, Available={c.IsAvailable}, " +
                    $"Met={c.IsMet}, CanGiftToday={c.CanGiftToday}, MaxHeart={c.IsMaxHeart}, " +
                    $"Hearts={hearts}, Loc={loc}, Tile={tile}",
                    LogLevel.Info
                );
            }
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

            //IsHudVisible = hud;
            //IsActiveMenuVisible = menu;
        }


    }




}





