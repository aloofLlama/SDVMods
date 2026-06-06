using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SDVCommon.Icons
{
    public readonly struct Icon
    {
        public Texture2D Texture { get; }
        public Rectangle Source { get; }
        public int Size { get; }
        public float Scale { get; }
        public Color Tint { get; }


        public Icon(Texture2D texture, Rectangle source, int size = 16, float scale = 1f, Color? tint = null)
        {
            Texture = texture;
            Source = source;
            Size = size;
            Scale = scale;
            Tint = tint ?? Color.White;
        }
    }

}




