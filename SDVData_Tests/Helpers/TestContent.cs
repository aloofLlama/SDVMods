using Newtonsoft.Json;
using SDVData;
using Xunit.Abstractions;
using System.IO;

namespace SDVData_Tests.Helpers
{
    public class TestPlantInfo
    {
        private readonly IReadOnlyList<PlantInfoData> _plants;
        private readonly ITestOutputHelper output;

        public TestPlantInfo(ITestOutputHelper output)
        {
            this.output = output;
            _plants = Load();
        }

        public static IReadOnlyList<PlantInfoData> Load()
        {
            string path = GetModCachePath();

            if (!File.Exists(path))
                throw new FileNotFoundException("PlantInfo.json not found", path);

            var json = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<List<PlantInfoData>>(json)!;
        }

        private static string GetModCachePath()
        {
            // Hardcoded mod folder path — stable and reliable
            string modPath =
                @"C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Mods\PlantingDay";

            return Path.Combine(modPath, "Cache", "PlantInfo.json");
        }
        /*
         * This test isn't really a test, but it generates the InlineData for the SeedPriceOverrides tests.
         * It also serves as a sanity check that the PlantInfo.json data is being read correctly.
         * 
         * To use:
         * 1. Run this test and copy the output.
         * 2. Paste the output into the SeedPriceOverrides test class, or inside the test method that needs it.
         */
        [Fact]
        public void GenerateInlinePriceDataForSeeds()
        {
            output.WriteLine($"Plant count: {_plants.Count}");
            var seen = new HashSet<string>();

            foreach (var plant in _plants)
            {

                string seedId = plant.SeedId;
                string seedName = TestHelpers.TrimSeedName(plant.Seed?.Name ?? "null");

                // CASE 1: plant has NO purchase options
                //if (plant.PurchaseOptions == null || plant.PurchaseOptions.Count == 0)
                //{

                //    if (seen.Add(seedId)) // only once per seed
                //    {
                //        output.WriteLine(
                //        $"[InlineData(\"{seedId}\", null, null)] // {seedName}"
                //        );

                //        //create for SeedPriceOverrides
                //        //output.WriteLine(
                //        //$"{{ (\"{seedId}\", \"{null}\"), {null} }}, // {seedName}"
                //        //);

                //    }

                //    continue;
                //}

                // CASE 2: plant HAS purchase options
                foreach (var option in plant.PurchaseOptions)
                {
                    string vendorId = option.VendorId;
                    string price = option.GoldPrice?.ToString() ?? "null";

                    if (//vendorId != "Joja" &&
                        //!TestHelpers.IsNightMarket(vendorId) &&
                        //vendorId != "IslandTrade" &&
                        //vendorId != "Raccoon" &&
                        vendorId == "SeedShop"
                        //vendorId != "skellady.SBVCP_AriMarket" &&
                        //vendorId != "Sandy" &&
                        //vendorId != "skellady.SBVCP_JumanaShop" &&
                        //vendorId != "AnimalShop" &&
                        //vendorId == "Traveler" &&
                        //
                        //!seedId.Contains("slimerrain.uncleirohapprovedtea", StringComparison.OrdinalIgnoreCase) &&
                        //!seedId.Contains("slimerrain.grainsoverhullcp", StringComparison.OrdinalIgnoreCase) &&
                        //!seedId.Contains("skellady.SBVCP", StringComparison.OrdinalIgnoreCase) &&
                        //!seedId.Contains("Cornucopia", StringComparison.OrdinalIgnoreCase) &&
                        //
                        //!TestHelpers.IsVanillaSeedId(seedId)
                        //
                        )
                    {
                        string key = $"{seedId}:{vendorId}";

                        if (seen.Add(key)) // only true the FIRST time
                        {
                            //---
                            // SEED PRICE TESTS
                            //------
                            //output.WriteLine(
                            //$"[InlineData(\"{seedId}\", \"{vendorId}\", {price})] //{seedName}"
                            //);

                            //create for SeedPriceOverrides
                            //output.WriteLine(
                            //$"{{ (\"{seedId}\", \"{vendorId}\"), {price} }}, // {seedName}"
                            //);




                        }
                    }
                }
            }
        }

        [Fact]
        public void GenerateInlineConditionDataForSeeds()
        {
            var seen = new HashSet<string>();

            foreach (var plant in _plants)
            {
                string seedId = plant.SeedId;
                string seedName = TestHelpers.TrimSeedName(plant.Seed?.Name ?? "null");

                foreach (var option in plant.PurchaseOptions)
                {
                    string vendorId = option.VendorId;

                    bool allVendorEntriesYear2Plus =
                        plant.PurchaseOptions
                            .Where(o => o.VendorId == vendorId)
                            .All(o => TestHelpers.GetMinYearForVendor(o) >= 2);

                    if (vendorId == "SeedShop" && allVendorEntriesYear2Plus)
                    {
                        string key = $"{seedId}:{vendorId}";
                        if (!seen.Add(key))
                            continue;

                        output.WriteLine(
                            $"[InlineData(\"{seedId}\")] //{seedName}"
                        );
                    }
                }
            }
        }







    }
}
