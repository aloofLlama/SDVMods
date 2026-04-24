using SDVData;
using SDVData_Tests.Helpers;

namespace  SDVData_Tests
{
    public class SeedPriceTests_Vanilla
    {
        //Incldes Pierre, Sandy, Jumana, Egg Festival, fixed price seeds at Traveling Cart
        //Does NOT include: Joja, Night Market, Trade Vendors (incl. racoon seeds & Valley Fair)
        //Does NOT inlude: Modded Seeds, Crafted Seeds
        private readonly IReadOnlyList<PlantInfoData> _plants;

        public SeedPriceTests_Vanilla()
        {
            _plants = TestPlantInfo.Load();
        }

        [Theory]
        [InlineData("472", "SeedShop", 20)]  // Parsnip Seeds
        [InlineData("473", "SeedShop", 60)]  // Bean Starter
        [InlineData("474", "SeedShop", 80)]  // Cauliflower Seeds
        [InlineData("475", "SeedShop", 50)]  // Potato Seeds
        [InlineData("476", "SeedShop", 40)]  // Garlic Seeds
        [InlineData("273", "SeedShop", 40)]  // Rice Shoot
        [InlineData("477", "SeedShop", 70)]  // Kale Seeds
        [InlineData("479", "SeedShop", 80)]  // Melon Seeds
        [InlineData("480", "SeedShop", 50)]  // Tomato Seeds
        [InlineData("481", "SeedShop", 80)]  // Blueberry Seeds
        [InlineData("482", "SeedShop", 40)]  // Pepper Seeds
        [InlineData("483", "SeedShop", 10)]  // Wheat Seeds
        [InlineData("484", "SeedShop", 40)]  // Radish Seeds
        [InlineData("485", "SeedShop", 100)] // Red Cabbage Seeds
        [InlineData("487", "SeedShop", 150)] // Corn Seeds
        [InlineData("302", "SeedShop", 60)]  // Hops Starter
        [InlineData("299", "SeedShop", 70)]  // Amaranth Seeds
        [InlineData("488", "SeedShop", 20)]  // Eggplant Seeds
        [InlineData("489", "SeedShop", 30)]  // Artichoke Seeds
        [InlineData("490", "SeedShop", 100)] // Pumpkin Seeds
        [InlineData("491", "SeedShop", 50)]  // Bok Choy Seeds
        [InlineData("492", "SeedShop", 60)]  // Yam Seeds
        [InlineData("493", "SeedShop", 240)] // Cranberry Seeds
        [InlineData("427", "SeedShop", 20)]  // Tulip Bulb
        [InlineData("429", "SeedShop", 30)]  // Jazz Seeds
        [InlineData("453", "SeedShop", 100)] // Poppy Seeds
        [InlineData("455", "SeedShop", 50)]  // Spangle Seeds
        [InlineData("431", "SeedShop", 200)] // Sunflower Seeds *Manually corrected from 100
        [InlineData("425", "SeedShop", 200)] // Fairy Seeds
        [InlineData("628", "SeedShop", 1700)] // Cherry Sapling
        [InlineData("629", "SeedShop", 1000)] // Apricot Sapling
        [InlineData("630", "SeedShop", 2000)] // Orange Sapling
        [InlineData("631", "SeedShop", 3000)] // Peach Sapling
        [InlineData("632", "SeedShop", 3000)] // Pomegranate Sapling
        [InlineData("633", "SeedShop", 2000)] // Apple Sapling
        //Grapes disabled by other mods
        public void Pierre_VanillaSeedPrices(string seedId, string vendorId, int expectedPrice)
        {
            // Get all plants with this seedId 
            var matchingPlants = _plants.Where(p => p.SeedId == seedId).ToList();

            Assert.NotEmpty(matchingPlants); 

            // Collect ALL matching vendor entries across all duplicates
            var options = matchingPlants
                .SelectMany(p => p.PurchaseOptions)
                .Where(o => o.VendorId == vendorId)
                .ToList();

            Assert.NotEmpty(options); // vendor must exist

            // Extract all prices
            var prices = options
                .Select(o => o.GoldPrice)
                .Distinct()
                .ToList();

            // If multiple prices exist → fail
            Assert.True(prices.Count == 1,
                $"Seed {seedId} at {vendorId} has inconsistent prices: {string.Join(", ", prices)}");

            Assert.Equal(expectedPrice, prices.Single());
        }



