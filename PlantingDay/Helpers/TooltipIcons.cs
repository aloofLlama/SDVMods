using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;


namespace PlantingDay.Helpers
{
    // Bundles texture + source rect + size into one object
    public readonly struct IconRef
    {
        public Texture2D Texture { get; }
        public Rectangle Source { get; }
        public int Size { get; }

        public IconRef(Texture2D texture, Rectangle source, int size = 16)
        {
            Texture = texture;
            Source = source;
            Size = size;
        }
    }

    // Loads spritesheets and exposes ready-to-use icons
    public static class TooltipIcons
    {
        public static Texture2D? Cursors { get; private set; }
        public static Texture2D? Objects { get; private set; }
        public static Texture2D? Maps { get; private set; }


        // Icons
        public static IconRef Rainbow { get; private set; } //using prismatic shard
        public static IconRef Warning { get; private set; }
        public static IconRef WaterSeeds { get; private set; }


        public static void Load()
        {
            Cursors = Game1.content.Load<Texture2D>("LooseSprites\\Cursors");
            Objects = Game1.content.Load<Texture2D>("Maps\\springobjects");
            Maps = Game1.content.Load<Texture2D>("Maps\\spring_beach");


            Warning = new IconRef(Cursors, new Rectangle(320, 496, 16, 16));

            Rainbow = new IconRef(Objects, new Rectangle(32, 48, 16, 16));
            WaterSeeds = new IconRef(Maps, new Rectangle(160, 112, 16, 16));

        }
    }


}
