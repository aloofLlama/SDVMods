using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace SeedInfo
{

    public class TooltipElement
        {
        public string? Text { get; set; }
        public Color TextColor { get; set; } = Color.White;
        public bool Bold { get; set; } = false;

        public Texture2D? Icon { get; set; }
        public Rectangle? IconSource { get; set; }
        public int IconSize { get; set; } = 16;

        public int PaddingTop { get; set; } = 2;
        public int PaddingBottom { get; set; } = 2;
        public List<string>? Seasons { get; set; } // for putting multiple seasons on one line


    }

}


