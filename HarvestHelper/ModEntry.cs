using HarmonyLib;
using HarvestHelper.Compatibility;
using HarvestHelper.Helpers;
using HarvestHelper.Services;
using SDVCommon.GameData;
using SDVCommon.Helpers;
using SDVCommon.Models.Builders;
using SDVCommon.Models.Tooltip;
using SDVCommon.OBSGift;
using SDVCommon.Rendering;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;



namespace HarvestHelper
{
    public class ModEntry : Mod
    {
        public static ModEntry Instance { get; private set; } = null!;
        public static IModHelper ModHelper { get; private set; } = null!;
        public static IMonitor ModMonitor { get; private set; } = null!;

        private StardewValley.Object? _cachedObj;
        private List<TooltipElement>? _cachedTooltip;


        //Temp for debug gift detection
        private readonly static Dictionary<string, int> _prevGifts = new();

        public override void Entry(IModHelper helper)
        {
            Instance = this;
            ModHelper = helper;
            ModEntry.ModMonitor = base.Monitor;

            SDVCommonLog.Initialize(this.Monitor);

            helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
            helper.Events.Display.RenderedActiveMenu += OnRenderedActiveMenu;

            helper.Events.Input.ButtonPressed += OnButtonPressed;

            //Temp for debug gift detection
            //helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;

            // Initialize shared gift knowledge
            GiftKnowledgeServiceOLD.Initialize(helper);

            // Harmony patch for perfect gift detection
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

            //skip recipes
            if (obj.IsRecipe)
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
            if (e.Button != SButton.F5)
                return;
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

            //foreach (var pair in Game1.player.friendshipData.Pairs)
            //{
            //    string npcName = pair.Key;
            //    Friendship f = pair.Value;

            //    NPC npc = Game1.getCharacterFromName(npcName, mustBeVillager: false);
            //    if (npc == null)
            //    {
            //        ModEntry.Instance.Monitor.Log($"[Friendship] {npcName}: NPC not found.", LogLevel.Warn);
            //        continue;
            //    }

            //    int maxHearts = GiftHelperOLD.GetMaxHearts(npc);
            //    int maxPoints = maxHearts * 250;

            //    int currentPoints = f.Points;
            //    int currentHearts = currentPoints / 250;

            //    ModEntry.Instance.Monitor.Log(
            //        $"[Friendship] {npcName}: {currentHearts}/{maxHearts} hearts ({currentPoints}/{maxPoints} points)",
            //        LogLevel.Info
            //    );
            //}
#endif

        }




    }
}




