using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace PlantingDay.Helpers
{
    internal class TooltipColors
    {
        public static readonly Color Normal = Color.Black;
        public static readonly Color Muted = Color.Gray;
        public static readonly Color Warning = Color.DarkRed;
        public static readonly Color Trellis = Color.Orange;
        public static readonly Color Paddy = Color.CornflowerBlue;

        //seasons
        public static readonly Color SpringColor = new(80, 200, 120); //green
        public static readonly Color SummerColor = new(208, 0, 255); //purple
        public static readonly Color FallColor = new(170, 70, 0); //orange
        public static readonly Color WinterColor = new(100, 160, 255); //blue

    }
}