        [Theory]
        [InlineData("472", "skellady.SBVCP_AriMarket", 15)] //Parsnip Seeds
        [InlineData("473", "skellady.SBVCP_AriMarket", 55)] //Bean Starter
        [InlineData("474", "skellady.SBVCP_AriMarket", 75)] //Cauliflower Seeds
        [InlineData("475", "skellady.SBVCP_AriMarket", 45)] //Potato Seeds
        [InlineData("476", "skellady.SBVCP_AriMarket", 35)] //Garlic Seeds
        [InlineData("477", "skellady.SBVCP_AriMarket", 65)] //Kale Seeds
        [InlineData("479", "skellady.SBVCP_AriMarket", 75)] //Melon Seeds
        [InlineData("480", "skellady.SBVCP_AriMarket", 45)] //Tomato Seeds
        [InlineData("481", "skellady.SBVCP_AriMarket", 75)] //Blueberry Seeds
        [InlineData("482", "skellady.SBVCP_AriMarket", 35)] //Pepper Seeds
        [InlineData("483", "skellady.SBVCP_AriMarket", 5)] //Wheat Seeds
        [InlineData("484", "skellady.SBVCP_AriMarket", 35)] //Radish Seeds
        [InlineData("485", "skellady.SBVCP_AriMarket", 95)] //Red Cabbage Seeds
        [InlineData("487", "skellady.SBVCP_AriMarket", 140)] //Corn Seeds
        [InlineData("302", "skellady.SBVCP_AriMarket", 55)] //Hops Starter
        [InlineData("299", "skellady.SBVCP_AriMarket", 65)] //Amaranth Seeds
        [InlineData("488", "skellady.SBVCP_AriMarket", 15)] //Eggplant Seeds
        [InlineData("489", "skellady.SBVCP_AriMarket", 25)] //Artichoke Seeds
        [InlineData("490", "skellady.SBVCP_AriMarket", 95)] //Pumpkin Seeds
        [InlineData("491", "skellady.SBVCP_AriMarket", 45)] //Bok Choy Seeds
        [InlineData("492", "skellady.SBVCP_AriMarket", 55)] //Yam Seeds
        [InlineData("493", "skellady.SBVCP_AriMarket", 230)] //Cranberry Seeds
        [InlineData("494", "skellady.SBVCP_AriMarket", 15)] //Beet Seeds
        [InlineData("495", "skellady.SBVCP_AriMarket", 50)] //Spring Seeds
        [InlineData("496", "skellady.SBVCP_AriMarket", 50)] //Summer Seeds
        [InlineData("497", "skellady.SBVCP_AriMarket", 50)] //Fall Seeds
        [InlineData("498", "skellady.SBVCP_AriMarket", 50)] //Winter Seeds
        public void Ari_VanillaSeedPrices(string seedId, string vendorId, int expectedPrice)
        {
            // Get all plants with this seedId 
            var matchingPlants = _plants.Where(p => p.SeedId == seedId).ToList();

            Assert.NotEmpty(matchingPlants);

            // Collect ALL matching vendor entries across all duplicates
            var options = matchingPlants
                .SelectMany(p => p.PurchaseOptions)
                .Where(o => o.VendorId == vendorId)
                .ToList();

            Assert.NotEmpty(options); // vendor must exist

            // Extract all prices
            var prices = options
                .Select(o => o.GoldPrice)
                .Distinct()
                .ToList();

            // If multiple prices exist → fail
            Assert.True(prices.Count == 1,
                $"Seed {seedId} at {vendorId} has inconsistent prices: {string.Join(", ", prices)}");

            Assert.Equal(expectedPrice, prices.Single());
        }


