using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace SDVCommon.Icons
{
    public static class IconRenderer
    {
        public static Texture2D RenderItemIcon(Item item, int size)
        {
            var device = Game1.graphics.GraphicsDevice;

            const int bufferSize = 64;
            var buffer = new RenderTarget2D(device, bufferSize, bufferSize);

            device.SetRenderTarget(buffer);
            device.Clear(Color.Transparent);

            SpriteBatch batch = Game1.spriteBatch;

            batch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp);

            item.drawInMenu(
                batch,
                Vector2.Zero,
                1f,
                1f,
                1f,
                StackDrawType.Hide,
                Color.White,
                false
            );

            batch.End();
            device.SetRenderTarget(null);

            var final = new RenderTarget2D(device, size, size);

            device.SetRenderTarget(final);
            device.Clear(Color.Transparent);

            batch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp);

            float scale = size / (float)bufferSize;
            batch.Draw(buffer, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);

            batch.End();
            device.SetRenderTarget(null);

            return final;
        }
    }
}
