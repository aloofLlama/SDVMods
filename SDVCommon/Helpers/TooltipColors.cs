using Microsoft.Xna.Framework;
using Netcode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDVCommon.Helpers
{
    public class TooltipColors
    {
        public static readonly Color Normal = Color.Black;
        public static readonly Color Deemphasize = Color.DarkSlateGray;
        public static readonly Color Muted = Color.DimGray;
        public static readonly Color Warning = Color.DarkRed;
        public static readonly Color Perfection = Color.MediumPurple; //TODO replace hardcoded with this
        public static readonly Color Separator = new(188,148,60);

        //Plant
        public static readonly Color Paddy = Color.CornflowerBlue;
        public static readonly Color Trellis = Color.Orange;



        //seasons
        public static readonly Color SpringColor = new(80, 200, 120); //green
        public static readonly Color SummerColor = new(208, 0, 255); //purple
        public static readonly Color FallColor = new(170, 70, 0); //orange
        public static readonly Color WinterColor = new(100, 160, 255); //blue

    }
}
