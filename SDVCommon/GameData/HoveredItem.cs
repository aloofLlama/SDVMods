using SDVCommon.Helpers;
using SDVCommon.Compatibility;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using System.Reflection;

namespace SDVCommon.GameData
{

    public readonly struct HoverResult
    {
        public Item? Item { get; init; }
        public NPC? NPC { get; init; }
        public HoverSource Source { get; init; }

        public bool HasValue => Item != null || NPC != null;

        private HoverResult(Item? item, NPC? npc, HoverSource source)
        {
            Item = item;
            NPC = npc;
            Source = source;
        }

        public static HoverResult FromItem(Item item, HoverSource source)
            => new HoverResult(item, null, source);

        public static HoverResult FromNPC(NPC npc, HoverSource source)
            => new HoverResult(null, npc, source);

        public static HoverResult None { get; } = new HoverResult(null, null, HoverSource.None);
    }

    internal class HoveredItem
    {
        public static HoverResult Get()
        {
            // Item hover (menus, inventory, chests, shops, etc.)
            var menuItem = GetItemFromMenus();
            if (menuItem != null)
                return HoverResult.FromItem(menuItem, HoverSource.Menu);

            // NPC hover (Social Page)
            var npc = GetNPCFromSocialPage();
            if (npc != null)
                return HoverResult.FromNPC(npc, HoverSource.SocialPage);

            // Collections Page
            var collItem = GetItemFromCollectionsPage();
            if (collItem != null)
                return HoverResult.FromItem(collItem, HoverSource.CollectionsPage);


            // BetterCrafting first (it replaces vanilla)
            var (bcItem, bcSource) = BetterCraftingCompat.GetItemFromBetterCraftingPage();
            if (bcItem != null)
                return HoverResult.FromItem(bcItem, bcSource);

            // Cooking Page
            var cookingItem = GetItemFromCookingPage();
            if (cookingItem != null)
                return HoverResult.FromItem(cookingItem, HoverSource.CookingPage);

            // Crafting Page
            var craftingItem = GetItemFromCraftingPage();
            if (craftingItem != null)
                return HoverResult.FromItem(craftingItem, HoverSource.CraftingPage);

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

        private static Item? GetItemFromCollectionsPage()
        {
            if (Game1.activeClickableMenu is not GameMenu gm)
                return null;

            if (gm.currentTab != GameMenu.collectionsTab)
                return null;

            if (gm.pages.Count <= GameMenu.collectionsTab)
                return null;

            if (gm.pages[GameMenu.collectionsTab] is not CollectionsPage cp)
                return null;

            int mouseX = Game1.getMouseX();
            int mouseY = Game1.getMouseY();

            foreach (var kvp in cp.collections)
            {
                foreach (var page in kvp.Value)
                {
                    foreach (var comp in page)
                    {
                        if (comp?.bounds.Contains(mouseX, mouseY) == true)
                        {
                            // Extract only the item ID
                            string raw = comp.name ?? "";
                            string id = raw.Split(new[] { ' ', '|' }, StringSplitOptions.RemoveEmptyEntries)[0];

                            return ItemRegistry.Create(id);
                        }
                    }
                }
            }

            return null;
        }

        private static Item? GetItemFromCookingPage()
        {
            if (Game1.activeClickableMenu is not CraftingPage cp)
                return null;

            if (!cp.cooking)
                return null;

            int mouseX = Game1.getMouseX();
            int mouseY = Game1.getMouseY();

            if (cp.pagesOfCraftingRecipes is List<Dictionary<ClickableTextureComponent, CraftingRecipe>> pages)
            {
                foreach (var dict in pages)
                {
                    foreach (var kvp in dict)
                    {
                        var comp = kvp.Key;
                        var recipe = kvp.Value;

                        if (comp.bounds.Contains(mouseX, mouseY))
                        {
                            return new CraftingRecipe(recipe.name, true).createItem();
                        }
                    }
                }
            }

            return null;
        }

        private static Item? GetItemFromCraftingPage()
        {
            if (Game1.activeClickableMenu is not CraftingPage cp)
                return null;

            if (cp.cooking)
                return null;

            int mouseX = Game1.getMouseX();
            int mouseY = Game1.getMouseY();

            if (cp.pagesOfCraftingRecipes is List<Dictionary<ClickableTextureComponent, CraftingRecipe>> pages)
            {
                foreach (var dict in pages)
                {
                    foreach (var kvp in dict)
                    {
                        var comp = kvp.Key;
                        var recipe = kvp.Value;

                        if (comp.bounds.Contains(mouseX, mouseY))
                            return recipe.createItem();
                    }
                }
            }

            return null;
        }


    }

    public enum HoverSource
{
    None,
    Menu,
    SocialPage,
    CollectionsPage,
    CookingPage,
    CraftingPage
}

}


