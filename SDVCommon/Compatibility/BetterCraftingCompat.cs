using SDVCommon.GameData;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Text;

namespace SDVCommon.Compatibility
{
    public static class BetterCraftingCompat
    {
        public static IBetterCrafting? Api { get; internal set; }

        public static (Item? item, HoverSource source) GetItemFromBetterCraftingPage()
        {
            var api = BetterCraftingCompat.Api;
            if (api == null)
                return (null, HoverSource.None);

            var menu = api.GetActiveMenu();
            if (menu == null)
                return (null, HoverSource.None);

            var recipe = menu.ActiveRecipe;
            if (recipe == null)
                return (null, HoverSource.None);

            Item? item = recipe.CreateItem(); 

            HoverSource source = menu.Cooking
                ? HoverSource.CookingPage
                : HoverSource.CraftingPage;

            return (item, source);
        }

    }
}
