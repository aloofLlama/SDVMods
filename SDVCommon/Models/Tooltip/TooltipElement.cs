using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SDVCommon.Helpers.Tooltip;
using SDVCommon.Icons;
using StardewValley;


namespace SDVCommon.Models.Tooltip
{

    public class TooltipElement
    {
        // Text
        public string? Text { get; set; }
        public Color TextColor { get; set; } = TooltipColors.Normal;
        public SpriteFont Font { get; set; } = Game1.smallFont;
        public bool Bold { get; set; }
        public bool Underline { get; set; }


        public Icon? Icon { get; set; }


        public List<InlineSegment>? InlineSegments { get; set; }
        public bool IsSeparator { get; set; } = false;
    }

    // Supports putting multiple items with different formatting on the same line.
    public class InlineSegment
    {
        public Icon? Icon { get; set; }
        public string Text { get; set; } = "";
        public Color TextColor { get; set; } = TooltipColors.Normal;
        public SpriteFont Font { get; set; } = Game1.smallFont;
        public bool Bold { get; set; }
        public bool Underline { get; set; }
        public bool IsLineBreak { get; set; }
    }
}




