using SDVData;
using SDVData_Tests.Helpers;

namespace SDVData_Tests
{
    public class SeedSaleConditionsTests
    {

        private readonly IReadOnlyList<PlantInfoData> _plants;

        public SeedSaleConditionsTests()
        {
            _plants = TestPlantInfo.Load();
        }

        [Theory]
        [InlineData("476")] //Garlic Seeds
        [InlineData("273")] //Rice Shoot
        [InlineData("485")] //Red Cabbage Seeds
        [InlineData("489")] //Artichoke Seeds
        [InlineData("Cornucopia_CottonBollSeeds")] //CottonBollSeeds
        [InlineData("Cornucopia_CucumberSeeds")] //CucumberSeeds
        [InlineData("Cornucopia_WatermelonSeeds")] //WatermelonSeeds
        [InlineData("Cornucopia_ZucchiniSeeds")] //ZucchiniSeeds
        [InlineData("Cornucopia_AsparagusSeeds")] //AsparagusSeeds
        [InlineData("Cornucopia_BarleySeeds")] //BarleySeeds
        [InlineData("Cornucopia_BlackBeansSeeds")] //BlackBeansSeeds
        [InlineData("Cornucopia_ButternutSquashSeeds")] //ButternutSquashSeeds
        [InlineData("Cornucopia_CanaryMelonSeeds")] //CanaryMelonSeeds
        [InlineData("Cornucopia_CantaloupeSeeds")] //CantaloupeSeeds
        [InlineData("Cornucopia_CassavaSeeds")] //CassavaSeeds
        [InlineData("Cornucopia_CelerySeeds")] //CelerySeeds
        [InlineData("Cornucopia_ChickpeaSeeds")] //ChickpeaSeeds
        [InlineData("Cornucopia_DurumSeeds")] //DurumSeeds
        [InlineData("Cornucopia_HabaneroSeeds")] //HabaneroSeeds
        [InlineData("Cornucopia_HoneydewSeeds")] //HoneydewSeeds
        [InlineData("Cornucopia_JalapenoSeeds")] //JalapenoSeeds
        [InlineData("Cornucopia_KidneyBeansSeeds")] //KidneyBeansSeeds
        [InlineData("Cornucopia_OkraSeeds")] //OkraSeeds
        [InlineData("Cornucopia_PintoBeansSeeds")] //PintoBeansSeeds
        [InlineData("Cornucopia_QuinoaSeeds")] //QuinoaSeeds
        [InlineData("Cornucopia_RedOnionSeeds")] //RedOnionSeeds
        [InlineData("Cornucopia_SugarBeetSeeds")] //SugarBeetSeeds
        [InlineData("Cornucopia_WasabiRootSeeds")] //WasabiRootSeeds
        [InlineData("Cornucopia_ChivesSeeds")] //ChivesSeeds
        [InlineData("Cornucopia_CilantroSeeds")] //CilantroSeeds
        [InlineData("Cornucopia_FennelSeeds")] //FennelSeeds
        [InlineData("Cornucopia_FenugreekSeeds")] //FenugreekSeeds
        [InlineData("Cornucopia_MarjoramSeeds")] //MarjoramSeeds
        [InlineData("Cornucopia_OreganoSeeds")] //OreganoSeeds
        [InlineData("Cornucopia_PerillaLeavesSeeds")] //PerillaLeavesSeeds
        [InlineData("Cornucopia_TarragonSeeds")] //TarragonSeeds
        [InlineData("Cornucopia_ThymeSeeds")] //ThymeSeeds
        [InlineData("Cornucopia_OrchidSeeds")] //OrchidSeeds
        [InlineData("Cornucopia_RoseSeeds")] //RoseSeeds
        [InlineData("Cornucopia_CarnationSeeds")] //CarnationSeeds
        [InlineData("Cornucopia_ClarySageSeeds")] //ClarySageSeeds
        [InlineData("Cornucopia_ClematisSeeds")] //ClematisSeeds
        [InlineData("Cornucopia_GeraniumSeeds")] //GeraniumSeeds
        [InlineData("Cornucopia_HyacinthSeeds")] //HyacinthSeeds
        [InlineData("Cornucopia_LarkspurSeeds")] //LarkspurSeeds
        [InlineData("Cornucopia_PurpleConeflowerSeeds")] //PurpleConeflowerSeeds
        [InlineData("Cornucopia_RoseSpringSeeds")] //RoseSpringSeeds
        [InlineData("Cornucopia_RoseSummerSeeds")] //RoseSummerSeeds
        [InlineData("Cornucopia_RoseFallSeeds")] //RoseFallSeeds
        [InlineData("Cornucopia_RoseWinterSeeds")] //RoseWinterSeeds
        public void AllVendors_AreYear2Plus(string seedId)
        {
            // Find all plants with this seedId
            var matchingPlants = _plants.Where(p => p.SeedId == seedId).ToList();
            Assert.NotEmpty(matchingPlants);

            // Collect ALL regular shopvendor entries across all duplicates
            var vendors = matchingPlants
                    .SelectMany(p => p.PurchaseOptions)
                    .Where(TestHelpers.IsRegularShopVendor)
                    .ToList();

            Assert.NotEmpty(vendors);

            var vendorInfo = vendors
                .Select(v => new
                {
                    VendorId = v.VendorId,
                    Year = TestHelpers.GetMinYearForVendor(v)
                })
                .ToList();

            Assert.All(vendorInfo, info =>
                    Assert.True(
                        info.Year >= 2,
                        $"Seed {seedId} has vendor {info.VendorId} selling in YEAR {info.Year}, expected YEAR >= 2"
                    )
                );
        }



    }
}
