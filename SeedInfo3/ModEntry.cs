using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using System.Collections.Generic;
using System.Linq;
//using SeedInfo.Data;
using SeedInfo.Models;

namespace SeedInfo
{
    public class ModEntry : Mod
    {
        //private Texture2D? fruitTreeTexture;

        public override void Entry(IModHelper helper)
        {

            helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
            helper.Events.Display.RenderedActiveMenu += OnRenderedActiveMenu;

            //fruitTreeTexture = Helper.GameContent.Load<Texture2D>("TileSheets/fruitTrees");

            helper.Events.Input.ButtonPressed += this.OnButtonPressed;




        }

        private void OnSaveLoaded(object? sender, SaveLoadedEventArgs e)
        {
            this.Monitor.Log("OnSaveLoaded reached");
            CropDatabase.Load(this.Helper);
            this.Monitor.Log("After CropDatabase.Load");

            this.Monitor.Log($"Crop count: {CropDatabase.Crops.Count}");

            foreach (var key in CropDatabase.Crops.Keys)
                this.Monitor.Log($"Loaded crop: {key}");

            //foreach (var key in cropsData.Keys)
            //        //{
            //        //    this.Monitor.Log($" - {key}", LogLevel.Info);
            //        //}


        }

        private void OnRenderedActiveMenu(object? sender, RenderedActiveMenuEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            Item? hovered = GetHoveredItemFromAnyMenu();
            if (hovered is not StardewValley.Object obj)
                return;

            DrawCropTooltipIfApplicable(e.SpriteBatch, obj);
        }

        private void DrawCropTooltipIfApplicable(SpriteBatch b, StardewValley.Object obj)
        {
            this.Monitor.Log("ENTRY REACHED — IF YOU SEE THIS, YOU ARE EDITING THE RIGHT FILE");

            if (obj.Category != StardewValley.Object.SeedsCategory)
                return;

            string cropKey = obj.ParentSheetIndex.ToString();

            if (!CropDatabase.Crops.TryGetValue(cropKey, out var crop))
                return;

            var elements = BuildCropTooltip(cropKey, crop);
            DrawTooltipLeftOfMouse(b, elements);

            foreach (var key in CropDatabase.Crops.Keys)
                Monitor.Log($"Loaded crop: {key}");



        }



        //using Microsoft.Xna.Framework;
        //using Microsoft.Xna.Framework.Graphics;
        //using Microsoft.Xna.Framework.Input;
        //using StardewModdingAPI;
        //using StardewModdingAPI.Events;
        //using StardewValley;
        //using StardewValley.GameData.Crops;
        //using StardewValley.GameData.Objects;
        //using StardewValley.Menus;
        //using System;
        //using System.Xml.Linq;
        //using static SeedInfo.TooltipElement;
        //using SeedInfo.Data;



        //namespace SeedInfo
        //{
        //    public class ModEntry : Mod
        //    {
        //        private Dictionary<string, CropDataModel>? cropsData;
        //        private Texture2D? fruitTreeTexture;




        //        public override void Entry(IModHelper helper)
        //        {
        //            helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
        //            helper.Events.Display.RenderedActiveMenu += OnRenderedActiveMenu;
        //            fruitTreeTexture = Helper.GameContent.Load<Texture2D>("TileSheets/fruitTrees");

        //        }

        //        private void OnRenderedActiveMenu(object? sender, RenderedActiveMenuEventArgs e)

        //        {

        //            // Only run when the world is ready and crop data is loaded
        //            if (!Context.IsWorldReady || cropsData == null)
        //                return;

        //            // Run when hovering over an item in backpacks, chests, shops, etc.
        //            Item? hovered = GetHoveredItemFromAnyMenu();
        //            if (hovered is not StardewValley.Object obj)
        //                return;

        //            DrawCropTooltipIfApplicable(e.SpriteBatch, obj);

        //        }

        //        private void DrawCropTooltipIfApplicable(SpriteBatch b, StardewValley.Object obj)
        //        {
        //            if (cropsData == null)
        //                return;

        //            // Only run for seeds
        //            if (obj.Category != StardewValley.Object.SeedsCategory)
        //                return;

        //            // Look up the crop data using the parent sheet index of the seed item
        //            string cropKey = obj.ParentSheetIndex.ToString();

        //            if (!cropsData.TryGetValue(cropKey, out var crop))
        //                return;

        //            // Get the seasons the crop grows in
        //            //string seasonList = string.Join(", ", crop.Seasons);
        //            //this.Monitor.Log($"crop grows in: {seasonList}", LogLevel.Info);

