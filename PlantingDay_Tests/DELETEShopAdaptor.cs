//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using PlantingDay_Tests.Helpers;
//using StardewValley.GameData.Shops;

//// Abstracts the game's shop data structure into a non-dependency for testing.

//namespace PlantingDay_Tests
//{
//    public static class ShopAdapter
//    {
//        public static Dictionary<string, ShopData> Convert(
//            Dictionary<string, FakeShopData> fake)
//        {
//            return fake.ToDictionary(
//                kvp => kvp.Key,
//                kvp => new ShopData
//                {
//                    Items = kvp.Value.Items.Select(i => new ShopItemData
//                    {
//                        ItemId = i.ItemId,
//                        Price = i.Price
//                    }).ToList()
//                });
//        }
//    }
//}
