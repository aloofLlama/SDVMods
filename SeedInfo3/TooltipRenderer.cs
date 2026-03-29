using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SeedInfo;
using SeedInfo.Compatibility;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SeedInfo
{
    public static class TooltipRenderer

    {
        public static void DrawMenu(SpriteBatch spriteBatch)
        {
            if (!Context.IsWorldReady)
                return;

            Item? hovered = GetHoveredItemFromAnyMenu();
            if (hovered is not StardewValley.Object obj)
                return;


            // Qualified ID of the hovered item
            string qid = obj.QualifiedItemId;

            // 1. Check if this is a custom bush sapling
            if (PlantDatabase.TryGetCornucopiaBushInfo(obj, out var bushInfo))
            {
                ModEntry.Instance.Monitor.Log("NOT HERE", LogLevel.Info);
                DrawTooltip(spriteBatch, bushInfo);
                return;
            }



            // 2. Otherwise, treat it as a normal seed/fruit tree/etc.
            string lookupKey = "O:" + obj.ItemId;

            var info = PlantDatabase.FromKey(lookupKey);
            if (info is null)
                return;

            DrawTooltip(spriteBatch, info);
        }

        //public static void DrawHud(SpriteBatch spriteBatch)
        //{
        //    if (!Context.IsWorldReady)
        //        return;
        //    //ModEntry.Instance.Monitor.Log("HERE1", LogLevel.Info);
        //    Item? hovered = GetHudHoveredItem();
        //    if (hovered is not StardewValley.Object obj)
        //        return;

        //    ModEntry.Instance.Monitor.Log("HERE2", LogLevel.Info);


        //    var info = PlantDatabase.FromItem(obj);
        //    if (info is null)
        //        return;

        //    DrawTooltip(spriteBatch, info);
        //}

        //private static Item? GetHudHoveredItem()
        //{
        //    // 1. Toolbar hover (most common)
        //    if (Game1.player.CurrentItem != null)
        //    return Game1.player.CurrentItem;

        //    // 2. On-screen menus (buffs, etc.)
        //    foreach (var menu in Game1.onScreenMenus)
        //    {
        //        switch (menu)
        //        {
        //            case ItemGrabMenu grab when grab.hoveredItem is Item item1:
        //                //ModEntry.Instance.Monitor.Log("HERE5", LogLevel.Info);

        //                return item1;

        //            case InventoryPage inv when inv.hoveredItem is Item item2:
        //                ModEntry.Instance.Monitor.Log("HERE6", LogLevel.Info);

        //                return item2;
        //        }
        //    }

        //    return null;
        //}

        private static void DrawTooltip(SpriteBatch spriteBatch, PlantInfo info)
        {
            var elements = BuildTooltip(info);
            PlaceTooltip(spriteBatch, elements);
        }

        private static List<TooltipElement> BuildTooltip(PlantInfo info)
        {
            var list = new List<TooltipElement>();

            if (info.Seasons.Count > 0)
            {
                var normalized = info.Seasons
                    .Select(s => Capitalize(s))
                    .ToList();

                list.Add(new TooltipElement { Seasons = normalized });
            }

            if (info.Trellis)
                list.Add(new TooltipElement { Text = "Requires a trellis", TextColor = Color.Orange });

            if (info.Paddy)
                list.Add(new TooltipElement { Text = "Grows faster near water", TextColor = Color.CornflowerBlue });

            return list;
        }
        private static string Capitalize(string s)
        {
            if (string.IsNullOrEmpty(s))
                return s;

            return char.ToUpper(s[0]) + s.Substring(1);
        }

        // Where to draw the tooltip
        private static void PlaceTooltip(SpriteBatch b, List<TooltipElement> elements)
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










        private static Item? GetHoveredItemFromAnyMenu()
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

                    Item? hovered = invPage?.hoveredItem;
                    //if (hovered is not null)
                    //{
                    //    this.Monitor.Log($"Hover item QID: {hovered.QualifiedItemId}", LogLevel.Info);

                    //}

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
    

            
      

        private static readonly Color SpringColor = new(80, 200, 120); //green
        private static readonly Color SummerColor = new(208, 0, 255); //purple
        private static readonly Color FallColor = new(170, 70, 0); //orange
        private static readonly Color WinterColor = new(100, 160, 255); //blue

        // Set the season color and bold if it matches the season
        private static (Color color, bool bold) GetSeasonStyle(string season)
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

        

        

        

    }
}