        //            var elements = BuildCropTooltip(crop);
        //            DrawTooltipLeftOfMouse(b, elements);
        //        }

        private static readonly Color SpringColor = new(80, 200, 120); //green
        private static readonly Color SummerColor = new(208, 0, 255); //purple
        private static readonly Color FallColor = new(170, 70, 0); //orange
        private static readonly Color WinterColor = new(100, 160, 255); //blue

        // Set the season color and bold if it matches the season
        private (Color color, bool bold) GetSeasonStyle(string season)
        {
            bool isCurrent = season.Equals(Game1.currentSeason, StringComparison.OrdinalIgnoreCase);

            Color color = season switch
            {
                "Spring" => SpringColor,
                "Summer" => SummerColor,
                "Fall" => FallColor,
                "Winter" => WinterColor,
                _ => Color.Black
            };

            // If it's not the current season, force black + not bold
            if (!isCurrent)
                return (Color.Black, false);

            // If it IS the current season, return the season color + bold
            return (color, true);
        }

        private List<TooltipElement> BuildCropTooltip(string itemId, CropDataModel crop)
        {
            var list = new List<TooltipElement>();

            // seasons from unified database
            var seasons = CropDatabase.GetSeasons(itemId);
            if (seasons.Count > 0)
            {
                list.Add(new TooltipElement
                {
                    Seasons = seasons,
                    PaddingBottom = 4
                });
            }

            // Trellis
            if (crop.Trellis)
            {
                list.Add(new TooltipElement
                {
                    Text = "Requires a trellis",
                    TextColor = Color.Orange
                });
            }

            // Paddy
            if (crop.Paddy)
            {
                list.Add(new TooltipElement
                {
                    Text = "Grows faster near water",
                    TextColor = Color.CornflowerBlue
                });
            }

            return list;
        }


        // Where to draw the tooltip
        private void DrawTooltipLeftOfMouse(SpriteBatch b, List<TooltipElement> elements)
        {
            SpriteFont font = Game1.smallFont;

            // Measure total height + max width
            int width = 0;
            int height = 0;

            foreach (var el in elements)
            {
                int lineHeight;

                if (el.Seasons != null && el.Seasons.Count > 0)
                {
                    // All seasons are on ONE line → measure the tallest season text
                    lineHeight = (int)font.MeasureString("Summer").Y;
                }
                else
                {
                    lineHeight = (int)font.MeasureString(el.Text ?? "").Y;
                }

                height += el.PaddingTop + lineHeight + el.PaddingBottom;

                int lineWidth = 0;

                if (el.Seasons != null && el.Seasons.Count > 0)
                {
                    // Sum widths of all seasons + spaces
                    foreach (var season in el.Seasons)
                        lineWidth += (int)font.MeasureString(season + " ").X;
                }
                else
                {
                    lineWidth = (int)font.MeasureString(el.Text ?? "").X;
                }

                if (el.Icon != null)
                    lineWidth += el.IconSize + 4;

                width = Math.Max(width, lineWidth);
            }

            width += 32;  // vanilla padding
            height += 32;

            // Position to the left of the mouse
            int x = Game1.getMouseX() - width + 32;
            int y = Game1.getMouseY() + 32;

            if (x < 0)
                x = 0;

            if (y + height > Game1.uiViewport.Height)
                y = Game1.uiViewport.Height - height;

            // Draw background
            IClickableMenu.drawTextureBox(
                b,
                x,
                y,
                width,
                height,
                Color.White
            );

            // Draw content
            int drawY = y + 16;

            foreach (var el in elements)
            {
                drawY += el.PaddingTop;

                int drawX = x + 16;

                // Icon
                if (el.Icon != null)
                {
                    b.Draw(el.Icon, new Rectangle(drawX, drawY, el.IconSize, el.IconSize), el.IconSource, Color.White);
                    drawX += el.IconSize + 4;
                }

                // Text OR Seasons
                if (el.Seasons != null && el.Seasons.Count > 0)
                {
                    int seasonX = drawX;

                    foreach (var season in el.Seasons)
                    {
                        (Color color, bool bold) = GetSeasonStyle(season);

                        // bold simulation
                        if (bold)
                        {
                            b.DrawString(font, season, new Vector2(seasonX + 1, drawY), color);
                            b.DrawString(font, season, new Vector2(seasonX, drawY), color);
                        }
                        else
                        {
                            b.DrawString(font, season, new Vector2(seasonX, drawY), color);
                        }

                        // advance X by width of this season + a space
                        seasonX += (int)font.MeasureString(season + " ").X;
                    }
                }
                else if (!string.IsNullOrEmpty(el.Text))
                {
                    if (el.Bold)
                    {
                        b.DrawString(font, el.Text, new Vector2(drawX + 1, drawY), el.TextColor);
                        b.DrawString(font, el.Text, new Vector2(drawX, drawY), el.TextColor);
                    }
                    else
                    {
                        b.DrawString(font, el.Text, new Vector2(drawX, drawY), el.TextColor);
                    }
                }

                if (el.Seasons != null && el.Seasons.Count > 0)
                {
                    // All seasons are on one line → use normal line height
                    drawY += (int)font.MeasureString("Summer").Y + el.PaddingBottom;
                }
                else
                {
                    drawY += (int)font.MeasureString(el.Text ?? "").Y + el.PaddingBottom;
                }
            }
        }

