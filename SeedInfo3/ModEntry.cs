using SeedInfo.Compatibility;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace SeedInfo
{
    public class ModEntry : Mod
    {
        public static ModEntry Instance { get; private set; } = null!;
        public static ICustomBushApi? CustomBushApi { get; private set; }

        public override void Entry(IModHelper helper)
        {
            Instance = this;

            // Load API + database once the save is ready
            helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;

            // Tooltip events
            helper.Events.Display.RenderedActiveMenu += OnRenderedActiveMenu;
            //helper.Events.Display.RenderedHud += OnRenderedHud;

            helper.Events.Input.ButtonPressed += OnButtonPressed;
        }


        private void OnButtonPressed(object? sender, StardewModdingAPI.Events.ButtonPressedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            Item item = Game1.player.CurrentItem;
            if (item == null)
                return;

            Monitor.Log($"Held item type: {item.GetType().FullName}", LogLevel.Warn);

            if (item is not StardewValley.Object obj)
            {
                Monitor.Log("Item is not a StardewValley.Object — cannot inspect modData or ObjectData.", LogLevel.Warn);
                return;
            }

            string saplingId = obj.QualifiedItemId;
            Monitor.Log($"=== Testing sapling: {saplingId} ===", LogLevel.Warn);

            // -------------------------
            // TEST 1: Load Custom Bush Data
            // -------------------------
            Monitor.Log("=== Test 1: Loading furyx639.CustomBush/Data ===", LogLevel.Warn);

            Dictionary<string, object>? bushDb = null;

            try
            {
                bushDb = Helper.GameContent.Load<Dictionary<string, object>>("furyx639.CustomBush/Data");
                Monitor.Log($"Loaded bush database with {bushDb.Count} entries.", LogLevel.Warn);
            }
            catch (Exception ex)
            {
                Monitor.Log($"Failed to load bush data: {ex}", LogLevel.Error);
                return;
            }

            // -------------------------
            // TEST 2: Does this sapling map to a bush?
            // -------------------------
            Monitor.Log("=== Test 2: Checking sapling → bush mapping ===", LogLevel.Warn);

            if (bushDb.TryGetValue(saplingId, out var rawEntry))
            {
                Monitor.Log($"Bush entry FOUND for {saplingId}.", LogLevel.Warn);

                // Dump the raw JSON object (it will appear as a JsonElement or Dictionary)
                Monitor.Log($"Raw bush entry: {rawEntry}", LogLevel.Warn);
            }
            else
            {
                Monitor.Log($"No bush entry found for {saplingId}.", LogLevel.Warn);
            }

            // -------------------------
            // TEST 3: Check ObjectData.CustomFields
            // -------------------------
            Monitor.Log("=== Test 3: ObjectData.CustomFields ===", LogLevel.Warn);

            if (Game1.objectData.TryGetValue(obj.ItemId, out var od))
            {
                if (od.CustomFields == null || od.CustomFields.Count == 0)
                {
                    Monitor.Log("No CustomFields found.", LogLevel.Warn);
                }
                else
                {
                    foreach (var kvp in od.CustomFields)
                        Monitor.Log($"{kvp.Key} = {kvp.Value}", LogLevel.Warn);
                }
            }
            else
            {
                Monitor.Log("ObjectData entry not found.", LogLevel.Warn);
            }
        }















        private void OnSaveLoaded(object? sender, SaveLoadedEventArgs e)
        {
            // Load Custom Bush API
            //CustomBushApi = Helper.ModRegistry.GetApi<ICustomBushApi>("spacechase0.CustomBush");

           // if (CustomBushApi == null)
             //   Monitor.Log("Custom Bush API not found — bush tooltips disabled.", LogLevel.Warn);

            // Initialize plant database
            PlantDatabase.Initialize();

            Monitor.Log("Plant database initialized.", LogLevel.Info);
        }

        private void OnRenderedActiveMenu(object? sender, RenderedActiveMenuEventArgs e)
        {
            TooltipRenderer.DrawMenu(e.SpriteBatch);

        }

        //private void OnRenderedHud(object? sender, RenderedHudEventArgs e)
        //{

        //    TooltipRenderer.DrawHud(e.SpriteBatch);
        //}
    }
    //    //ModEntry.Instance.Monitor.Log("HERE", LogLevel.Info);

}











