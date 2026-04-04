using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PlantingDay.Models;
using StardewModdingAPI;
using StardewValley;
using System.Xml.Linq;
using xTile;


namespace PlantingDay.Helpers
{

    public readonly struct IconRef
    {
        public Texture2D Texture { get; }
        public Rectangle Source { get; }
        public int Size { get; }
        public float Scale { get; }

        public IconRef(Texture2D texture, Rectangle source, int size = 16, float scale = 1f)
        {
            Texture = texture;
            Source = source;
            Size = size;
            Scale = scale;
        }
    }


    // Loads spritesheets and exposes ready-to-use icons
    public static class TooltipIcons
    {
        public static Texture2D? Cursors { get; private set; }
        public static Texture2D? Objects { get; private set; }
        public static Texture2D? Beach { get; private set; }
        public static Texture2D? Tools { get; private set; }
        public static Texture2D? Weapons { get; private set; }




        // Icons
        public static IconRef Rainbow { get; private set; }
        public static IconRef Warning { get; private set; }
        public static IconRef WaterSeeds { get; private set; }
        public static IconRef Watercan { get; private set; }
        public static IconRef Scythe { get; private set; }
        public static IconRef GoldStar { get; private set; }
        public static IconRef LittleCoin { get; private set; }
        public static IconRef NightStars { get; private set; }




        //public static IconRef Spiral { get; private set; }




        public static void Initialize()
        {
            // Load vanilla spritesheets
            Cursors = Game1.content.Load<Texture2D>("LooseSprites/Cursors");
            Objects = Game1.content.Load<Texture2D>("Maps/springobjects");
            Beach = Game1.content.Load<Texture2D>("Maps/spring_beach");
            Tools = Game1.content.Load<Texture2D>("TileSheets/tools");
            Weapons = Game1.content.Load<Texture2D>("TileSheets/weapons");

            // Static UI icons (IconRef)
            //Growth Info
            Warning = new IconRef(Cursors, new Rectangle(320, 496, 16, 16), size: 16, scale: 2f);
            Rainbow = new IconRef(Cursors, new Rectangle(596, 1888, 16, 16), size: 16, scale: 1.5f);
            WaterSeeds = new IconRef(Beach, new Rectangle(160, 112, 16, 16), size: 16, scale: 2f);
            Watercan = new IconRef(Tools, new Rectangle(48, 225, 16, 16), size: 16, scale: 2f);
            Scythe = new IconRef(Weapons, new Rectangle(112, 80, 16, 16), size: 16, scale: 2f);
            
            //Economics Info
            GoldStar = new IconRef(Cursors, new Rectangle(346, 400, 8, 8), size: 8, scale: 2f);
            LittleCoin = new IconRef(Cursors, new Rectangle(290, 414, 6, 6), size: 6, scale: 2f);
            NightStars = new IconRef(Objects, new Rectangle(272, 224, 16, 16), size: 16, scale: 1f);

        }
    }


}
