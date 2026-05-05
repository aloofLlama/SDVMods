using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Text;

namespace SDVCommon.Helpers
{
    internal class HoveredItem
    {

        public static Item? GetFromAnyMenu()
        {
            IClickableMenu menu = Game1.activeClickableMenu;
            if (menu == null)
                return null;

            switch (menu)
            {
                // Inventory (GameMenu → InventoryPage)
                case GameMenu gm:
                    if (gm.currentTab != GameMenu.inventoryTab)
                        return null;

                    var invPage = gm.pages
                        .OfType<InventoryPage>()
                        .FirstOrDefault();

                    return invPage?.hoveredItem;

                // Chests, Fridges, Dressers, Junimo Chests, etc.
                case StardewValley.Menus.ItemGrabMenu chest:
                    return chest.hoveredItem;

                // Pierre’s shop, Traveling Cart, Krobus, Qi, etc.
                case StardewValley.Menus.ShopMenu shop:
                    return shop.hoveredItem as Item;

                default:
                    return null;

            }
        }

    }
}
