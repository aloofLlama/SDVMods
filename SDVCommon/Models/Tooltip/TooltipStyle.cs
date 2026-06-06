
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace SDVCommon.Models.Tooltip
{
    public struct TooltipStyle
    {
        public SpriteFont Font;
        public int LineHeightPadding; // space between content (text or icon) and line (added to both top and bottom)
        public int IconWidthPadding; // space around icon width (added to both left and right)
        public int BorderPadding; // space between content and border

        public int SeparatorThickness; //thickness of the drawn line
        public int SeparatorRowHeight; // total height of the separator element (including padding)

        public int UnderlineThickness; //thickness of the drawn line
        public int UnderlineOffset; // how far below the text it sits

        public TooltipStyle(
            SpriteFont font,
            int lineHeightPadding,
            int iconWidthPadding,
            int borderPadding,
            int separatorThickness,
            int separatorRowHeight,
            int underlineThickness,
            int underlineOffset)
        {
            Font = font;
            LineHeightPadding = lineHeightPadding;
            IconWidthPadding = iconWidthPadding;
            BorderPadding = borderPadding;

            SeparatorThickness = separatorThickness;
            SeparatorRowHeight = separatorRowHeight;

            UnderlineThickness = underlineThickness;
            UnderlineOffset = underlineOffset;
        }

        public static TooltipStyle Default =>
              new TooltipStyle(
                  Game1.smallFont,
                  3,   //line height padding
                  3,   //icon width padding
                  20,   //border padding

                  2,    //separator thickness
                  10,   //separator row height

                  2,    //underline thickness
                  6     //underline offset
              );

    }
}
