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
        PurpleSlime,

        //Other
        Heart,
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
            Add(IconKey.LittleCoin, Sheet("Cursors"), new Rectangle(193, 373, 9, 9), 9, 2f);
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

            // Other
            Add(IconKey.Heart, Sheet("Cursors"), new Rectangle(211, 428, 7, 6), 7, 2.5f);
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




