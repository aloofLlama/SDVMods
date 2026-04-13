using SDVData;
using SDVData_Tests.Helpers;
using System.Numerics;

namespace SDVData_Tests
{
    public class SeedPriceTests_Modded
    {
        //Incldes Pierre, Sandy, Jumana, Marnie,  Festivals, fixed price seeds at Traveling Cart
        //Does NOT include: Joja, Night Market, Trade Vendors (incl. racoon seeds & Valley Fair)
        //Does NOT inlude: Vanilla Seeds, Crafted Seeds
        //Specifically: skellady.SBVCP_MoonberrySeeds (trade), "Cornucopia_CottonBollSeeds" (trade), Cornucopia_GinsengSeeds (trade)
        //Data created in TestContent
        private readonly IReadOnlyList<PlantInfoData> _plants;

        public SeedPriceTests_Modded()
        {
            _plants = TestPlantInfo.Load();
        }

        [Theory]
        [InlineData("slimerrain.uncleirohapprovedteacp_Chamomile_Seeds", "SeedShop", 50)] //Chamomile_Seeds
        [InlineData("slimerrain.uncleirohapprovedteacp_Chrysanthemum_Seeds", "SeedShop", 130)] //Chrysanthemum_Seeds *MANUALLY CORRECTED
        [InlineData("slimerrain.uncleirohapprovedteacp_Goji_Berry_Seeds", "SeedShop", 160)] //Berry_Seeds *MANUALLY CORRECTED
        [InlineData("slimerrain.uncleirohapprovedteacp_Lavender_Seeds", "SeedShop", 240)] //Lavender_Seeds *MANUALLY CORRECTED
        [InlineData("slimerrain.uncleirohapprovedteacp_Lily_Bulb", "SeedShop", 120)] //Lily_Bulb
        [InlineData("slimerrain.uncleirohapprovedteacp_Lotus_Seeds", "SeedShop", 180)] //Lotus_Seeds *MANUALLY CORRECTED
        [InlineData("slimerrain.uncleirohapprovedteacp_Sugarcane_Seeds", "SeedShop", 350)] //Sugarcane_Seeds *MANUALLY CORRECTED
        [InlineData("slimerrain.uncleirohapprovedteacp_Cassia_Sapling", "Sandy", 2400)] //Cassia_Sapling
        //2 Bushes not yet supported
        public void UncleIrohTea_Pierre_ModdedSeedPrices(string seedId, string vendorId, int expectedPrice)
        {
            // Get all plants with this seedId (some appear twice in your snapshot)
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
        [InlineData("slimerrain.grainsoverhullcp_Barley_Seeds", "SeedShop", 10)] //Barley_Seeds *MANUALLY CORRECTED
        [InlineData("slimerrain.grainsoverhullcp_Buckwheat_Seeds", "SeedShop", 200)] //Buckwheat_Seeds *MANUALLY CORRECTED
        [InlineData("slimerrain.grainsoverhullcp_Millet_Seeds", "SeedShop", 5)] //Millet_Seeds
        [InlineData("slimerrain.grainsoverhullcp_Oat_Seeds", "SeedShop", 16)] //Oat_Seeds *MANUALLY CORRECTED
        [InlineData("slimerrain.grainsoverhullcp_Rye_Seeds", "SeedShop", 20)] //Rye_Seeds *MANUALLY CORRECTED
        [InlineData("slimerrain.grainsoverhullcp_Sorghum_Seeds", "SeedShop", 10)] //Sorghum_Seeds *MANUALLY CORRECTED
        public void GrainsOverhull_Pierre_ModdedSeedPrices(string seedId, string vendorId, int expectedPrice)
        {
            // Get all plants with this seedId (some appear twice in your snapshot)
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
        [InlineData("Cornucopia_BasilSeeds", "SeedShop", 20)] //BasilSeeds
        [InlineData("Cornucopia_BellPepperSeeds", "SeedShop", 130)] //BellPepperSeeds
        [InlineData("Cornucopia_TurnipSeeds", "SeedShop", 30)] //TurnipSeeds
        [InlineData("Cornucopia_CottonBollSeeds", "SeedShop", 200)] //CottonBollSeeds
        [InlineData("Cornucopia_CucumberSeeds", "SeedShop", 100)] //CucumberSeeds
        [InlineData("Cornucopia_LettuceSeeds", "SeedShop", 20)] //LettuceSeeds
        [InlineData("Cornucopia_OnionSeeds", "SeedShop", 80)] //OnionSeeds
        [InlineData("Cornucopia_PeanutSeeds", "SeedShop", 60)] //PeanutSeeds
        [InlineData("Cornucopia_SpinachSeeds", "SeedShop", 10)] //SpinachSeeds
        [InlineData("Cornucopia_WatermelonSeeds", "SeedShop", 300)] //WatermelonSeeds
        [InlineData("Cornucopia_ZucchiniSeeds", "SeedShop", 50)] //ZucchiniSeeds
        [InlineData("Cornucopia_AdzukiBeanSeeds", "SeedShop", 100)] //AdzukiBeanSeeds
        [InlineData("Cornucopia_AsparagusSeeds", "SeedShop", 60)] //AsparagusSeeds
        [InlineData("Cornucopia_BarleySeeds", "SeedShop", 10)] //BarleySeeds
        [InlineData("Cornucopia_BlackBeansSeeds", "SeedShop", 60)] //BlackBeansSeeds
        [InlineData("Cornucopia_BuckwheatSeeds", "SeedShop", 10)] //BuckwheatSeeds
        [InlineData("Cornucopia_ButternutSquashSeeds", "SeedShop", 180)] //ButternutSquashSeeds
        [InlineData("Cornucopia_CabbageSeeds", "SeedShop", 70)] //CabbageSeeds
        [InlineData("Cornucopia_CanaryMelonSeeds", "SeedShop", 450)] //CanaryMelonSeeds
        [InlineData("Cornucopia_CanolaFlowerSeeds", "SeedShop", 100)] //CanolaFlowerSeeds
        [InlineData("Cornucopia_CantaloupeSeeds", "SeedShop", 80)] //CantaloupeSeeds
        [InlineData("Cornucopia_CassavaSeeds", "SeedShop", 50)] //CassavaSeeds
        [InlineData("Cornucopia_CelerySeeds", "SeedShop", 30)] //CelerySeeds
        [InlineData("Cornucopia_ChickpeaSeeds", "SeedShop", 200)] //ChickpeaSeeds
        [InlineData("Cornucopia_DaikonSeeds", "SeedShop", 140)] //DaikonSeeds
        [InlineData("Cornucopia_DurumSeeds", "SeedShop", 10)] //DurumSeeds
        [InlineData("Cornucopia_GreenPeasSeeds", "SeedShop", 50)] //GreenPeasSeeds
        [InlineData("Cornucopia_GroundcherriesSeeds", "SeedShop", 150)] //GroundcherriesSeeds
        [InlineData("Cornucopia_HabaneroSeeds", "SeedShop", 130)] //HabaneroSeeds
        [InlineData("Cornucopia_HoneydewSeeds", "SeedShop", 80)] //HoneydewSeeds
        [InlineData("Cornucopia_JalapenoSeeds", "SeedShop", 20)] //JalapenoSeeds
        [InlineData("Cornucopia_KidneyBeansSeeds", "SeedShop", 180)] //KidneyBeansSeeds
        [InlineData("Cornucopia_NavyBeansSeeds", "SeedShop", 80)] //NavyBeansSeeds
        [InlineData("Cornucopia_OatsSeeds", "SeedShop", 10)] //OatsSeeds
        [InlineData("Cornucopia_OkraSeeds", "SeedShop", 160)] //OkraSeeds
        [InlineData("Cornucopia_PintoBeansSeeds", "SeedShop", 80)] //PintoBeansSeeds
        [InlineData("Cornucopia_QuinoaSeeds", "SeedShop", 20)] //QuinoaSeeds
        [InlineData("Cornucopia_RedOnionSeeds", "SeedShop", 80)] //RedOnionSeeds
        [InlineData("Cornucopia_ShallotSeeds", "SeedShop", 80)] //ShallotSeeds
        [InlineData("Cornucopia_SugarBeetSeeds", "SeedShop", 100)] //SugarBeetSeeds
        [InlineData("Cornucopia_SweetPotatoSeeds", "SeedShop", 70)] //SweetPotatoSeeds
        [InlineData("Cornucopia_WasabiRootSeeds", "SeedShop", 100)] //WasabiRootSeeds
        [InlineData("Cornucopia_ChivesSeeds", "SeedShop", 20)] //ChivesSeeds
        [InlineData("Cornucopia_CilantroSeeds", "SeedShop", 20)] //CilantroSeeds
        [InlineData("Cornucopia_DillSeeds", "SeedShop", 50)] //DillSeeds
        [InlineData("Cornucopia_FennelSeeds", "SeedShop", 20)] //FennelSeeds
        [InlineData("Cornucopia_FenugreekSeeds", "SeedShop", 20)] //FenugreekSeeds
        [InlineData("Cornucopia_MarjoramSeeds", "SeedShop", 30)] //MarjoramSeeds
        [InlineData("Cornucopia_MintSeeds", "SeedShop", 30)] //MintSeeds
        [InlineData("Cornucopia_OreganoSeeds", "SeedShop", 30)] //OreganoSeeds
        [InlineData("Cornucopia_ParsleySeeds", "SeedShop", 20)] //ParsleySeeds
        [InlineData("Cornucopia_PerillaLeavesSeeds", "SeedShop", 30)] //PerillaLeavesSeeds
        [InlineData("Cornucopia_RosemarySeeds", "SeedShop", 30)] //RosemarySeeds
        [InlineData("Cornucopia_SageSeeds", "SeedShop", 40)] //SageSeeds
        [InlineData("Cornucopia_TarragonSeeds", "SeedShop", 30)] //TarragonSeeds
        [InlineData("Cornucopia_ThymeSeeds", "SeedShop", 30)] //ThymeSeeds
        [InlineData("Cornucopia_MustardSeeds", "SeedShop", 40)] //MustardSeeds
        [InlineData("Cornucopia_BlueMistSeeds", "SeedShop", 20)] //BlueMistSeeds
        [InlineData("Cornucopia_ChrysanthemumSeeds", "SeedShop", 40)] //ChrysanthemumSeeds
        [InlineData("Cornucopia_IrisSeeds", "SeedShop", 50)] //IrisSeeds
        [InlineData("Cornucopia_LilySeeds", "SeedShop", 50)] //LilySeeds
        [InlineData("Cornucopia_MorningGlorySeeds", "SeedShop", 240)] //MorningGlorySeeds
        [InlineData("Cornucopia_OrchidSeeds", "SeedShop", 200)] //OrchidSeeds
        [InlineData("Cornucopia_PansySeeds", "SeedShop", 30)] //PansySeeds
        [InlineData("Cornucopia_RoseSeeds", "SeedShop", 200)] //RoseSeeds
        [InlineData("Cornucopia_BluebonnetSeeds", "SeedShop", 20)] //BluebonnetSeeds
        [InlineData("Cornucopia_CarnationSeeds", "SeedShop", 40)] //CarnationSeeds
        [InlineData("Cornucopia_ChamomileSeeds", "SeedShop", 20)] //ChamomileSeeds
        [InlineData("Cornucopia_ClarySageSeeds", "SeedShop", 20)] //ClarySageSeeds
        [InlineData("Cornucopia_ClematisSeeds", "SeedShop", 90)] //ClematisSeeds
        [InlineData("Cornucopia_DahliaSeeds", "SeedShop", 30)] //DahliaSeeds
        [InlineData("Cornucopia_DaisySeeds", "SeedShop", 10)] //DaisySeeds
        [InlineData("Cornucopia_FreesiaSeeds", "SeedShop", 40)] //FreesiaSeeds
        [InlineData("Cornucopia_GeraniumSeeds", "SeedShop", 40)] //GeraniumSeeds
        [InlineData("Cornucopia_HyacinthSeeds", "SeedShop", 40)] //HyacinthSeeds
        [InlineData("Cornucopia_HydrangeaSeeds", "SeedShop", 40)] //HydrangeaSeeds
        [InlineData("Cornucopia_LarkspurSeeds", "SeedShop", 50)] //LarkspurSeeds
        [InlineData("Cornucopia_LavenderSeeds", "SeedShop", 20)] //LavenderSeeds
        [InlineData("Cornucopia_LupineSeeds", "SeedShop", 30)] //LupineSeeds
        [InlineData("Cornucopia_PeonySeeds", "SeedShop", 50)] //PeonySeeds
        [InlineData("Cornucopia_PetuniaSeeds", "SeedShop", 30)] //PetuniaSeeds
        [InlineData("Cornucopia_PurpleConeflowerSeeds", "SeedShop", 30)] //PurpleConeflowerSeeds
        [InlineData("Cornucopia_RoseSpringSeeds", "SeedShop", 200)] //RoseSpringSeeds
        [InlineData("Cornucopia_RoseSummerSeeds", "SeedShop", 200)] //RoseSummerSeeds
        [InlineData("Cornucopia_RoseFallSeeds", "SeedShop", 200)] //RoseFallSeeds
        [InlineData("Cornucopia_RoseWinterSeeds", "SeedShop", 200)] //RoseWinterSeeds
        [InlineData("Cornucopia_VioletSeeds", "SeedShop", 20)] //VioletSeeds
        [InlineData("Cornucopia_AvocadoSapling", "SeedShop", 4000)] //AvocadoSapling
        [InlineData("Cornucopia_CocoaPodSapling", "SeedShop", 2500)] //CocoaPodSapling
        [InlineData("Cornucopia_PearSapling", "SeedShop", 2750)] //PearSapling
        [InlineData("Cornucopia_PistachioSapling", "SeedShop", 1874)] //PistachioSapling
        [InlineData("Cornucopia_AlmondSapling", "SeedShop", 1874)] //AlmondSapling
        [InlineData("Cornucopia_CashewSapling", "SeedShop", 2000)] //CashewSapling
        [InlineData("Cornucopia_FigSapling", "SeedShop", 2000)] //FigSapling
        [InlineData("Cornucopia_GrapefruitSapling", "SeedShop", 3000)] //GrapefruitSapling
        [InlineData("Cornucopia_LemonSapling", "SeedShop", 1000)] //LemonSapling
        [InlineData("Cornucopia_LimeSapling", "SeedShop", 1000)] //LimeSapling
        [InlineData("Cornucopia_NectarineSapling", "SeedShop", 3000)] //NectarineSapling
        [InlineData("Cornucopia_PecanSapling", "SeedShop", 2500)] //PecanSapling
        [InlineData("Cornucopia_PersimmonSapling", "SeedShop", 3500)] //PersimmonSapling
        [InlineData("Cornucopia_PomeloSapling", "SeedShop", 1700)] //PomeloSapling
        [InlineData("Cornucopia_UmeSapling", "SeedShop", 1000)] //UmeSapling
        [InlineData("Cornucopia_WalnutSapling", "SeedShop", 1874)] //WalnutSapling
        [InlineData("Cornucopia_YuzuSapling", "SeedShop", 1000)] //YuzuSapling
        [InlineData("Cornucopia_CamphorLeavesSapling", "SeedShop", 2000)] //CamphorLeavesSapling
        [InlineData("Cornucopia_CinnamonSticksSapling", "SeedShop", 3000)] //CinnamonSticksSapling
        [InlineData("Cornucopia_EucalyptusLeavesSapling", "SeedShop", 2750)] //EucalyptusLeavesSapling
        [InlineData("Cornucopia_NutmegSapling", "SeedShop", 2000)] //NutmegSapling
        [InlineData("Cornucopia_JasmineSapling", "SeedShop", 1500)] //JasmineSapling
        [InlineData("Cornucopia_MagnoliaSapling", "SeedShop", 1250)] //MagnoliaSapling
        [InlineData("Cornucopia_WisteriaSapling", "SeedShop", 1250)] //WisteriaSapling

        public void Cornucopia_Pierre_ModdedSeedPrices(string seedId, string vendorId, int expectedPrice)
        {
            // Get all plants with this seedId (some appear twice in your snapshot)
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
        [InlineData("skellady.SBVCP_SunberrySeeds", "skellady.SBVCP_AriMarket", 110)] //SunberrySeeds
        [InlineData("skellady.SBVCP_CarobSapling", "skellady.SBVCP_AriMarket", 5000)] //CarobSapling
        public void Sunberry_Ari_ModdedSeedPrices(string seedId, string vendorId, int expectedPrice)
        {
            // Get all plants with this seedId (some appear twice in your snapshot)
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
        [InlineData("skellady.SBVCP_ArfajSeeds", "skellady.SBVCP_JumanaShop", 50)] //ArfajSeeds
        [InlineData("skellady.SBVCP_CallaLilySeeds", "skellady.SBVCP_JumanaShop", 20)] //CallaLilySeeds
        [InlineData("skellady.SBVCP_CoffeeBlossomSeeds", "skellady.SBVCP_JumanaShop", 20)] //CoffeeBlossomSeeds
        [InlineData("skellady.SBVCP_CyclamenSeeds", "skellady.SBVCP_JumanaShop", 30)] //CyclamenSeeds
        [InlineData("skellady.SBVCP_HyacinthSeeds", "skellady.SBVCP_JumanaShop", 30)] //HyacinthSeeds
        [InlineData("skellady.SBVCP_RoyalAnemoneSeeds", "skellady.SBVCP_JumanaShop", 40)] //RoyalAnemoneSeeds
        [InlineData("skellady.SBVCP_SpeedwellSeeds", "skellady.SBVCP_JumanaShop", 30)] //SpeedwellSeeds **MANUALLY CORRECTED
        [InlineData("skellady.SBVCP_WildMustardSeeds", "skellady.SBVCP_JumanaShop", 40)] //WildMustardSeeds

        public void Sunberry_Jumana_ModdedSeedPrices(string seedId, string vendorId, int expectedPrice)
        {
            // Get all plants with this seedId (some appear twice in your snapshot)
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
        [InlineData("Cornucopia_SugarcaneSeeds", "Sandy", 20)] //SugarcaneSeeds
        [InlineData("Cornucopia_AgaveSeeds", "Sandy", 100)] //AgaveSeeds
        [InlineData("Cornucopia_BambooSeeds", "Sandy", 80)] //BambooSeeds
        [InlineData("Cornucopia_BlueAgaveSeeds", "Sandy", 180)] //BlueAgaveSeeds
        [InlineData("Cornucopia_AloeSeeds", "Sandy", 100)] //AloeSeeds
        [InlineData("Cornucopia_CuminSeeds", "Sandy", 80)] //CuminSeeds
        [InlineData("Cornucopia_LemongrassSeeds", "Sandy", 20)] //LemongrassSeeds
        [InlineData("Cornucopia_LotusSeeds", "Sandy", 60)] //LotusSeeds
        [InlineData("Cornucopia_FairyDusterSeeds", "Sandy", 20)] //FairyDusterSeeds
        [InlineData("Cornucopia_BreadfruitSapling", "Sandy", 8000)] //BreadfruitSapling
        [InlineData("Cornucopia_DragonFruitSapling", "Sandy", 5000)] //DragonFruitSapling
        [InlineData("Cornucopia_LycheeSapling", "Sandy", 5000)] //LycheeSapling
        [InlineData("Cornucopia_HibiscusSapling", "Sandy", 3000)] //HibiscusSapling
        [InlineData("Cornucopia_YlangYlangSapling", "Sandy", 3000)] //YlangYlangSapling
        public void Cornucopia_Sandy_ModdedSeedPrices(string seedId, string vendorId, int expectedPrice)
        {
            // Get all plants with this seedId (some appear twice in your snapshot)
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
        [InlineData("Cornucopia_CatnipSeeds", "AnimalShop", 60)] //Cornucopia_CatnipSeeds
        [InlineData("Cornucopia_PinkCatSeeds", "AnimalShop", 20)] //Cornucopia_PinkCatSeeds
        [InlineData("Cornucopia_BeeBalmSeeds", "AnimalShop", 20)] //Cornucopia_BeeBalmSeeds
        [InlineData("Cornucopia_ButtercupSeeds", "AnimalShop", 20)] //Cornucopia_ButtercupSeeds
        [InlineData("Cornucopia_HoneysuckleSeeds", "AnimalShop", 240)] //Cornucopia_HoneysuckleSeeds
        [InlineData("Cornucopia_WolfsbaneSeeds", "AnimalShop", 50)] //Cornucopia_WolfsbaneSeeds
        [InlineData("Cornucopia_MelaleucaLeavesSapling", "AnimalShop", 4000)] //Cornucopia_MelaleucaLeavesSapling
        public void Cornucopia_Marnie_ModdedSeedPrices(string seedId, string vendorId, int expectedPrice)
        {
            // Get all plants with this seedId (some appear twice in your snapshot)
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
        [InlineData("Cornucopia_KiwiSeeds", "Festival_Luau_Pierre", 100)] //KiwiSeeds
        [InlineData("Cornucopia_WatermelonSeeds", "Festival_Luau_Pierre", 300)] //WatermelonSeeds
        [InlineData("Cornucopia_GinsengSeeds", "Traveler", 2200)] //GinsengSeeds
        [InlineData("Cornucopia_WasabiRootSeeds", "Festival_EggFestival_Pierre", 100)] //WasabiRootSeeds
        [InlineData("Cornucopia_LicoriceRootSeeds", "Dwarf", 100)] //LicoriceRootSeeds
        [InlineData("Cornucopia_WormwoodSeeds", "Dwarf", 100)] //WormwoodSeeds
        [InlineData("Cornucopia_Soybeans", "Traveler", 195)] //Soybeans actual 195-1000 

        public void Misc_ModdedSeedPrices(string seedId, string vendorId, int expectedPrice)
        {
            // Get all plants with this seedId (some appear twice in your snapshot)
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
