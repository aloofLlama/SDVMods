using HarvestHelper.Compatibility;
using HarvestHelper.Helpers;
using HarvestHelper.Services;
using SDVCommon;
using SDVCommon.Helpers;
using SDVCommon.GameData;
using SDVCommon.OBSGift;
using SDVCommon.Models.Builders;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using HarmonyLib;



namespace HarvestHelper
{
    public class ModEntry : Mod
    {
        public static ModEntry Instance { get; private set; } = null!;
        public static IModHelper ModHelper { get; private set; } = null!;
        public static IMonitor ModMonitor { get; private set; } = null!;

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
            Initializer.InitializeAll();
        }



        [EventPriority(EventPriority.Low - 1)]
        private void OnRenderedActiveMenu(object? sender, RenderedActiveMenuEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            Item? hovered = HoveredItem.GetFromAnyMenu();

            //must be an object
            if (hovered is not StardewValley.Object obj)
                return;

            //skip recipes
            if (obj.IsRecipe)
                return;


            string lookupKey = obj.ItemId;

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

            var elements = TooltipBuilder.BuildTooltip(harvest, obj);

            TooltipRenderer.DrawLeftOfCursor(e.SpriteBatch, elements);

        }

        private void OnButtonPressed(object? sender, StardewModdingAPI.Events.ButtonPressedEventArgs e)
        {
#if DEBUG
            // Only run when the player presses F5
            if (e.Button != SButton.F5)
                return;
            HarvestInfoBuilder.Reset();
            Initializer.InitializeAll();
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




