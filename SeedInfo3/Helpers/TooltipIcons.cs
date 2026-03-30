using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;


namespace SeedInfo
{
    // Bundles texture + source rect + size into one object
    public struct IconRef
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
        public static Texture2D Cursors { get; private set; }
        public static Texture2D Objects { get; private set; }

        // Example icons
        public static IconRef Warning { get; private set; }
        public static IconRef Clock { get; private set; }

        public static void Load()
        {
            Cursors = Game1.content.Load<Texture2D>("LooseSprites\\Cursors");
            Objects = Game1.content.Load<Texture2D>("Maps\\springobjects");

            Warning = new IconRef(Cursors, new Rectangle(412, 495, 16, 16));
            Clock = new IconRef(Cursors, new Rectangle(403, 495, 9, 9), size: 12);
        }
    }


}
