using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using System;
using System.Collections.Generic;

namespace SDVCommon.Icons
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

    public enum IconKey
    {
        Warning,

        // Plant features
        Rainbow,
        WaterSeeds,
        Watercan,
        Scythe,
        Trellis,
        Spiral,

        // Economics
        GoldStar,
        StarToken,
        LittleCoin,
        NightStars,
        TravelingCart,

        // Monsters
        DustSprite,
        Grub,
        MagmaDuggy,
        HotHead,
        Mummy,
        Serpent,
        PurpleSlime
    }

    public static class TooltipIcons
    {
        private static readonly Dictionary<IconKey, Icon> _icons = new();
        private static readonly Dictionary<string, Texture2D> _sheets = new();

        public static Icon Get(IconKey key) => _icons[key];

        public static void Initialize()
        {
            //
            // Load all spritesheets ONCE
            //
            LoadSheet("Cursors", "LooseSprites/Cursors");
            LoadSheet("Cursors1_6", "LooseSprites/Cursors_1_6");
            LoadSheet("Cursors2", "LooseSprites/Cursors2");
            LoadSheet("Objects", "Maps/springobjects");
            LoadSheet("Beach", "Maps/spring_beach");
            LoadSheet("Crops", "Tilesheets/crops");
            LoadSheet("Tools", "TileSheets/tools");
            LoadSheet("Weapons", "TileSheets/weapons");
            LoadSheet("Fences", "LooseSprites/Fence1");
            LoadSheet("Debris", "Tilesheets/Debris");
            LoadSheet("Furniture", "Tilesheets/Furniture");
            LoadSheet("WizardFurniture", "Tilesheets/wizard_furniture");
            LoadSheet("DustSprite", "Characters/Monsters/Dust Spirit");
            LoadSheet("Grub", "Characters/Monsters/Grub");
            LoadSheet("MagmaDuggy", "Characters/Monsters/Magma Duggy");
            LoadSheet("Hothead", "Characters/Monsters/Hot Head");
            LoadSheet("Mummy", "Characters/Monsters/Mummy");
            LoadSheet("Serpent", "Characters/Monsters/Serpent");
            //LoadSheet("PurpleSlime", "Characters/Monsters/Purple Slime");




            //
            // Register icons
            //
            Add(IconKey.Warning, Sheet("Cursors"), new Rectangle(320, 496, 16, 16), 16, 2f);

            // Plant features
            Add(IconKey.Rainbow, Sheet("Cursors"), new Rectangle(596, 1888, 16, 16), 16, 1.5f);
            Add(IconKey.WaterSeeds, Sheet("Beach"), new Rectangle(160, 112, 16, 16), 16, 1.5f);
            Add(IconKey.Watercan, Sheet("Tools"), new Rectangle(48, 225, 16, 16), 16, 2f);
            Add(IconKey.Scythe, Sheet("Weapons"), new Rectangle(112, 80, 16, 16), 16, 2f);
            Add(IconKey.Trellis, Sheet("Crops"), new Rectangle(0, 611, 16, 28), 28, 1f);
            Add(IconKey.Spiral, Sheet("Cursors2"), new Rectangle(66, 242, 12, 12), 12, 2.5f);

            // Economics
            Add(IconKey.StarToken, Sheet("Cursors"), new Rectangle(338, 400, 8, 8), 8, 2f);
            Add(IconKey.GoldStar, Sheet("Cursors"), new Rectangle(346, 400, 8, 8), 8, 2f);
            Add(IconKey.LittleCoin, Sheet("Cursors"), new Rectangle(290, 414, 6, 6), 6, 1f);
            Add(IconKey.NightStars, Sheet("Cursors1_6"), new Rectangle(337, 375, 11, 11), 11, 2f);
            Add(IconKey.TravelingCart, Sheet("Cursors"), new Rectangle(192, 1411, 20, 20), 20, 1.5f);

            // Monsters
            Add(IconKey.DustSprite, Sheet("DustSprite"), new Rectangle(33, 10, 13, 12), 13, 2f);
            Add(IconKey.Grub, Sheet("Grub"), new Rectangle(2, 7, 11, 17), 17, 2f);
            Add(IconKey.MagmaDuggy, Sheet("MagmaDuggy"), new Rectangle(1, 33, 14, 12), 14, 1.5f);
            Add(IconKey.HotHead, Sheet("Hothead"), new Rectangle(32, 0, 16, 16), 16, 1.5f);
            Add(IconKey.Mummy, Sheet("Mummy"), new Rectangle(17, 38, 14, 24), 24, 1.25f);
            Add(IconKey.Serpent, Sheet("Serpent"), new Rectangle(69, 0, 26, 31), 31, 1f);
            Add(IconKey.PurpleSlime, Sheet("WizardFurniture"), new Rectangle(81, 240, 14, 14), 14, 1f);

        }

        //
        // Helpers
        //
        private static void LoadSheet(string key, string path)
            => _sheets[key] = Game1.content.Load<Texture2D>(path);

        private static Texture2D Sheet(string key)
            => _sheets[key];

        private static void Add(IconKey key, Texture2D tex, Rectangle rect, int size, float scale)
            => _icons[key] = new Icon(tex, rect, size, scale);
    }
}




//using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Graphics;
//using StardewValley;