        private Item? GetHoveredItemFromAnyMenu()
        {
            IClickableMenu menu = Game1.activeClickableMenu;
            if (menu == null)
                return null;

            switch (menu)
            {
                // Inventory (GameMenu → InventoryPage)
                case GameMenu gm:
                    var invPage = gm.pages
                        .OfType<StardewValley.Menus.InventoryPage>()
                        .FirstOrDefault();

                    return invPage?.hoveredItem;

                // Chests, Fridges, Dressers, Junimo Chests, etc.
                case StardewValley.Menus.ItemGrabMenu chest:
                    return chest.hoveredItem;

                // Pierre’s shop, Traveling Cart, Krobus, Qi, etc.
                case StardewValley.Menus.ShopMenu shop:
                    return shop.hoveredItem as Item;

                default:
                    return null;
            }
        }


        private void OnButtonPressed(object? sender, ButtonPressedEventArgs e)
        {
            // Load a translation string from the i18n folder and show it in a HUD message
            var msg = Helper.Translation.Get("Message.Debug", new { name = Game1.player.Name, button = e.Button });

            // print button presses to the console window
            this.Monitor.Log(msg, LogLevel.Debug);
        }
    }
}

//        public override void OnSaveLoaded(object? sender, SaveLoadedEventArgs e)
//        {
//            CropDatabase.Load(this.Helper);
//        }



//        //private void OnSaveLoaded(object? sender, SaveLoadedEventArgs e)
//        //{
//        //    // Load vanilla crop data
//        //    var rawCropData = Helper.GameContent
//        //        .Load<Dictionary<string, StardewValley.GameData.Crops.CropData>>("Data/Crops");

//        //    // Load fruit tree data
//        //    var rawFruitTrees = Helper.GameContent
//        //        .Load<Dictionary<string, StardewValley.GameData.FruitTrees.FruitTreeData>>("Data/FruitTrees");

//        //    foreach (var (key, tree) in rawFruitTrees)
//        //    {
//        //        cropsData[key] = new CropDataFull
//        //        {
//        //            FruitSeason = tree.FruitSeason
//        //        };
//        //    }

//        //    // Load Custom Bush Data
//        //    var rawBushes = Helper.GameContent
//        //         .Load<Dictionary<string, StardewValley.GameData.Bushes.BushData>>("Data/Bushes");

//        //    foreach (var (key, bush) in rawBushes)
//        //    {
//        //        cropsData[key] = new CropDataFull
//        //        {
//        //            HarvestSeasons = bush.HarvestSeasons?.ToList()
//        //        };
//        //    }

//        //    // Load custom crop data 
//        //    foreach (var (key, crop) in rawCropData)
//        //    {
//        //        cropsData[key] = new CropDataFull
//        //        {
//        //            Seasons = crop.Seasons?.Select(s => s.ToString()).ToList() ?? new(),
//        //            GrowSeasons = crop.GrowSeasons?.Select(s => s.ToString()).ToList(),
//        //            ExtraSeasons = crop.ExtraSeasons?.Select(s => s.ToString()).ToList(),
//        //            Trellis = crop.IsRaised,
//        //            Paddy = crop.IsPaddyCrop
//        //        };
//        //    }

//        //    // Create custom dictionary
//        //    cropsData = new Dictionary<string, CropDataFull>();

//        //    // Convert vanilla → your custom class
//        //    foreach (var (key, crop) in rawCropData)
//        //    {
//        //        cropsData[key] = new CropDataFull
//        //        {
//        //            Seasons = crop.Seasons.Select(s => s.ToString()).ToList(),
//        //            Trellis = crop.IsRaised,
//        //            Paddy = crop.IsPaddyCrop
//        //        };
//        //    }
//        //}

//        //private void OnSaveLoaded(object? sender, SaveLoadedEventArgs e)
//        //{
//        //    cropsData = new Dictionary<string, CropDataModel>();

