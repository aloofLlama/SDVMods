using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PlantingDay.Models;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlantingDay.Helpers
{
    internal class IconRenderer_plants
    {
        public static void InitializeIcons(PlantInfo plant)
        {
            //Seed Icon
            if (plant.Seed != null)
            {
                var item = ItemRegistry.Create(plant.Seed.Id);
                if (item != null)
                    plant.SeedIconTexture = RenderItemIcon(item, 16);

            }

            //Harvest Icon
            if (plant.Harvest != null)
            {
                var item = ItemRegistry.Create(plant.Harvest.Id);
                if (item != null)
                    plant.HarvestIconTexture = RenderItemIcon(item, 32);
            }
        }

        public static Texture2D RenderItemIcon(Item item, int size = 16)
        {
            var device = Game1.graphics.GraphicsDevice;

            // Step 1: draw into a 64x64 buffer (the size drawInMenu expects)
            const int bufferSize = 64;
            var buffer = new RenderTarget2D(device, bufferSize, bufferSize);

            //var oldTargets = device.GetRenderTargets();

            device.SetRenderTarget(buffer);
            device.Clear(Color.Transparent);

            SpriteBatch batch = Game1.spriteBatch;

            batch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp);

            item.drawInMenu(
                batch,
                Vector2.Zero,
                1f,            // natural scale (same as before)
                1f,
                1f,
                StackDrawType.Hide,
                Color.White,
                false
            );

            batch.End();
            device.SetRenderTarget(null);

            // Step 2: downscale the 64x64 buffer into your final 16x16 icon
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