//namespace SDVCommon.Icons
//{
//    public readonly struct Icon
//    {
//        public Texture2D Texture { get; }
//        public Rectangle Source { get; }
//        public int Size { get; }
//        public float Scale { get; }

//        public Icon(Texture2D texture, Rectangle source, int size = 16, float scale = 1f)
//        {
//            Texture = texture;
//            Source = source;
//            Size = size;
//            Scale = scale;
//        }
//    }



//    // Loads spritesheets and exposes ready-to-use icons
//    public static class TooltipIcons
//    {
//        // Icons
//        public static Icon Warning { get; private set; }

//        // For the plant features
//        public static Icon Rainbow { get; private set; }
//        public static Icon WaterSeeds { get; private set; }
//        public static Icon Watercan { get; private set; }
//        public static Icon Scythe { get; private set; }
//        public static Icon Trellis { get; private set; }
//        public static Icon Spiral { get; private set; }



//        // For economics
//        public static Icon GoldStar { get; private set; }
//        public static Icon StarToken { get; private set; } //From Valley Fair
//        public static Icon LittleCoin { get; private set; }
//        public static Icon NightStars { get; private set; }
//        public static Icon TravelingCart { get; private set; }

//        //For Monsters
//        public static Icon DustSprite { get; private set; }
//        public static Icon Grub { get; private set; }
//        public static Icon MagmaDuggy { get; private set; }
//        public static Icon HotHead { get; private set; }





//        //Spritesheets
//        public static Texture2D? Cursors { get; private set; }
//        public static Texture2D? Cursors1_6 { get; private set; }
//        public static Texture2D? Cursors2 { get; private set; }
//        public static Texture2D? Objects { get; private set; }
//        public static Texture2D? Beach { get; private set; }
//        public static Texture2D? Tools { get; private set; }
//        public static Texture2D? Weapons { get; private set; }
//        public static Texture2D? Fences { get; private set; }
//        public static Texture2D? Debris { get; private set; }
//        public static Texture2D? Furniture { get; private set; }
//        public static Texture2D? WizardFurniture { get; private set; }




//        public static void Initialize()
//        {
//            // Load vanilla spritesheets
//            Cursors = Game1.content.Load<Texture2D>("LooseSprites/Cursors");
//            Cursors1_6 = Game1.content.Load<Texture2D>("LooseSprites/Cursors_1_6");
//            Cursors2 = Game1.content.Load<Texture2D>("LooseSprites/Cursors2");
//            Objects = Game1.content.Load<Texture2D>("Maps/springobjects");
//            Beach = Game1.content.Load<Texture2D>("Maps/spring_beach");
//            Tools = Game1.content.Load<Texture2D>("TileSheets/tools");
//            Weapons = Game1.content.Load<Texture2D>("TileSheets/weapons");
//            Fences = Game1.content.Load<Texture2D>("LooseSprites/Fence1");
//            Debris = Game1.content.Load<Texture2D>("Tilesheets/Debris");
//            Furniture = Game1.content.Load<Texture2D>("Tilesheets/Furniture");
//            WizardFurniture = Game1.content.Load<Texture2D>("Tilesheets/wizard_furniture");
//            //monster spritesheets
//            DustSpriteSS = Game1.content.Load<Texture2D>("Tilesheets/wizard_furniture");

//            //-------
//            // Static UI icons (Icon)
//            //-------

//            Warning = new Icon(Cursors, new Rectangle(320, 496, 16, 16), size: 16, scale: 2f);

//            //For plant features
//            Rainbow = new Icon(Cursors, new Rectangle(596, 1888, 16, 16), size: 16, scale: 1.5f);
//            //WaterSeeds = new Icon(Cursors, new Rectangle(536, 1945, 7, 8), size: 8, scale: 3f);
//            //WaterSeeds = new Icon(Cursors, new Rectangle(247, 405, 8, 8), size: 8, scale: 2.5f);
//            WaterSeeds = new Icon(Beach, new Rectangle(160, 112, 16, 16), size: 16, scale: 1.5f);
//            Watercan = new Icon(Tools, new Rectangle(48, 225, 16, 16), size: 16, scale: 2f);
//            Scythe = new Icon(Weapons, new Rectangle(112, 80, 16, 16), size: 16, scale: 2f);
//            Trellis = new Icon(Fences, new Rectangle(43, 130, 5, 18), size: 18, scale: 2f);
//            Spiral = new Icon(Cursors2, new Rectangle(66, 242, 12, 12), size: 12, scale: 2.5f);

//            //Economics Info
//            StarToken = new Icon(Cursors, new Rectangle(338, 400, 8, 8), size: 8, scale: 2f);
//            GoldStar = new Icon(Cursors, new Rectangle(346, 400, 8, 8), size: 8, scale: 2f);
//            LittleCoin = new Icon(Cursors, new Rectangle(290, 414, 6, 6), size: 6, scale: 1f);
//            NightStars = new Icon(Cursors1_6, new Rectangle(337, 375, 11, 11), size: 11, scale: 2f);
//            TravelingCart = new Icon(Cursors, new Rectangle(192, 1411, 20, 20), size: 20, scale: 1.5f);

//            //Monsters
//            DustSprite = new Icon(Cursors, new Rectangle(338, 400, 8, 8), size: 8, scale: 2f);

//        }
//    }


//}
