using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PlantingDay.Models;
using StardewModdingAPI;
using StardewValley;
using System.Xml.Linq;


namespace PlantingDay.Helpers
{
    // Bundles texture + source rect + size into one object
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
        public static Texture2D? Maps { get; private set; }
        public static Texture2D? Tools { get; private set; }
        public static Texture2D? Weapons { get; private set; }




        // Icons
        public static IconRef Rainbow { get; private set; }
        public static IconRef Warning { get; private set; }
        public static IconRef WaterSeeds { get; private set; }
        public static IconRef Watercan { get; private set; }
        public static IconRef Scythe { get; private set; }

        //public static IconRef Spiral { get; private set; }

        public static int? OverrideIconSize { get; set; }



        public static void Load()
        {
            Cursors = Game1.content.Load<Texture2D>("LooseSprites\\Cursors");
            Objects = Game1.content.Load<Texture2D>("Maps\\springobjects");
            Maps = Game1.content.Load<Texture2D>("Maps\\spring_beach");
            Tools = Game1.content.Load<Texture2D>("Tilesheets\\tools");
            Weapons = Game1.content.Load<Texture2D>("Tilesheets\\weapons");



            Warning = new IconRef(Cursors, new Rectangle(320, 496, 16, 16));
            //Spiral = new IconRef(Cursors, new Rectangle(360, 1438, 10, 10));


            Rainbow = new IconRef(Cursors, new Rectangle(596, 1888, 16, 16), size: 16, scale: 0.6f);

            WaterSeeds = new IconRef(Maps, new Rectangle(160, 112, 16, 16), size: 16, scale: 0.8f);
            Watercan = new IconRef(Tools, new Rectangle(48, 225, 16, 16), size: 16, scale: 0.8f);
            Scythe = new IconRef(Weapons, new Rectangle(112, 80, 16, 16), size: 16, scale: 0.8f);

        }
    }


}
