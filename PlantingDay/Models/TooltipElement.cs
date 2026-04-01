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

        public List<InlineSegment>? InlineSegments { get; set; }

        // Seasons (optional)
        public List<string> Seasons { get; set; } = new();
        public List<Color> SeasonColors { get; set; } = new();
        public List<bool> SeasonBold { get; set; } = new();


        // Icon (optional)
        public IconRef? Icon { get; set; }




        // Layout
        public int PaddingTop { get; set; } = 2;
        public int PaddingBottom { get; set; } = 2;



    }

}


