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


        public override void Entry(IModHelper helper)
        {
            Instance = this;
            ModHelper = helper;
            ModEntry.ModMonitor = base.Monitor;

            SDVCommonLog.Initialize(this.Monitor);
            ModConfig = helper.ReadConfig<ModConfig>();

            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
            //helper.Events.Display.RenderedActiveMenu += OnRenderedActiveMenu;
            helper.Events.Display.RenderedHud += OnRenderedHud;
            helper.Events.Input.ButtonPressed += OnButtonPressed;


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

            if (!obj.canBeGivenAsGift())
                return;

            if (!GiftHelper.GetKnownBy(obj, GiftTaste.Love, TasteSourceMode.All).Any() &&
                !GiftHelper.GetKnownBy(obj, GiftTaste.Like, TasteSourceMode.All).Any())
                return;


            var elements = TooltipBuilder.BuildTooltip(obj);
            if (elements is not { Count: > 0 })
                return;

            TooltipRenderer.DrawBottomLeft(e.SpriteBatch, elements);
        }



        private void OnButtonPressed(object? sender, StardewModdingAPI.Events.ButtonPressedEventArgs e)
        {
            if (e.Button == ModConfig.ToggleTooltipKey)
            {
                _showTooltip = !_showTooltip;
            }


            // Reinitialize for debug
            if (e.Button == SButton.F5)
            {
                GiftKnowledgeService.InitializeGlobal(ModHelper);
                Initializer.InitializeAll(ModHelper);

            }

            var monitor = ModEntry.Instance.Monitor;

            monitor.Log("=== WHO CLASSIFY SAYS I CAN GIFT TODAY ===", LogLevel.Info);

            foreach (var npc in GiftHelper.GetAllGiftableNPCs())
            {
                var c = NpcGiftClassificationBuilder.Classify(npc);

                if (c.CanGiftToday)
                {
                    monitor.Log($"{c.Name} — CanGiftToday", LogLevel.Info);
                }
            }

            monitor.Log("=== END WHO CLASSIFY SAYS I CAN GIFT TODAY ===", LogLevel.Info);
        }


    }




}





