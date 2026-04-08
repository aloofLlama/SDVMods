using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using PlantingDay.Compatibility;
using PlantingDay.Helpers;
using PlantingDay.Models;
using StardewModdingAPI;
using StardewValley;
using StardewValley.GameData.HomeRenovations;
using StardewValley.Menus;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace PlantingDay
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



            // Use the correct key format O:#### to see if the item is in the plant library
            string lookupKey = obj.ItemId;

            var plant = PlantDatabase.LookupFromKey(lookupKey);

            if (plant is null)
                return;
            ModEntry.Instance.Monitor.Log($"Harvest ID for {plant.Seed.Name} = {plant.Harvest?.Id}", LogLevel.Info);


            var elements = TooltipBuilder.BuildTooltip(plant);

            DrawTooltip(spriteBatch, elements);

            //foreach (var p in plant.PurchaseOptions)
            //{
            //    ModEntry.Instance.Monitor.Log(
            //        $"Item: {plant.Id} Vendor: {p.VendorId} Price: {p.GoldPrice}",
            //        LogLevel.Info
            //    );
            //}
            //ModEntry.Instance.Monitor.Log(
            //    $"Plant {plant.Id} has {plant.PurchaseOptions.Count} purchase options.",
            //    LogLevel.Info
                //);

        }

        //--------------
        // Get the item to draw the tooltip about
        //--------------

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

                    //Item? hovered = invPage?.hoveredItem;
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

        //--------------
        // Creating the display
        //--------------


        private static void DrawTooltip(SpriteBatch b, List<TooltipElement> elements)
        {
            SpriteFont font = Game1.smallFont;

            const int IconRenderSize = 32;   // draw size (old system)
            const int IconColumnWidth = 32;   // spacing you liked

            int width = 0;
            int height = 0;
            int maxIconColumnWidth = 0;

            //
            // PASS 1 — Measure height + icon column width
            //
            foreach (var el in elements)
            {
                int lineHeight = font.LineSpacing;

                if (el.IconTexture != null || el.IconRef.HasValue)
                {
                    int iconSize = IconRenderSize;
                    lineHeight = Math.Max(lineHeight, iconSize + 2);
                    maxIconColumnWidth = Math.Max(maxIconColumnWidth, IconColumnWidth);
                }

                height += el.PaddingTop + lineHeight + el.PaddingBottom;
            }

            //
            // PASS 2 — Measure width
            //
            foreach (var el in elements)
            {
                int lineWidth = 0;

                // Only add icon column width for NON-inline rows
                if (el.InlineSegments == null && (el.IconTexture != null || el.IconRef.HasValue))
                    lineWidth += maxIconColumnWidth;

                // Inline rows
                if (el.InlineSegments != null)
                {
                    foreach (var seg in el.InlineSegments)
                    {
                        if (seg.IconRef.HasValue)
                        {
                            lineWidth += font.LineSpacing;
                        }
                        if (!string.IsNullOrEmpty(seg.Text))
                        {
                            lineWidth += (int)font.MeasureString(seg.Text).X;
                        }
                    }
                }
                else if (!string.IsNullOrEmpty(el.Text))
                {
                    lineWidth += (int)font.MeasureString(el.Text).X;
                }

                width = Math.Max(width, lineWidth);
            }

            width += 32;
            height += 32;

            //
            // Position tooltip
            //
            int x = Game1.getMouseX() - width + 32;
            int y = Game1.getMouseY() + 32;

            if (x < 0)
                x = 0;

            if (y + height > Game1.uiViewport.Height)
                y = Game1.uiViewport.Height - height;

            //
            // Draw background
            //
            IClickableMenu.drawTextureBox(b, x, y, width, height, Color.White);

            //
            // PASS 3 — Draw content
            //
            int drawY = y + 16;

            foreach (var el in elements)
            {
                drawY += el.PaddingTop;
                int drawX = x + 16;

                int lineHeight = font.LineSpacing;

                if (el.IconTexture != null || el.IconRef.HasValue)
                {
                    int iconSize = IconRenderSize;
                    lineHeight = Math.Max(lineHeight, iconSize + 2);
                }

                //
                // Separator
                //
                if (el.IsSeparator)
                {
                    int lineY = drawY + font.LineSpacing / 2;

                    b.Draw(
                        Game1.staminaRect,
                        new Rectangle(drawX, lineY, width - 32, 2),
                        Color.White * 0.35f
                    );

                    drawY += font.LineSpacing + el.PaddingBottom;
                    continue;
                }

                //
                // Draw icon
                //
                if (el.IconTexture != null)
                {
                    int iconSize = IconRenderSize;

                    int yOffset = drawY + (lineHeight - iconSize) / 2;
                    int xOffset = drawX + (IconColumnWidth - iconSize) / 2;

                    b.Draw(
                        el.IconTexture,
                        new Rectangle(xOffset, yOffset, iconSize, iconSize),
                        Color.White
                    );

                    drawX += IconColumnWidth;
                }
                else if (el.IconRef.HasValue)
                {
                    var icon = el.IconRef.Value;

                    int usedWidth = DrawPaddedIcon(
                        b,
                        icon.Texture,
                        icon.Source,
                        icon.Scale,
                        IconColumnWidth,
                        drawX,
                        drawY,
                        lineHeight
                    );

                    drawX += IconColumnWidth;
                    //var icon = el.IconRef.Value;

                    //// Compute scaled size
                    //float scale = icon.Scale;
                    //int iconSize = (int)(icon.Source.Width * scale);

                    //// Center inside the icon column
                    //int yOffset = drawY + (lineHeight - iconSize) / 2;
                    //int xOffset = drawX + (IconColumnWidth - iconSize) / 2;

                    //// Draw using scale (Stardew‑style)
                    //b.Draw(
                    //    icon.Texture,
                    //    new Vector2(xOffset, yOffset),
                    //    icon.Source,
                    //    Color.White,
                    //    0f,
                    //    Vector2.Zero,
                    //    scale,
                    //    SpriteEffects.None,
                    //    1f
                    //);

                    //drawX += IconColumnWidth;
                }

                //
                // Inline segments
                //
                if (el.InlineSegments != null)
                {
                    int xCursor = drawX;

                    foreach (var seg in el.InlineSegments)
                    {
                        if (seg.IconRef.HasValue)
                        {
                            var icon = seg.IconRef.Value;

                            DrawPaddedIcon(
                                b,
                                icon.Texture,
                                icon.Source,
                                icon.Scale,
                                lineHeight, 
                                xCursor,
                                drawY,
                                lineHeight
                            );

                            xCursor += lineHeight;
                        }
                        if (!string.IsNullOrEmpty(seg.Text))
                        {
                            if (seg.Bold)
                            {
                                b.DrawString(font, seg.Text, new Vector2(xCursor + 1, drawY), seg.Color);
                                b.DrawString(font, seg.Text, new Vector2(xCursor, drawY), seg.Color);
                            }
                            else
                            {
                                b.DrawString(font, seg.Text, new Vector2(xCursor, drawY), seg.Color);
                            }

                            xCursor += (int)font.MeasureString(seg.Text).X;
                        }
                    }

                    drawY += lineHeight + el.PaddingBottom;
                    continue;
                }

                //
                // Normal text
                //
                if (!string.IsNullOrEmpty(el.Text))
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

                drawY += lineHeight + el.PaddingBottom;
            }
        }




        //-----------------
        // Building support
        //-----------------


        // Put multiple segments on the same line with separators (e.g., seasons)
        public static List<InlineSegment> BuildInlineSegments<T>(
            IEnumerable<T> items,
            Func<T, IEnumerable<InlineSegment>> buildSegments,
            string separator = " • ")
        {
            var result = new List<InlineSegment>();
            var list = items.ToList();

            for (int i = 0; i < list.Count; i++)
            {
                // Build ONCE
                var segs = buildSegments(list[i]).ToList();

                // Skip empty
                if (segs.Count == 0)
                    continue;

                // Add segments
                result.AddRange(segs);

                // Add separator if not last
                if (i < list.Count - 1)
                {
                    result.Add(new InlineSegment
                    {
                        Text = separator,
                        Color = TooltipColors.Muted,
                        Bold = false
                    });
                }
            }

            return result;
        }
        private static int DrawPaddedIcon(
             SpriteBatch b,
             Texture2D texture,
             Rectangle source,
             float scale,
             int boxSize,
             int x,
             int y,
             int lineHeight
             )
        {
            int drawW = (int)(source.Width * scale);
            int drawH = (int)(source.Height * scale);

            // Center inside the box
            int xOffset = x + (boxSize - drawW) / 2;
            int yOffset = y + (lineHeight - drawH) / 2;

            b.Draw(texture, new Rectangle(xOffset, yOffset, drawW, drawH), source, Color.White);

            return drawW;
        }

        //private static int DrawPaddedIcon(
        //    SpriteBatch b,
        //    Texture2D texture,
        //    Rectangle source,
        //    float scale,
        //    int boxSize,
        //    int x,
        //    int y,
        //    int lineHeight
        //)
        //{
        //    // Scale based on source size, not box size
        //    int drawW = (int)(source.Width * scale);
        //    int drawH = (int)(source.Height * scale);

        //    // If the icon is larger than the box, clamp it
        //    float max = Math.Max(drawW, drawH);
        //    if (max > boxSize)
        //    {
        //        float shrink = boxSize / max;
        //        drawW = (int)(drawW * shrink);
        //        drawH = (int)(drawH * shrink);
        //    }

        //    // Center inside the box
        //    int xOffset = x + (boxSize - drawW) / 2;
        //    int yOffset = y + (lineHeight - drawH) / 2;

        //    b.Draw(texture, new Rectangle(xOffset, yOffset, drawW, drawH), source, Color.White);

        //    return drawW;
        //}

    }
}



