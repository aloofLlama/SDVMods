using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PlantingDay.Models;
using StardewModdingAPI;
using StardewValley;
using System.Xml.Linq;
using xTile;


namespace PlantingDay.Helpers.Icons
{
    public readonly struct Icon
    {
        public Texture2D Texture { get; }
        public Rectangle Source { get; }
        public int Size { get; }
        public float Scale { get; }

        public Icon(Texture2D texture, Rectangle source, int size = 16, float scale = 1f)
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
        // Icons
        public static Icon Warning { get; private set; }

        // For the plant features
        public static Icon Rainbow { get; private set; }
        public static Icon WaterSeeds { get; private set; }
        public static Icon Watercan { get; private set; }
        public static Icon Scythe { get; private set; }
        public static Icon Trellis { get; private set; }
        public static Icon Spiral { get; private set; }



        // For economics
        public static Icon GoldStar { get; private set; }
        public static Icon StarToken { get; private set; } //From Valley Fair
        public static Icon LittleCoin { get; private set; }
        public static Icon NightStars { get; private set; }
        
        
        //Spritesheets
        public static Texture2D? Cursors { get; private set; }
        public static Texture2D? Cursors2 { get; private set; }
        public static Texture2D? Objects { get; private set; }
        public static Texture2D? Beach { get; private set; }
        public static Texture2D? Tools { get; private set; }
        public static Texture2D? Weapons { get; private set; }
        public static Texture2D? Fences { get; private set; }
        public static Texture2D? Debris { get; private set; }
        public static Texture2D? Furniture { get; private set; }





        public static void Initialize()
        {
            // Load vanilla spritesheets
            Cursors = Game1.content.Load<Texture2D>("LooseSprites/Cursors");
            Cursors2 = Game1.content.Load<Texture2D>("LooseSprites/Cursors2");
            Objects = Game1.content.Load<Texture2D>("Maps/springobjects");
            Beach = Game1.content.Load<Texture2D>("Maps/spring_beach");
            Tools = Game1.content.Load<Texture2D>("TileSheets/tools");
            Weapons = Game1.content.Load<Texture2D>("TileSheets/weapons");
            Fences = Game1.content.Load<Texture2D>("LooseSprites/Fence1");
            Debris = Game1.content.Load<Texture2D>("Tilesheets/Debris");
            Furniture = Game1.content.Load<Texture2D>("Tilesheets/Furniture");



            //-------
            // Static UI icons (Icon)
            //-------

            Warning = new Icon(Cursors, new Rectangle(320, 496, 16, 16), size: 16, scale: 2f);

            //For plant features
            Rainbow = new Icon(Cursors, new Rectangle(596, 1888, 16, 16), size: 16, scale: 1.5f);
            //WaterSeeds = new Icon(Cursors, new Rectangle(536, 1945, 7, 8), size: 8, scale: 3f);
            //WaterSeeds = new Icon(Cursors, new Rectangle(247, 405, 8, 8), size: 8, scale: 2.5f);
            WaterSeeds = new Icon(Beach, new Rectangle(160, 112, 16, 16), size: 16, scale: 1.5f);
            Watercan = new Icon(Tools, new Rectangle(48, 225, 16, 16), size: 16, scale: 2f);
            Scythe = new Icon(Weapons, new Rectangle(112, 80, 16, 16), size: 16, scale: 2f);
            Trellis = new Icon(Fences, new Rectangle(43, 130, 5, 18), size: 18, scale: 2f);
            Spiral = new Icon(Cursors2, new Rectangle(66, 242, 12, 12), size: 12, scale: 2.5f);

            //Economics Info
            StarToken = new Icon(Cursors, new Rectangle(338, 400, 8, 8), size: 8, scale: 2f);
            GoldStar = new Icon(Cursors, new Rectangle(346, 400, 8, 8), size: 8, scale: 2f);
            LittleCoin = new Icon(Cursors, new Rectangle(290, 414, 6, 6), size: 6, scale: 1f);
            NightStars = new Icon(Debris, new Rectangle(1, 214, 8, 8), size: 8, scale: 1.5f);
            // or try the moon from furniture or wizard furniture or stars from cursors 1_6

        }
    }


}
