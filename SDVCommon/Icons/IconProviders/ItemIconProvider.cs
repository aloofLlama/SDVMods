using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SDVCommon.Icons;
using StardewValley;
using StardewValley.Objects;
using StardewValley.ItemTypeDefinitions;

public class ItemIconProvider : IIconProvider
{
    public bool CanHandle(string id)
    {
        // Accepts both qualified and unqualified IDs, as ItemRegistry can resolve either
        return ItemRegistry.GetData(id) != null;
    }

    public Icon? LoadIcon(string id)
    {
        // Resolve item data (works for unqualified + qualified IDs)
        var data = ItemRegistry.GetData(id);
        if (data == null)
            return null;

        // Texture + source rect come directly from the item definition
        Texture2D tex = data.GetTexture();
        Rectangle rect = data.GetSourceRect();

        // Determine tint (artisan goods, colored objects)
        Color tint = GetTint(id);

        // Size = height of the source rect (16 for objects, 32 for big craftables)
        int size = rect.Height;

        return new Icon(tex, rect, size, 2f, tint);
    }

    // ------------------------------
    // TINT LOGIC
    // ------------------------------
    private Color GetTint(string id)
    {
        try
        {
            Item? item = ItemRegistry.Create(id);

            if (item is ColoredObject colored)
                return colored.color.Value;

            return Color.White;
        }
        catch
        {
            return Color.White;
        }
    }
}

























//public static class TooltipIcons
//{
//    private static readonly Dictionary<IconKey, Icon> _icons = new();
//    private static readonly Dictionary<string, Texture2D> _sheets = new();

//    public static Icon Get(IconKey key) => _icons[key];


//    //
//    // Helpers
//    //
//    private static void LoadSheet(string key, string path)
//        => _sheets[key] = Game1.content.Load<Texture2D>(path);

//    private static Texture2D Sheet(string key)
//        => _sheets[key];

//    private static void Add(IconKey key, Texture2D tex, Rectangle rect, int size, float scale)
//        => _icons[key] = new Icon(tex, rect, size, scale);


//    public static Icon GetIconForGameObject(string qualifiedId, float scale)
//    {
//        Texture2D sheet;
//        int iconx;
//        int icony;
//        int x;
//        int y;
//        int size;

//        switch (qualifiedId)
//        {
//            //
//            // BIG CRAFTABLES (BC)
//            //
//            case var id when id.StartsWith("(BC)"):
//                {
//                    int bcId = int.Parse(id.AsSpan(4));
//                    sheet = Sheet("Craftables");

//                    iconx = 16;
//                    icony = 32;

//                    const int cols = 8; // Craftables sheet is 8 across

//                    int col = bcId % cols;
//                    int row = bcId / cols;

//                    x = col * iconx;
//                    y = row * icony;

//                    size = icony; // 32px tall
//                    break;
//                }

//            //
//            // OBJECTS (O)
//            //
//            case var id when id.StartsWith("(O)"):
//                {
//                    int objId = int.Parse(id.AsSpan(3));
//                    sheet = Sheet("Objects");

//                    iconx = 16;
//                    icony = 16;

//                    const int cols = 24; // springobjects is 24 across

//                    int col = objId % cols;
//                    int row = objId / cols;

//                    x = col * iconx;
//                    y = row * icony;

//                    size = iconx; // 16px
//                    break;
//                }

//            default:
//                throw new NotSupportedException($"Unsupported QualifiedItemId: {qualifiedId}");
//        }

//        Rectangle rect = new Rectangle(x, y, iconx, icony);
//        return new Icon(sheet, rect, size, scale);
//    }
//}
