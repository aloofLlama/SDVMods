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
        public int Price { get; set; }
        public ItemInfo? Harvest { get; set; } = new ItemInfo();



        // Future expansion: artisan goods, cooking, loved-by, etc.
        //public List<ArtisanProductInfo> ArtisanProducts { get; set; } = new();
        //public List<CookingRecipeInfo> CookingRecipes { get; set; } = new();
        //public List<string> LovedByNPCs { get; set; } = new();
    }
}
