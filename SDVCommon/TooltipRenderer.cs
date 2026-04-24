using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PlantingDay.Helpers;
using PlantingDay.Models.Runtime;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using SDVCommon.Helpers;


namespace SDVCommon
{

    public static class TooltipRenderer
    {

        public static Item? GetHoveredItemFromAnyMenu()
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


        public static void DrawTooltip(SpriteBatch b, List<TooltipElement> elements)
        {
            SpriteFont font = Game1.smallFont;

            const int IconRenderSize = 32;  
            const int IconColumnWidth = 34;
            const int separatorPadding = 12;
            const int inlineIconWidthPadding = 6;

            int width = 0;
            int height = 0;
            int maxIconColumnWidth = 0;

            //
            // PASS 1 — Measure height + icon column width
            //
            foreach (var el in elements)
            {
                int lineHeight = font.LineSpacing;

                if (el.IsSeparator)
                {
                    height += el.PaddingTop + separatorPadding + el.PaddingBottom; // or whatever height you want
                    continue;
                }

                if (el.IconTexture != null || el.Icon.HasValue)
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
                if (el.InlineSegments == null && (el.IconTexture != null || el.Icon.HasValue))
                    lineWidth += maxIconColumnWidth;

                // Inline rows
                if (el.InlineSegments != null)
                {
                    foreach (var seg in el.InlineSegments)
                    {
                        if (seg.Icon.HasValue)
                        {
                            lineWidth += font.LineSpacing + inlineIconWidthPadding;
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

                width = Math.Max(width, lineWidth) + 2;
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

                if (el.IconTexture != null || el.Icon.HasValue)
                {
                    int iconSize = IconRenderSize;
                    lineHeight = Math.Max(lineHeight, iconSize + 2);
                }

                //
                // Separator
                //
                if (el.IsSeparator)
                {
                    int lineY = drawY + separatorPadding /2;

                    b.Draw(
                        Game1.staminaRect,
                        new Rectangle(drawX, lineY, width - 32, 2),
                        new Color(255, 170, 110)
                    );

                    drawY += separatorPadding;
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
                else if (el.Icon.HasValue)
                {
                    var icon = el.Icon.Value;

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
                }

                //
                // Inline segments
                //
                if (el.InlineSegments != null)
                {
                    int xCursor = drawX;

                    foreach (var seg in el.InlineSegments)
                    {
                        if (seg.Icon.HasValue)
                        {
                            var icon = seg.Icon.Value;

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

                            xCursor += lineHeight + inlineIconWidthPadding;
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
                var segs = buildSegments(list[i]).ToList();

                if (segs.Count == 0)
                    continue;

                result.AddRange(segs);

                // Look ahead: does the next item produce segments?
                for (int j = i + 1; j < list.Count; j++)
                {
                    var nextSegs = buildSegments(list[j]);
                    if (nextSegs != null && nextSegs.Any())
                    {
                        result.Add(new InlineSegment
                        {
                            Text = separator,
                            Color = TooltipColors.Muted,
                            Bold = false
                        });
                        break;
                    }
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



