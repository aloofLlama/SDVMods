using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SDVCommon.Helpers;
using SDVCommon.Icons;
using SDVCommon.Models.Tooltip;
using SDVCommon.Services;
using StardewValley;
using StardewValley.Menus;


namespace SDVCommon.Rendering
{

    public static class TooltipRenderer
    {

        public static void DrawLeftOfCursor(SpriteBatch b, List<TooltipElement> elements)
        {
            TooltipStyle style = TooltipStyle.Default;

            var (width, height) = GetTooltipSize(elements, style);
            var (x, y) = PositionLeftOfCursor(width, height);
            DrawTooltipAt(b, elements, style, x, y, width, height);
        }

        public static void DrawLeftandAboveCursor(SpriteBatch b, List<TooltipElement> elements)
        {
            TooltipStyle style = TooltipStyle.Default;

            var (width, height) = GetTooltipSize(elements, style);
            var (x, y) = PositionLeftAboveCursor(width, height);

            DrawTooltipAt(b, elements, style, x, y, width, height);
        }


        public static void DrawBottomLeft(SpriteBatch b, List<TooltipElement> elements)
        {
            TooltipStyle style = TooltipStyle.Default;

            var (width, height) = GetTooltipSize(elements, style);
            var (x, y) = PositionBottomLeft(height);
            DrawTooltipAt(b, elements, style, x, y, width, height);
        }

        public static void DrawBottomRight(SpriteBatch b, List<TooltipElement> elements)
        {
            TooltipStyle style = TooltipStyle.Default;

            var (width, height) = GetTooltipSize(elements, style);
            var (x, y) = PositionBottomRight(width, height);
            DrawTooltipAt(b, elements, style, x, y, width, height);
        }


        private static (int width, int height) MeasureTooltip(
            List<TooltipElement> elements,
            TooltipStyle style)
        {
            int width = 0;
            int height = 0;
            //int IconColumnWidth = style.IconColumnWidth;

            //SDVCommonLog.Log($"[{DateTime.Now:HH:mm:ss}] NEW MEASURE**************************************************************************************",
              //              LogHelper.DebugAlert);

            int cnt = 0;

            // Measure Height
            foreach (var el in elements)
            {
                cnt++;
                //SDVCommonLog.Log($"       Element {cnt}", LogHelper.DebugDebug);

                if (el.IsSeparator)
                {
                    height += style.SeparatorRowHeight;
                    //SDVCommonLog.Log($"       Sep Height {style.SeparatorRowHeight}", LogHelper.DebugDebug);
                    //SDVCommonLog.Log($"{height} - HEIGHT", LogHelper.DebugInfo);
                    continue;
                }

                int rowHeight = TextRowHeight(style);



                int cntInline = 1; //start at one since hitting the line break increments
                if (el.InlineSegments != null)
                {
                    rowHeight = InlineRowHeight(el.InlineSegments, style);

                    foreach (var seg in el.InlineSegments)
                    {
                        if (seg.IsLineBreak)
                        {
                            cntInline++;
                            continue;
                        }
                    }

                    height += rowHeight * cntInline;
                    //SDVCommonLog.Log($"{height} - HEIGHT | Inline row count  {cntInline}", LogHelper.DebugInfo);

                    continue;
                }
                else
                {
                    if (el.Icon.HasValue)
                    {
                        var icon = el.Icon.Value;
                        int iconRowHeight = IconRowHeight(icon, style);
                        rowHeight = Math.Max(rowHeight, iconRowHeight);
                    }

                    height += rowHeight;
                    //SDVCommonLog.Log($"       Non inline row {rowHeight}", LogHelper.DebugDebug);

                }

                //SDVCommonLog.Log($"{height} - HEIGHT", LogHelper.DebugInfo);

            }

            // Measure Width
            foreach (var el in elements)
            {
                int lineWidth = 0;

                if (el.Icon.HasValue)
                {
                    int columnWidth = IconColumnWidth(el.Icon.Value, style);
                    lineWidth += columnWidth;
                }

                if (!string.IsNullOrEmpty(el.Text))
                {
                    lineWidth += TextWidth(el.Text, style);
                }


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
                        {
                            int columnWidth = IconColumnWidth(seg.Icon.Value, style);
                            currentLineWidth += columnWidth;
                        }

                        if (!string.IsNullOrEmpty(seg.Text))
                            currentLineWidth += TextWidth(seg.Text, style);
                    }

                    maxLineWidth = Math.Max(maxLineWidth, currentLineWidth);
                    lineWidth += maxLineWidth;
                }

                //set width according to the widest line
                width = Math.Max(width, lineWidth);

            }

            width += style.BorderPadding * 2;
            height += style.BorderPadding * 2;