        [Theory]
        [InlineData("478", "Sandy", 100)] //Rhubarb Seeds
        [InlineData("486", "Sandy", 400)] //Starfruit Seeds
        [InlineData("802", "Sandy", 150)] //Cactus Seeds
        [InlineData("494", "Sandy", 20)] //Beet Seeds
        public void Sandy_VanillaSeedPrices(string seedId, string vendorId, int expectedPrice)
        {
            // Get all plants with this seedId 
            var matchingPlants = _plants.Where(p => p.SeedId == seedId).ToList();

            Assert.NotEmpty(matchingPlants);

            // Collect ALL matching vendor entries across all duplicates
            var options = matchingPlants
                .SelectMany(p => p.PurchaseOptions)
                .Where(o => o.VendorId == vendorId)
                .ToList();

            Assert.NotEmpty(options); // vendor must exist

            // Extract all prices
            var prices = options
                .Select(o => o.GoldPrice)
                .Distinct()
                .ToList();

            // If multiple prices exist → fail
            Assert.True(prices.Count == 1,
                $"Seed {seedId} at {vendorId} has inconsistent prices: {string.Join(", ", prices)}");

            Assert.Equal(expectedPrice, prices.Single());
        }


        [Theory]
        [InlineData("427", "skellady.SBVCP_JumanaShop", 20)] //Tulip Bulb
        [InlineData("429", "skellady.SBVCP_JumanaShop", 30)] //Jazz Seeds
        [InlineData("453", "skellady.SBVCP_JumanaShop", 100)] //Poppy Seeds
        [InlineData("455", "skellady.SBVCP_JumanaShop", 50)] //Spangle Seeds
        [InlineData("431", "skellady.SBVCP_JumanaShop", 120)] //Sunflower Seeds
        [InlineData("425", "skellady.SBVCP_JumanaShop", 200)] //Fairy Seeds
        public void Jumana_VanillaSeedPrices(string seedId, string vendorId, int expectedPrice)
        {
            // Get all plants with this seedId 
            var matchingPlants = _plants.Where(p => p.SeedId == seedId).ToList();

            Assert.NotEmpty(matchingPlants);

            // Collect ALL matching vendor entries across all duplicates
            var options = matchingPlants
                .SelectMany(p => p.PurchaseOptions)
                .Where(o => o.VendorId == vendorId)
                .ToList();

            Assert.NotEmpty(options); // vendor must exist

            // Extract all prices
            var prices = options
                .Select(o => o.GoldPrice)
                .Distinct()
                .ToList();

            // If multiple prices exist → fail
            Assert.True(prices.Count == 1,
                $"Seed {seedId} at {vendorId} has inconsistent prices: {string.Join(", ", prices)}");

            Assert.Equal(expectedPrice, prices.Single());
        }


        [Theory]
        [InlineData("745", "Festival_EggFestival_Pierre", 100)] //Strawberry Seeds
        //[InlineData("485", "Traveler", 0)] //Red Cabbage Seeds *N/A - only listed separately for game special consideration
        [InlineData("433", "Traveler", 2500)] //Coffee Bean
        [InlineData("347", "Traveler", 1000)] //Rare Seed
        public void Misc_VanillaSeedPrices(string seedId, string vendorId, int expectedPrice)
        {
            // Get all plants with this seedId 
            var matchingPlants = _plants.Where(p => p.SeedId == seedId).ToList();

            Assert.NotEmpty(matchingPlants);

            // Collect ALL matching vendor entries across all duplicates
            var options = matchingPlants
                .SelectMany(p => p.PurchaseOptions)
                .Where(o => o.VendorId == vendorId)
                .ToList();

            Assert.NotEmpty(options); // vendor must exist

            // Extract all prices
            var prices = options
                .Select(o => o.GoldPrice)
                .Distinct()
                .ToList();

            // If multiple prices exist → fail
            Assert.True(prices.Count == 1,
                $"Seed {seedId} at {vendorId} has inconsistent prices: {string.Join(", ", prices)}");

            Assert.Equal(expectedPrice, prices.Single());
        }

    }

}
