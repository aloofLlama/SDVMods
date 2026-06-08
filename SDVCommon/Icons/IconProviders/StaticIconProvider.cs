using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using SDVCommon.Icons;
using StardewValley;

public class StaticIconProvider : IIconProvider
{
    private readonly Dictionary<IconKey, Icon> _icons = new();
    private readonly Dictionary<string, Texture2D> _sheets = new();

    public StaticIconProvider()
    {
        LoadSheets();
        RegisterIcons();
    }

    public bool CanHandle(string id)
        => Enum.TryParse<IconKey>(id, out _);

    public Icon? LoadIcon(string id)
    {
        if (Enum.TryParse<IconKey>(id, out var key) && _icons.TryGetValue(key, out var icon))
            return icon;

        return null;
    }

    private void LoadSheets()
    {
        //Variety basic sheets
        _sheets["Cursors"] = Game1.content.Load<Texture2D>("LooseSprites/Cursors");
        _sheets["Cursors1_6"] = Game1.content.Load<Texture2D>("LooseSprites/Cursors_1_6");
        _sheets["Cursors2"] = Game1.content.Load<Texture2D>("LooseSprites/Cursors2");
        _sheets["Objects"] = Game1.content.Load<Texture2D>("Maps/springobjects");

        //Random stuff sheets
        _sheets["Beach"] = Game1.content.Load<Texture2D>("Maps/spring_beach");
        _sheets["Crops"] = Game1.content.Load<Texture2D>("TileSheets/crops");
        _sheets["Tools"] = Game1.content.Load<Texture2D>("TileSheets/tools");
        _sheets["Weapons"] = Game1.content.Load<Texture2D>("TileSheets/weapons");
        //_sheets["Fences"] = Game1.content.Load<Texture2D>("LooseSprites/Fence1");
        //_sheets["Debris"] = Game1.content.Load<Texture2D>("TileSheets/Debris");
        //_sheets["Furniture"] = Game1.content.Load<Texture2D>("TileSheets/Furniture");
        _sheets["WizardFurniture"] = Game1.content.Load<Texture2D>("TileSheets/wizard_furniture");
        //_sheets["Craftables"] = Game1.content.Load<Texture2D>("TileSheets/Craftables");

        //Monster sheets
        _sheets["DustSprite"] = Game1.content.Load<Texture2D>("Characters/Monsters/Dust Spirit");
        _sheets["Grub"] = Game1.content.Load<Texture2D>("Characters/Monsters/Grub");
        _sheets["MagmaDuggy"] = Game1.content.Load<Texture2D>("Characters/Monsters/Magma Duggy");
        _sheets["Hothead"] = Game1.content.Load<Texture2D>("Characters/Monsters/Hot Head");
        _sheets["Mummy"] = Game1.content.Load<Texture2D>("Characters/Monsters/Mummy");
        _sheets["Serpent"] = Game1.content.Load<Texture2D>("Characters/Monsters/Serpent");
    }



    private void RegisterIcons()
    {
        Add(IconKey.Warning, "Cursors", new Rectangle(320, 496, 16, 16), 16, 2f);
        Add(IconKey.Heart, "Cursors", new Rectangle(211, 428, 7, 6), 7, 3f);
        Add(IconKey.Plate, "Objects", new Rectangle(16, 128, 16, 16), 16, 1.5f);

        // Plant features
        Add(IconKey.Rainbow, "Cursors", new Rectangle(596, 1888, 16, 16), 16, 1.5f);
        Add(IconKey.WaterSeeds, "Beach", new Rectangle(160, 112, 16, 16), 16, 1.5f);
        Add(IconKey.Watercan, "Tools", new Rectangle(48, 225, 16, 16), 16, 2f);
        Add(IconKey.Scythe, "Weapons", new Rectangle(112, 80, 16, 16), 16, 2f);
        Add(IconKey.Trellis, "Crops", new Rectangle(0, 611, 16, 28), 28, 1f);
        Add(IconKey.Spiral, "Cursors2", new Rectangle(66, 242, 12, 12), 12, 2.5f);

        // Economics
        Add(IconKey.GoldStar, "Cursors", new Rectangle(346, 400, 8, 8), 8, 2f);
        Add(IconKey.StarToken, "Cursors", new Rectangle(338, 400, 8, 8), 8, 2f);
        Add(IconKey.LittleCoin, "Cursors", new Rectangle(193, 373, 9, 9), 9, 1.8f);
        Add(IconKey.NightStars, "Cursors1_6", new Rectangle(337, 375, 11, 11), 11, 2f);
        Add(IconKey.TravelingCart, "Cursors", new Rectangle(192, 1411, 20, 20), 20, 1.5f);

        // Monsters
        Add(IconKey.DustSprite, "DustSprite", new Rectangle(33, 10, 13, 12), 13, 2f);
        Add(IconKey.Grub, "Grub", new Rectangle(2, 7, 11, 17), 17, 2f);
        Add(IconKey.MagmaDuggy, "MagmaDuggy", new Rectangle(1, 33, 14, 12), 14, 1.5f);
        Add(IconKey.HotHead, "Hothead", new Rectangle(32, 0, 16, 16), 16, 1.5f);
        Add(IconKey.Mummy, "Mummy", new Rectangle(17, 38, 14, 24), 24, 1.25f);
        Add(IconKey.Serpent, "Serpent", new Rectangle(69, 0, 26, 31), 31, 1f);
        Add(IconKey.PurpleSlime, "WizardFurniture", new Rectangle(81, 240, 14, 14), 14, 1f);

    }

    private void Add(IconKey key, string sheet, Rectangle rect, int size, float scale)
    {
        _icons[key] = new Icon(_sheets[sheet], rect, size, scale);
    }

}

    public enum IconKey
{
    Warning,
    Heart,
    Plate,

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

}