//        //    // Load vanilla crop data
//        //    var rawCropData = Helper.GameContent
//        //        .Load<Dictionary<string, StardewValley.GameData.Crops.CropData>>("Data/Crops");

//        //    foreach (var (key, crop) in rawCropData)
//        //    {
//        //        cropsData[key] = new CropDataModel
//        //        {
//        //            Seasons = crop.Seasons?.Select(s => s.ToString()).ToList() ?? new(),
//        //            GrowSeasons = crop.GrowSeasons?.Select(s => s.ToString()).ToList(),
//        //            ExtraSeasons = crop.ExtraSeasons?.Select(s => s.ToString()).ToList(),
//        //            Trellis = crop.IsRaised,
//        //            Paddy = crop.IsPaddyCrop
//        //        };
//        //    }

//        //    // Load fruit tree data
//        //    var rawFruitTrees = Helper.GameContent
//        //        .Load<Dictionary<string, StardewValley.GameData.FruitTrees.FruitTreeData>>("Data/FruitTrees");

//        //    foreach (var (key, tree) in rawFruitTrees)
//        //    {
//        //        cropsData[key] = new CropDataModel
//        //        {
//        //            FruitSeason = tree.FruitSeason
//        //        };
//        //    }

//        //    // Load custom bush data
//        //    var rawBushes = Helper.GameContent
//        //         .Load<Dictionary<string, StardewValley.GameData.Bushes.BushData>>("Data/Bushes");

//        //    foreach (var (key, bush) in rawBushes)
//        //    {
//        //        cropsData[key] = new CropDataModel
//        //        {
//        //            HarvestSeasons = bush.HarvestSeasons?.ToList()
//        //        };
//        //    }

//        //    // Load custom crop data 
//        //    var rawCornucopiaCrops = Helper.GameContent
//        //         .Load<Dictionary<string, dynamic>>("Mods/cornucopia.crops/Data/Crops");

//        //    foreach (var (key, crop) in rawCornucopiaCrops)
//        //    {
//        //        if (!cropsData.ContainsKey(key))
//        //            cropsData[key] = new CropDataModel();

//        //        cropsData[key].GrowSeasons = crop.GrowSeasons?.ToObject<List<string>>();
//        //        cropsData[key].ExtraSeasons = crop.ExtraSeasons?.ToObject<List<string>>();
//        //    }

//        //    // Load Raccoon Seeds
//        //    var rawRaccoonCrops = Helper.GameContent
//        //         .Load<Dictionary<string, dynamic>>("Mods/raccoon.crops/Data/Crops");

//        //    foreach (var (key, crop) in rawRaccoonCrops)
//        //    {
//        //        if (!cropsData.ContainsKey(key))
//        //            cropsData[key] = new CropDataModel();

//        //        cropsData[key].GrowSeasons = crop.GrowSeasons?.ToObject<List<string>>();
//        //        cropsData[key].ExtraSeasons = crop.ExtraSeasons?.ToObject<List<string>>();
//        //    }
//        //}

//        ////Get Seasons for all different types of crops (vanilla, modded, fruit trees, bushes) and combine into one list
//        //private List<string> GetAllSeasons(CropDataModel data)
//        //{
//        //    var seasons = new List<string>();

//        //    // Vanilla crops
//        //    if (data.Seasons != null)
//        //        seasons.AddRange(data.Seasons);

//        //    // Modded crops
//        //    if (data.GrowSeasons != null)
//        //        seasons.AddRange(data.GrowSeasons);

//        //    if (data.ExtraSeasons != null)
//        //        seasons.AddRange(data.ExtraSeasons);

//        //    // Fruit trees
//        //    if (!string.IsNullOrEmpty(data.FruitSeason))
//        //        seasons.Add(data.FruitSeason);

//        //    // Custom bushes
//        //    if (data.HarvestSeasons != null)
//        //        seasons.AddRange(data.HarvestSeasons);

//        //    // Cleanup
//        //    return seasons
//        //        .Where(s => !string.IsNullOrWhiteSpace(s))
//        //        .Select(s => s.Trim().ToLowerInvariant())
//        //        .Distinct()
//        //        .Select(s => char.ToUpper(s[0]) + s.Substring(1))
//        //        .ToList();
//        //}

//        //// 🔍 Print all crop keys so we can see what the real key is
//        //this.Monitor.Log("Loaded crop keys:", LogLevel.Info);
//        //foreach (var key in cropsData.Keys)
//        //{
//        //    this.Monitor.Log($" - {key}", LogLevel.Info);
//        //}
//    }


//}


