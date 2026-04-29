using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDVData
{
    public class HarvestInfoData
    {
        public string HarvestId { get; set; } = "";
        public string? SeedId { get; set; } = "";
        public ItemInfo? Harvest { get; set; } = new ItemInfo();

        // Shipping Achievements
        public bool ShipOne { get; set; }
        public bool ShipPolyCulture { get; set; }
        public bool ShipMonoCulture { get; set; }

        // Future expansion: artisan goods, cooking, loved-by, etc.
        //public List<ArtisanProductInfo> ArtisanProducts { get; set; } = new();
        //public List<CookingRecipeInfo> CookingRecipes { get; set; } = new();
        //public List<string> LovedByNPCs { get; set; } = new();
    }
}
