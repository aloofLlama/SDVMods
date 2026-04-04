using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PlantingDay.Helpers;


namespace PlantingDay.Models
{

    public class TooltipElement
    {
        // Text
        public string? Text { get; set; }
        public Color TextColor { get; set; } = TooltipColors.Normal;
        public bool Bold { get; set; }

        // Icon (optional)
        public Texture2D? IconTexture { get; set; }   // dynamic item icons
        public IconRef? IconRef { get; set; }         // static UI icons

        // Layout
        public int PaddingTop { get; set; } = 2;
        public int PaddingBottom { get; set; } = 2;

        public List<InlineSegment>? InlineSegments { get; set; }
        public bool IsSeparator { get; set; } = false;
    }

    // Supports putting multiple items with different formatting on the same line.
    public struct InlineSegment
    {
        public IconRef? IconRef;
        public string Text;
        public Color Color;
        public bool Bold;
    }



}





