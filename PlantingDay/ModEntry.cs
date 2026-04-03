using PlantingDay.Compatibility;
using PlantingDay.Helpers;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System.Runtime.CompilerServices;


namespace PlantingDay
{
    public class ModEntry : Mod
    {
        public static ModEntry Instance { get; private set; } = null!;
        public static ICustomBushApi? CustomBushApi { get; private set; }
        public static IModHelper ModHelper { get; private set; } = null!;
        public static IMonitor ModMonitor { get; private set; } = null!;

        public override void Entry(IModHelper helper)
        {
            Instance = this;
            ModHelper = helper;
            ModEntry.ModMonitor = base.Monitor;

            //TooltipIcons.Load();
            TooltipIcons.Initialize();

            //helper.Events.GameLoop.GameLaunched += OnGameLaunched;

            helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;

            helper.Events.Display.RenderedActiveMenu += OnRenderedActiveMenu;

            //helper.Events.Display.RenderedHud += OnRenderedHud;
            //helper.Events.Input.ButtonPressed += OnButtonPressed;
            //Helper.Events.Content.AssetRequested += OnAssetRequested;
        }


        private void OnGameLaunched(object? sender, StardewModdingAPI.Events.GameLaunchedEventArgs e)
        {
            /* PAUSE BUSH
            // Load the Custom Bush API
            CustomBushApi = Helper.ModRegistry.GetApi<ICustomBushApi>("furyx639.CustomBush");

            if (CustomBushApi == null)
            {
                Monitor.Log("Custom Bush API not found.", LogLevel.Warn);
                return;
            }

            Monitor.Log("Custom Bush API loaded!", LogLevel.Warn);
            */

        }

        private void OnSaveLoaded(object? sender, SaveLoadedEventArgs e)
        {
            PlantDatabase.Initialize();

            foreach (var plant in PlantDatabase.AllPlants)
                plant.InitializeIcons();



            //Monitor.Log("Plant database initialized.", LogLevel.Info);

            // Load Custom Bush API
            //CustomBushApi = Helper.ModRegistry.GetApi<ICustomBushApi>("spacechase0.CustomBush");

            // if (CustomBushApi == null)
            //   Monitor.Log("Custom Bush API not found — bush tooltips disabled.", LogLevel.Warn);

        }

        private void OnRenderedActiveMenu(object? sender, RenderedActiveMenuEventArgs e)
        {
            
            TooltipRenderer.DrawMenu(e.SpriteBatch);

        }


        /*DEBUG STUFF NOTHING BELOW HERE
                private void OnButtonPressed(object? sender, StardewModdingAPI.Events.ButtonPressedEventArgs e)
                {
                    ModEntry.Instance.Monitor.Log("RUN BUTTON PRESS", LogLevel.Info);




                    if (ModEntry.Instance == null)
                    {
                        Monitor.Log("[Seed Info] ERROR: ModEntry.Instance is NULL!", LogLevel.Error);
                        return;
                    }


                    if (!Context.IsWorldReady)
                        return;

                    Item item = Game1.player.CurrentItem;
                    if (item == null)
                        return;

                    Monitor.Log($"[Seed Info] Held item type: {item.GetType().FullName}", LogLevel.Warn);

                    if (item is not StardewValley.Object obj)
                    {
                        Monitor.Log("[Seed Info] Item is not a StardewValley.Object — skipping bush test.", LogLevel.Warn);
                        return;
                    }

                    Monitor.Log($"[Seed Info] Running TryGetCornucopiaBushInfo on {obj.QualifiedItemId}", LogLevel.Warn);

                    // Call your method
                    if (PlantDatabase.TryGetCornucopiaBushInfo(obj, out PlantInfo? info))
                    {
                        Monitor.Log("[Seed Info] TryGetCornucopiaBushInfo returned TRUE.", LogLevel.Warn);

                        if (info == null)
                        {
                            Monitor.Log("[Seed Info] But PlantInfo is NULL (unexpected).", LogLevel.Warn);
                        }
                        else
                        {
                            Monitor.Log($"[Seed Info] PlantInfo created:", LogLevel.Warn);
                            //Monitor.Log($"  Name: {info.Name}", LogLevel.Warn);
                            //Monitor.Log($"  Type: {info.Type}", LogLevel.Warn);
                            //Monitor.Log($"  Seasons: {string.Join(", ", info.Seasons ?? new())}", LogLevel.Warn);
                            //Monitor.Log($"  AgeToProduce: {info.AgeToProduce}", LogLevel.Warn);
                            //Monitor.Log($"  Texture: {info.TexturePath}", LogLevel.Warn);
                            //Monitor.Log($"  SpriteRow: {info.TextureSpriteRow}", LogLevel.Warn);
                        }
                    }
                    else
                    {
                        Monitor.Log("[Seed Info] TryGetCornucopiaBushInfo returned FALSE.", LogLevel.Warn);
                    }
                }

                // -------------------------
                // TEST 1: Load Custom Bush Data
                // -------------------------
                //Monitor.Log("=== Test 1: Loading furyx639.CustomBush/Data ===", LogLevel.Warn);

                //Dictionary<string, object>? bushDb = null;

                //try
                //{
                //    bushDb = Helper.GameContent.Load<Dictionary<string, object>>("furyx639.CustomBush/Data");
                //    Monitor.Log($"Loaded bush database with {bushDb.Count} entries.", LogLevel.Warn);
                //}
                //catch (Exception ex)
                //{
                //    Monitor.Log($"Failed to load bush data: {ex}", LogLevel.Error);
                //    return;
                //}

                // -------------------------
                // TEST 2: Does this sapling map to a bush?
                // -------------------------
                //Monitor.Log("=== Test 2: Checking sapling → bush mapping ===", LogLevel.Warn);

                //    if (bushDb.TryGetValue(saplingId, out var rawEntry))
                //    {
                //        Monitor.Log($"Bush entry FOUND for {saplingId}.", LogLevel.Warn);

                //        // Dump the raw JSON object (it will appear as a JsonElement or Dictionary)
                //        Monitor.Log($"Raw bush entry: {rawEntry}", LogLevel.Warn);
                //    }
                //    else
                //    {
                //        Monitor.Log($"No bush entry found for {saplingId}.", LogLevel.Warn);
                //    }

                //    // -------------------------
                //    // TEST 3: Check ObjectData.CustomFields
                //    // -------------------------
                //    Monitor.Log("=== Test 3: ObjectData.CustomFields ===", LogLevel.Warn);

                //    if (Game1.objectData.TryGetValue(obj.ItemId, out var od))
                //    {
                //        if (od.CustomFields == null || od.CustomFields.Count == 0)
                //        {
                //            Monitor.Log("No CustomFields found.", LogLevel.Warn);
                //        }
                //        else
                //        {
                //            foreach (var kvp in od.CustomFields)
                //                Monitor.Log($"{kvp.Key} = {kvp.Value}", LogLevel.Warn);
                //        }
                //    }
                //    else
                //    {
                //        Monitor.Log("ObjectData entry not found.", LogLevel.Warn);
                //    }
                //}
        */





        //private void OnRenderedHud(object? sender, RenderedHudEventArgs e)
        //{

        //    TooltipRenderer.DrawHud(e.SpriteBatch);
        //}
    }
    //    //ModEntry.Instance.Monitor.Log("HERE", LogLevel.Info);

}











