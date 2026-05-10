using SDVCommon.Helpers;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using System.Reflection;

namespace SDVCommon.GameData
{

    public readonly struct HoverResult
    {
        public Item? Item { get; }
        public NPC? NPC { get; }
        public string? ItemId { get; }

        public bool HasValue => Item != null || NPC != null || ItemId != null;

        public HoverResult(Item item) { Item = item; NPC = null; ItemId = null; }
        public HoverResult(NPC npc) { Item = null; NPC = npc; ItemId = null; }
        public HoverResult(string id) { Item = null; NPC = null; ItemId = id; }

        public static HoverResult None => new();
    }

    internal class HoveredItem
    {
        public static HoverResult Get()
        {
            // 1. Item hover
            var item = GetItemFromMenus();
            if (item != null)
                return new HoverResult(item);

            // 2. NPC hover (Social Page)
            var npc = GetNPCFromSocialPage();
            if (npc != null)
                return new HoverResult(npc);

            // 3. Collections Page
            var id = GetItemIdFromCollectionsPage();
            if (id != null)
                return new HoverResult(id);

            return HoverResult.None;
        
        }

        public static Item? GetItemFromMenus()
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
            return null;
        }
        private static NPC? GetNPCFromSocialPage()
        {
            if (Game1.activeClickableMenu is not GameMenu gm)
                return null;

            if (gm.currentTab != GameMenu.socialTab)
                return null;

            var pages = gm.pages;
            if (pages.Count <= GameMenu.socialTab)
                return null;

            if (pages[GameMenu.socialTab] is not SocialPage socialPage)
                return null;

            int mouseX = Game1.getMouseX();
            int mouseY = Game1.getMouseY();

            // Loop through visible slots
            for (int i = 0; i < socialPage.characterSlots.Count; i++)
            {
                var slot = socialPage.characterSlots[i];

                // Only consider visible rows
                if (i < socialPage.slotPosition || i >= socialPage.slotPosition + SocialPage.slotsOnPage)
                    continue;

                if (slot.bounds.Contains(mouseX, mouseY))
                {
                    var entry = socialPage.GetSocialEntry(i);

                    // Skip players and children (same as game logic)
                    if (entry.IsPlayer || entry.IsChild)
                        return null;

                    // entry.Character is the NPC instance
                    if (entry.Character is NPC npc)
                        return npc;

                    return null;
                }
            }

            return null;
        }

        private static string? GetItemIdFromCollectionsPage()
        {
            if (Game1.activeClickableMenu is not GameMenu gm)
                return null;

            if (gm.currentTab != GameMenu.collectionsTab)
                return null;

            var pages = gm.pages;
            if (pages.Count <= GameMenu.collectionsTab)
                return null;

            if (pages[GameMenu.collectionsTab] is not CollectionsPage cp)
                return null;

            var field = typeof(CollectionsPage).GetField("hoveredItemId",
                BindingFlags.Instance | BindingFlags.NonPublic);

            if (field == null)
                return null;

            return field.GetValue(cp) as string;
        }

        //////public static Item? GetFromMenus()
        //////{
        //////    IClickableMenu menu = Game1.activeClickableMenu;
        //////    if (menu == null)
        //////        return null;

        //////    switch (menu)
        //////    {
        //////        // Inventory (GameMenu → InventoryPage)
        //////        case GameMenu gm:
        //////            if (gm.currentTab != GameMenu.inventoryTab)
        //////                return null;

        //////            var invPage = gm.pages
        //////                .OfType<InventoryPage>()
        //////                .FirstOrDefault();

        //////            return invPage?.hoveredItem;

        //////        // Chests, Fridges, Dressers, Junimo Chests, etc.
        //////        case StardewValley.Menus.ItemGrabMenu chest:
        //////            return chest.hoveredItem;

        //////        // Pierre’s shop, Traveling Cart, Krobus, Qi, etc.
        //////        case StardewValley.Menus.ShopMenu shop:
        //////            return shop.hoveredItem as Item;

        //////    }



        //////    //if (menu.GetType().FullName == "StardewValley.Menus.CookMenu")
        //////    //{
        //////    //    var t = menu.GetType();

        //////    //    var prop = t.GetProperty("hoveredItem");
        //////    //    if (prop != null)
        //////    //        return prop.GetValue(menu) as Item;

        //////    //    var field = t.GetField("hoveredItem");
        //////    //    if (field != null)
        //////    //        return field.GetValue(menu) as Item;
        //////    //}

        //////    //// Better Crafting support (reflection)
        //////    //var type = menu.GetType();

        //////    //if (type.FullName == "BetterCrafting.BetterCraftingPage")
        //////    //{
        //////    //    // Get the root UI element
        //////    //    var rootField = type.GetField("root", BindingFlags.NonPublic | BindingFlags.Instance);
        //////    //    if (rootField == null)
        //////    //        return null;

        //////    //    var root = rootField.GetValue(menu);
        //////    //    if (root == null)
        //////    //        return null;

        //////    //    // root is a UIElement — find the element under the mouse
        //////    //    var method = root.GetType().GetMethod("GetElementAt", BindingFlags.Public | BindingFlags.Instance);
        //////    //    if (method == null)
        //////    //        return null;

        //////    //    var element = method.Invoke(root, new object[] { Game1.getMouseX(), Game1.getMouseY() });
        //////    //    if (element == null)
        //////    //        return null;

        //////    //    // Many BC elements expose an Item property
        //////    //    var itemProp = element.GetType().GetProperty("Item");
        //////    //    if (itemProp != null)
        //////    //        return itemProp.GetValue(element) as Item;

        //////    //    // Some expose HeldItem instead
        //////    //    var heldProp = element.GetType().GetProperty("HeldItem");
        //////    //    if (heldProp != null)
        //////    //        return heldProp.GetValue(element) as Item;

        //////    //}
        //////    return null;

        //////}



    }
}