            return (width, height);
        }

        private static void DrawTooltipAt(
            SpriteBatch b,
            List<TooltipElement> elements,
            TooltipStyle style,
            int x,
            int y,
            int width,
            int height)
        {
            //SDVCommonLog.Log($"NEW DRAW", LogHelper.DebugAlert);

            // Draw background
            IClickableMenu.drawTextureBox(b, x, y, width, height, Color.White);

            //SDVCommonLog.Log($"{0} | START", LogHelper.DebugInfo);

            // Draw content
            int drawY = y + style.BorderPadding;
            //SDVCommonLog.Log($"{drawY - y} | Border padding", LogHelper.DebugInfo);

            foreach (var el in elements)
            {
                //SDVCommonLog.Log($"{drawY - y} | Start New Element", LogHelper.DebugInfo);

                int drawX = x + style.BorderPadding;

                // Separator
                if (el.IsSeparator)
                {
                    int lineY = drawY + style.SeparatorRowHeight / 2; //draw in the middle vertically
                    int separatorLength = width - style.BorderPadding * 2; // width of the textbox reduced by the padding on both sides

                    b.Draw(
                        Game1.staminaRect,
                        new Rectangle(drawX, lineY, separatorLength, style.SeparatorThickness),
                        new Color(255, 170, 110)
                    );

                    drawY += style.SeparatorRowHeight;
                    //SDVCommonLog.Log($"{drawY - y} | Separator End | dif {style.SeparatorRowHeight}", LogHelper.DebugInfo);

                    continue;
                }

                int rowHeight = TextRowHeight(style);

                // Icons
                if (el.Icon.HasValue)
                {
                    var icon = el.Icon.Value;
                    int iconRowHeight = IconRowHeight(icon, style);
                    rowHeight = Math.Max(rowHeight, iconRowHeight);

                    //SDVCommonLog.Log($"       Basic row height: {rowHeight}", LogHelper.DebugDebug);

                    int usedWidth = DrawPaddedIcon(
                        b,
                        icon,
                        style,
                        rowHeight,
                        drawX,
                        drawY
                    );

                    drawX += usedWidth;
                }


                // Inline segments
                if (el.InlineSegments != null)
                {
                    //SDVCommonLog.Log($"{drawY - y} | Inline Segment Row Start", LogHelper.DebugInfo);
                    int xCursor = drawX;

                    rowHeight = InlineRowHeight(el.InlineSegments, style);

                    //SDVCommonLog.Log($"       Inline row height: {rowHeight}", LogHelper.DebugDebug);

                    int tmpcnt = 1;
                    foreach (var seg in el.InlineSegments)
                    {
                        if (seg.IsLineBreak)
                        {
                            tmpcnt++;
                            // Move to next line with top and bottom padding
                            drawY += rowHeight;

                            // Reset X cursor to start of text column
                            xCursor = drawX;

                            continue;
                        }

                        if (seg.Icon.HasValue)
                        {
                            var icon = seg.Icon.Value;

                            int usedWidth = DrawPaddedIcon(
                                b,
                                icon,
                                style,
                                rowHeight,
                                xCursor,
                                drawY
                            );

                            xCursor += usedWidth;

                        }

                        if (!string.IsNullOrEmpty(seg.Text))
                        {

                            int usedWidth = DrawPaddedText(
                                seg.Text,
                                seg.Bold,
                                seg.Underline,
                                seg.TextColor,
                                seg.Font,
                                b,
                                style,
                                rowHeight,
                                xCursor,
                                drawY
                                );

                            xCursor += usedWidth;
                        }
                    }

                    //drawY += rowHeight;
                    //SDVCommonLog.Log($"{drawY - y} | Inline Segment End | dif {tmpcnt * rowHeight}", LogHelper.DebugInfo);

                }

                //Basic row

                    //SDVCommonLog.Log($"       Basic text row height: {rowHeight}", LogHelper.DebugDebug);


                    // Normal text
                    if (!string.IsNullOrEmpty(el.Text))
                    {
                        //SDVCommonLog.Log($"{drawY - y} | Normal Text Row Start |", LogHelper.DebugInfo);

                        DrawPaddedText(
                            el.Text,
                            el.Bold,
                            el.Underline,
                            el.TextColor,
                            el.Font,
                            b,
                            style,
                            rowHeight,
                            drawX,
                            drawY
                            );

                        //drawY += rowHeight;
                        //SDVCommonLog.Log($"{drawY - y} | Normal Text Row End | dif {rowHeight}", LogHelper.DebugInfo);


                }
                drawY += rowHeight;

            }
        }


        private static int DrawPaddedIcon(
             SpriteBatch b,
             Icon icon,
             TooltipStyle style,
             int rowHeight,
             int x,
             int y
             )
        {

            Texture2D texture = icon.Texture;
            Rectangle source = icon.Source;
            float scale = icon.Scale;
            Color tint = icon.Tint;

            //SDVCommonLog.Log($"                                      {scale} | IconScale |", LogHelper.DebugAlert);


            int drawW = (int)(source.Width * scale);
            int drawH = (int)(source.Height * scale);


            int columnWidth = IconColumnWidth(icon, style);

            // Center inside the box
            int xOffset = x + (columnWidth - drawW) / 2;
            int yOffset = y + (rowHeight - drawH) / 2;

            b.Draw(texture, new Rectangle(xOffset, yOffset, drawW, drawH), source, tint);

            return columnWidth;
        }

        private static int DrawPaddedText(
            string text,
            bool bold,
            bool underline,
            Color color,
            SpriteFont font,
            SpriteBatch b,
            TooltipStyle style,
            int rowHeight,
             int x,
             int y
             )
        {
            if (string.IsNullOrEmpty(text))
                return 0;

            float textHeight = font.MeasureString(text).Y;

            int verticalOffset = (int)((rowHeight - textHeight) / 2f);
            int textY = y + verticalOffset;

            if (bold)
                DrawBoldString(b, font, text, new Vector2(x, textY), color);
            else
                b.DrawString(font, text, new Vector2(x, textY), color);

            if (underline)
            {
                DrawUnderline(
                    b,
                    text,
                    x,
                    textY,
                    color,
                    style
                );
            }

            return (int)font.MeasureString(text).X;
        }

        private static void DrawBoldString(
            SpriteBatch b,
            SpriteFont font,
            string text,
            Vector2 position,
            Color color)
        {
            // Shadow/offset pass
            b.DrawString(font, text, new Vector2(position.X + 1, position.Y), color);

            // Main pass
            b.DrawString(font, text, position, color);
        }

        private static void DrawUnderline(
            SpriteBatch b,
            string text,
            int x,
            int y,
            Color color,
            TooltipStyle style)
        {
            // Measure the text width/height
            var size = style.Font.MeasureString(text);

            int thickness = style.UnderlineThickness;

            // Vertical placement of underline
            float underlineY = y + size.Y - style.UnderlineOffset;

            b.Draw(
                Game1.staminaRect,
                new Rectangle(
                    x,
                    (int)underlineY,
                    (int)size.X,
                    thickness
                ),
                color
            );
        }

        //-----------------
        // Height and Width Helpers
        //-----------------

        private static int TextRowHeight(TooltipStyle style)
        {
            return style.Font.LineSpacing + style.LineHeightPadding * 2;
        }

        private static int TextWidth(string text, TooltipStyle style)
        {
            return (int)style.Font.MeasureString(text).X;
        }

        private static int IconRowHeight(Icon icon, TooltipStyle style)
        {
            int iconRenderHeight = (int)(icon.Source.Height * icon.Scale);
            return iconRenderHeight + style.LineHeightPadding * 2;

        }

        private static int InlineRowHeight(List<InlineSegment> segments, TooltipStyle style)
        {
            //first check if any segment is an icon to get the correct row height
            //assumes all wrapped rows are the same height
            int rowHeight = TextRowHeight(style);

            foreach (var seg in segments)
            {
                if (seg.Icon.HasValue)
                {
                    var icon = seg.Icon.Value;
                    int iconRowHeight = IconRowHeight(icon, style);
                    rowHeight = Math.Max(rowHeight, iconRowHeight);
                }

            }

            return rowHeight;
        }

        private static int IconColumnWidth(Icon icon, TooltipStyle style)
        {
            int iconRenderWidth = (int)(icon.Source.Width * icon.Scale);
            int iconColumnWidth = iconRenderWidth + style.IconWidthPadding * 2;
            return iconColumnWidth;

        }





        //-----------------
        // Draw positions
        //-----------------

        private static (int x, int y) PositionLeftOfCursor(int width, int height)
        {
            int x, y;

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

        private static (int x, int y) PositionLeftAboveCursor(int width, int height)
        {
            int x, y;

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

            y = Game1.getMouseY() - height;

            if (x < 0)
                x = 0;

            if (y < 0)
                y = 0;

            return (x, y);
        }


        private static (int x, int y) PositionBottomLeft(int height)
        {
            int x = 32;
            int y = Game1.uiViewport.Height - height - 32;
            return (x, y);
        }

        private static (int x, int y) PositionBottomRight(int width, int height)
        {
            int x = Game1.uiViewport.Width - width - 32;
            int y = Game1.uiViewport.Height - height - 32;
            return (x, y);
        }

        //----------
        //Measurement Caching
        //----------
        private static readonly Dictionary<List<TooltipElement>, (int w, int h)> _sizeCache
            = new(ReferenceEqualityComparer<List<TooltipElement>>.Instance);

        private static (int w, int h) GetTooltipSize(List<TooltipElement> elements, TooltipStyle style)
        {
            if (!_sizeCache.TryGetValue(elements, out var size))
            {
                size = MeasureTooltip(elements, style);
                _sizeCache[elements] = size;
            }

            return size;
        }

        public static void InvalidateSize(List<TooltipElement> elements)
        {
            _sizeCache.Remove(elements);
        }

    }
}



