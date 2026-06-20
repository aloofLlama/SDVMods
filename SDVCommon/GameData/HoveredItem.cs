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

            // Crafting Page - workbench and game menu tab
            var craftingItem = GetItemFromCraftingPage();
            if (craftingItem != null)
                return HoverResult.FromItem(craftingItem, HoverSource.CraftingPage);

            // Cooking Page
            var cookingItem = GetItemFromCookingPage();
            if (cookingItem != null)
                return HoverResult.FromItem(cookingItem, HoverSource.CookingPage);


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

            if (gm.pages[GameMenu.collectionsTab] is not CollectionsPage cp)
                return null;

            int tab = cp.currentTab;
            int page = cp.currentPage;

            // Ensure tab exists
            if (!cp.collections.TryGetValue(tab, out var tabPages))
                return null;

            // Skip Achievements tab entirely
            // Collection pages return numberic ID, achievements also have numeric ID and will pop for the objects of the same number
            if (cp.currentTab == CollectionsPage.achievementsTab)
                return null;

            // Ensure page exists
            if (page < 0 || page >= tabPages.Count)
                return null;

            var visiblePage = tabPages[page];

            int mouseX = Game1.getMouseX();
            int mouseY = Game1.getMouseY();

            foreach (var comp in visiblePage)
            {
                if (comp?.bounds.Contains(mouseX, mouseY) == true)
                {
                    string raw = comp.name ?? "";

                    string id = raw.Split(new[] { ' ', '|' }, StringSplitOptions.RemoveEmptyEntries)[0];

                    return ItemRegistry.Create(id);
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

            int page = cp.currentCraftingPage;

            if (page < 0 || page >= cp.pagesOfCraftingRecipes.Count)
                return null;

            // This page is a Dictionary<ClickableTextureComponent, CraftingRecipe>
            var dict = cp.pagesOfCraftingRecipes[page];

            foreach (var kvp in dict)
            {
                var comp = kvp.Key;      // ClickableTextureComponent
                var recipe = kvp.Value;  // CraftingRecipe

                if (comp.bounds.Contains(mouseX, mouseY))
                {
                    // recipe.name is the internal recipe ID
                    var cooked = new CraftingRecipe(recipe.name, isCookingRecipe: true);
                    return cooked.createItem();
                }
            }

            return null;
        }

        private static Item? GetItemFromCraftingPage()
        {
            // Case 1: Standalone crafting menu (workbench, toolbar)
            if (Game1.activeClickableMenu is CraftingPage direct && !direct.cooking)
                return GetFromCraftingPage(direct);

            // Case 2: Crafting tab inside the ESC menu
            if (Game1.activeClickableMenu is GameMenu gm)
            {
                if (gm.currentTab == GameMenu.craftingTab &&
                    gm.pages[GameMenu.craftingTab] is CraftingPage cp &&
                    !cp.cooking)
                {
                    return GetFromCraftingPage(cp);
                }
            }

            return null;
        }

        private static Item? GetFromCraftingPage(CraftingPage cp)
        {

            int mouseX = Game1.getMouseX();
            int mouseY = Game1.getMouseY();

            int page = cp.currentCraftingPage;

            if (page < 0 || page >= cp.pagesOfCraftingRecipes.Count)
                return null;

            var dict = cp.pagesOfCraftingRecipes[page];

            foreach (var kvp in dict)
            {
                var comp = kvp.Key;      // ClickableTextureComponent
                var recipe = kvp.Value;  // CraftingRecipe

                if (comp.bounds.Contains(mouseX, mouseY))
                {
                    return recipe.createItem();
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


