using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Reflection;
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

            }

            //if (menu.GetType().FullName == "StardewValley.Menus.CookMenu")
            //{
            //    var t = menu.GetType();

            //    var prop = t.GetProperty("hoveredItem");
            //    if (prop != null)
            //        return prop.GetValue(menu) as Item;

            //    var field = t.GetField("hoveredItem");
            //    if (field != null)
            //        return field.GetValue(menu) as Item;
            //}

            //// Better Crafting support (reflection)
            //var type = menu.GetType();

            //if (type.FullName == "BetterCrafting.BetterCraftingPage")
            //{
            //    // Get the root UI element
            //    var rootField = type.GetField("root", BindingFlags.NonPublic | BindingFlags.Instance);
            //    if (rootField == null)
            //        return null;

            //    var root = rootField.GetValue(menu);
            //    if (root == null)
            //        return null;

            //    // root is a UIElement — find the element under the mouse
            //    var method = root.GetType().GetMethod("GetElementAt", BindingFlags.Public | BindingFlags.Instance);
            //    if (method == null)
            //        return null;

            //    var element = method.Invoke(root, new object[] { Game1.getMouseX(), Game1.getMouseY() });
            //    if (element == null)
            //        return null;

            //    // Many BC elements expose an Item property
            //    var itemProp = element.GetType().GetProperty("Item");
            //    if (itemProp != null)
            //        return itemProp.GetValue(element) as Item;

            //    // Some expose HeldItem instead
            //    var heldProp = element.GetType().GetProperty("HeldItem");
            //    if (heldProp != null)
            //        return heldProp.GetValue(element) as Item;

            //}
            return null;

        }

    }
}
