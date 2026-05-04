using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SDVCommon.Helpers;
using SDVCommon.Tooltip;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;


namespace SDVCommon
{

    public static class TooltipRenderer
    {

        public static void DrawLeftOfCursor(SpriteBatch b, List<TooltipElement> elements)
        {
            SpriteFont font = Game1.smallFont;
            TooltipStyle style = TooltipStyle.Default;

            var (width, height) = MeasureTooltip(elements, font, style);
            var (x, y) = PositionLeftOfCursor(width, height);
            DrawTooltipAt(b, elements, font, style, x, y, width, height);
        }


        public static void DrawBottomLeft(SpriteBatch b, List<TooltipElement> elements)
        {
            SpriteFont font = Game1.smallFont;
            TooltipStyle style = TooltipStyle.Default;

            var (width, height) = MeasureTooltip(elements, font, style);
            var (x, y) = PositionBottomLeft(width, height);
            DrawTooltipAt(b, elements, font, style, x, y, width, height);
        }

        public struct TooltipStyle
        {
            public int IconRenderSize;
            public int IconColumnWidth;
            public int SeparatorPadding;
            public int InlineIconWidthPadding;

            public static TooltipStyle Default => new TooltipStyle
            {
                IconRenderSize = 32,
                IconColumnWidth = 34,
                SeparatorPadding = 12,
                InlineIconWidthPadding = 6
            };
        }



        private static (int width, int height) MeasureTooltip(
            List<TooltipElement> elements,
            SpriteFont font,
            TooltipStyle style)
        { 
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
                    height += el.PaddingTop + style.SeparatorPadding + el.PaddingBottom;
                    continue;
                }

                if (el.IconTexture != null || el.Icon.HasValue)
                {
                    int iconSize = style.IconRenderSize;
                    lineHeight = Math.Max(lineHeight, iconSize + 2);
                    maxIconColumnWidth = Math.Max(maxIconColumnWidth, style.IconColumnWidth);
                }

                int lineCount = 1;

                if (el.InlineSegments != null)
                {
                    lineCount = 1 + el.InlineSegments.Count(s => s.IsLineBreak);
                }

                height += el.PaddingTop + lineHeight * lineCount + el.PaddingBottom;
            }

            // PASS 2 — Measure width
            foreach (var el in elements)
            {
                int lineWidth = 0;

                // Always account for icon column if there's an icon
                if (el.IconTexture != null || el.Icon.HasValue)
                    lineWidth += maxIconColumnWidth;

                if (el.InlineSegments != null)
                {
                    int currentLineWidth = 0;
                    int maxLineWidth = 0;

                    foreach (var seg in el.InlineSegments)
                    {
                        if (seg.IsLineBreak)
                        {
                            maxLineWidth = Math.Max(maxLineWidth, currentLineWidth);
                            currentLineWidth = 0;
                            continue;
                        }

                        if (seg.Icon.HasValue)
                            currentLineWidth += font.LineSpacing + style.InlineIconWidthPadding;

                        if (!string.IsNullOrEmpty(seg.Text))
                            currentLineWidth += (int)font.MeasureString(seg.Text).X;
                    }

                    maxLineWidth = Math.Max(maxLineWidth, currentLineWidth);
                    lineWidth += maxLineWidth;
                }
                else if (!string.IsNullOrEmpty(el.Text))
                {
                    lineWidth += (int)font.MeasureString(el.Text).X;
                }

                width = Math.Max(width, lineWidth) + el.PaddingRight;
            }

            width += 32;
            height += 32;

