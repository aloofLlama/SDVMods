using PlantingDay.Helpers;
using PlantingDay.Models;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PlantingDay.PlantDatabase;
using Microsoft.Xna.Framework;


//namespace PlantingDay
//{
//    public static class IconBuilder
//    {
//        public static void BuildIconsForAllPlants(Dictionary<string, PlantInfo> plants)
//        {
//            foreach (var plant in plants.Values)
//            {
//                if (plant.Seed != null)
//                    plant.Seed.Icon = IconHelper.FromObjectId(plant.Seed.Id);

//                if (plant.Harvest != null)
//                    plant.Harvest.Icon = IconHelper.FromObjectId(plant.Harvest.Id);
//            }
//        }
//        public static class IconHelper
//        {
//            public static IconRef? FromObjectId(string id, float scale = 1f)
//            {
//                var item = ItemRegistry.Create(id);
//                if (item == null)
//                    return null;

//                return FromItem(item, scale);
//            }

//            public static IconRef FromItem(Item item, float scale = 1f)
//            {
//                Rectangle src = Game1.getSourceRectForStandardTileSheet(
//                    Game1.objectSpriteSheet,
//                    item.ParentSheetIndex,
//                    16,
//                    16
//                );

//                return new IconRef(Game1.objectSpriteSheet, src, 16, scale);
//            }
//        }
//    }
//}
