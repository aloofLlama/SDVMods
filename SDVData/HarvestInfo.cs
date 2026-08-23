

namespace SDVData
{
    public class HarvestInfo
    {
        public string HarvestQId { get; set; } = "";
        public string DisplayName { get; set; } = "";
        public string? SeedQId { get; set; } = "";
        public int Category { get; set; } //e.g. seeds, vegetable,fruit

        //public ObjectInfo? Harvest { get; set; } = new ObjectInfo();
        public string ModSource { get; set; } = ""; 


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