            return (width, height);
        }

        private static (int x, int y) PositionLeftOfCursor(int width, int height)
        {
            int x = Game1.getMouseX() - width - 32;
            int y = Game1.getMouseY() + 32;

            bool isShippingMenu = Game1.activeClickableMenu is ShippingMenu;

            if (isShippingMenu)
            {
                // Shipping menu cursor is shifted right → move tooltip left more
                x = Game1.getMouseX() - width - 64;
            }
            else
            {
                x = Game1.getMouseX() - width - 32;
            }

            y = Game1.getMouseY() + 32;

            if (x < 0)
                x = 0;

            if (y + height > Game1.uiViewport.Height)
                y = Game1.uiViewport.Height - height;

            return (x, y);
        }

        private static (int x, int y) PositionBottomLeft(int width, int height)
        {
            int x = 32;
            int y = Game1.uiViewport.Height - height - 32;
            return (x, y);
        }

        private static void DrawTooltipAt(
            SpriteBatch b,
            List<TooltipElement> elements,
            SpriteFont font,
            TooltipStyle style,
            int x,
            int y,
            int width,
            int height)
        {

            // Draw background
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
                    int iconSize = style.IconRenderSize;
                    lineHeight = Math.Max(lineHeight, iconSize + 2);
                }

                //
                // Separator
                //
                if (el.IsSeparator)
                {
                    int lineY = drawY + style.SeparatorPadding / 2;

                    b.Draw(
                        Game1.staminaRect,
                        new Rectangle(drawX, lineY, width - 32, 2),
                        new Color(255, 170, 110)
                    );

                    drawY += style.SeparatorPadding;
                    continue;
                }

                //
                // Draw icon
                //
                if (el.IconTexture != null)
                {
                    int iconSize = style.IconRenderSize;

                    int yOffset = drawY + (lineHeight - iconSize) / 2;
                    int xOffset = drawX + (style.IconColumnWidth - iconSize) / 2;

                    b.Draw(
                        el.IconTexture,
                        new Rectangle(xOffset, yOffset, iconSize, iconSize),
                        Color.White
                    );

                    drawX += style.IconColumnWidth;
                }
                else if (el.Icon.HasValue)
                {
                    var icon = el.Icon.Value;

                    int usedWidth = DrawPaddedIcon(
                        b,
                        icon.Texture,
                        icon.Source,
                        icon.Scale,
                        style.IconColumnWidth,
                        drawX,
                        drawY,
                        lineHeight
                    );

                    drawX += style.IconColumnWidth;
                }

                //
                // Inline segments
                //
                if (el.InlineSegments != null)
                {
                    int xCursor = drawX;

                    foreach (var seg in el.InlineSegments)
                    {
                        // handle explicit line breaks
                        if (seg.IsLineBreak)
                        {
                            // Move to next line
                            drawY += lineHeight;

                            // Reset X cursor to start of text column
                            xCursor = drawX;

                            continue;
                        }

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

                            xCursor += lineHeight + style.InlineIconWidthPadding;
                        }

                        if (!string.IsNullOrEmpty(seg.Text))
                        {
                            if (seg.Bold)
                            {
                                b.DrawString(font, seg.Text, new Vector2(xCursor + 1, drawY), seg.TextColor);
                                b.DrawString(font, seg.Text, new Vector2(xCursor, drawY), seg.TextColor);
                            }
                            else
                            {
                                b.DrawString(font, seg.Text, new Vector2(xCursor, drawY), seg.TextColor);
                            }
                            var size = font.MeasureString(seg.Text);


                            if (seg.Underline)
                            {

                                int thickness = 2;
                                float underlineY = drawY + size.Y - 6;

                                b.Draw(
                                    Game1.staminaRect,
                                    new Rectangle(
                                        xCursor,              
                                        (int)underlineY,
                                        (int)size.X,
                                        thickness
                                    ),
                                    seg.TextColor
                                );
                            }


                            xCursor += (int)size.X;
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

                    if (el.Underline)
                    {
                        var size = font.MeasureString(el.Text);

                        // underline thickness
                        int thickness = 2;

                        // underline Y position
                        float underlineY = drawY + size.Y - 6;

                        b.Draw(
                            Game1.staminaRect,
                            new Rectangle(
                                drawX,
                                (int)underlineY,
                                (int)size.X,
                                thickness
                            ),
                            el.TextColor
                        );
                    }

                    drawY += lineHeight + el.PaddingBottom;
                }
            }
        }






        //-----------------
        // Building support
        //-----------------



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


    }
}



