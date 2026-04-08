using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PlantingDay.Models;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace PlantingDay.Helpers
{
    internal class IconRenderer_plants
    {
        public static void InitializeIcons(PlantInfo plant)
        {
            //Seed Icon
            if (plant.Seed != null)
            {
                string gameId = IdHelper.ToGameId(plant.Seed.Id);
                var item = ItemRegistry.Create(gameId);
                if (item != null)
                {
                    //MARK plant.SeedIconTexture = RenderItemIcon(item, 16);
                    var tex = RenderItemIcon(item, 16);
                    plant.SeedIconRef = new IconRef(
                        tex,
                        new Rectangle(0, 0, tex.Width, tex.Height),
                        size: tex.Width,   // or 16
                        scale: 2f          // ← make it bigger
                    );
                }

            }

            //Harvest Icon
            if (plant.Harvest!= null)
            {
                //string gameId = IdHelper.ToGameId(plant.Harvest.Id);
                var item = ItemRegistry.Create(plant.HarvestId);
                //ModEntry.Instance.Monitor.Log($"ATTEMPT: {plant.HarvestId}", LogLevel.Info);
                if (item != null)
                {
                    //MARK plant.HarvestIconTexture = RenderItemIcon(item, 32);
                    var tex = RenderItemIcon(item, 32);
                    plant.HarvestIconRef = new IconRef(
                        tex,
                        new Rectangle(0, 0, tex.Width, tex.Height),
                        size: tex.Width,   // or 16
                        scale: 1f          // ← make it bigger
                    );
                    //ModEntry.Instance.Monitor.Log($"SUCEED: {plant.HarvestId}", LogLevel.Info);

                }
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
                1f,           
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
