using Microsoft.Xna.Framework;

namespace SDVCommon.Icons
{
    public static class IconExtensions
    {
        public static Icon WithScale(this Icon icon, float scale)
            => new Icon(icon.Texture, icon.Source, icon.Size, scale, icon.Tint);

        public static Icon WithTint(this Icon icon, Color tint)
            => new Icon(icon.Texture, icon.Source, icon.Size, icon.Scale, tint);

        public static Icon? GetIcon(this IconKey key)
            => IconRegistry.GetIcon(key.ToString());
    }
}
