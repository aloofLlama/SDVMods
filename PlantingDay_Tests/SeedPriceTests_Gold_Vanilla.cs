using PlantingDay;
using PlantingDay.Models;
using PlantingDay_Tests.Helpers;

namespace PlantingDay_Tests
{
    public class SeedPriceTests_Vanilla
    {
        private readonly IReadOnlyList<PlantInfo> _plants;

        public SeedPriceTests_Vanilla()
        {
            _plants = TestPlantDatabase.Load();
        }

        [Fact]
        public void ParsnipSeeds_ShouldCost20_AtPierre()
        {
            var seed = _plants.Single(p => p.SeedId == "472");
            var shop = seed.PurchaseOptions.Single(p => p.VendorId == "SeedShop");
            var expectedPrice = 20;

            Assert.Equal(expectedPrice, shop.GoldPrice);
        }
    }










    //        [Fact]
    //        public void ParsnipSeeds_ShouldCost20_AtPierre()
    //        {
    //            string fromShop = "SeedShop";
    //            string fromSeedId = "(O)472";
    //            string 



    //            var fakeShops = FakeShop.Create(
    //                ("SeedShop", FakeShop.Shop(
    //                    FakeShop.Item("(O)472", 20)
    //                ))
    //            );

    //            var shops = ShopAdapter.Convert(fakeShops);

    //            var result = VendorHelper.BuildPurchaseInfo("472", shops);

    //            Assert.Single(result);
    //            Assert.Equal("SeedShop", result[0].VendorId);
    //            Assert.Equal(20, result[0].GoldPrice);
    //        }




    //        [Fact]
    //        public void SunflowerSeeds_ShouldCost200_AtPierre()
    //        {
    //            var fakeShops = FakeShop.Create(
    //                ("SeedShop", FakeShop.Shop(
    //                    FakeShop.Item("(O)431", 100)
    //                ))
    //);

    //            var shops = ShopAdapter.Convert(fakeShops);

    //            var result = VendorHelper.BuildPurchaseInfo("431", shops);

    //            Assert.Single(result);
    //            Assert.Equal("SeedShop", result[0].VendorId);
    //            Assert.Equal(200, result[0].GoldPrice);
    //        }
    //        [Fact]
    //        public void CranberrySeeds_ShouldCost240_AtPierre()
    //        {
    //            var fakeShops = FakeShop.Create(
    //                ("SeedShop", FakeShop.Shop(
    //                    FakeShop.Item("(O)493", 0)
    //                ))
    //);

    //            var shops = ShopAdapter.Convert(fakeShops);

    //            var result = VendorHelper.BuildPurchaseInfo("493", shops);

    //            Assert.Single(result);
    //            Assert.Equal("SeedShop", result[0].VendorId);
    //            Assert.Equal(240, result[0].GoldPrice);
    //        }





    //    [Fact]
    //    public void Seed_ShouldOnlyAppearInPierreShop()
    //    {
    //        var shops = FakeShop.Create(
    //            ("SeedShop", FakeShop.Shop(
    //                FakeShop.Item("(O)495", 80)
    //            )),
    //            ("AriShop", FakeShop.Shop(
    //                FakeShop.Item("(O)9999", 100)
    //            ))
    //        );

    //        var result = PurchaseInfoLogic.BuildPurchaseInfo("495", shops);

    //        Assert.Single(result);
    //        Assert.Equal("SeedShop", result[0].VendorId);
    //    }
    //    [Fact]
    //    public void Seed_ShouldOnlyAppearInModdedShop()
    //    {
    //        var shops = FakeShop.Create(
    //            ("MagicShop", FakeShop.Shop(
    //                FakeShop.Item("(O)7777", 500)
    //            ))
    //        );

    //        var result = PurchaseInfoLogic.BuildPurchaseInfo("7777", shops);

    //        Assert.Single(result);
    //        Assert.Equal("MagicShop", result[0].VendorId);
    //        Assert.Equal(500, result[0].GoldPrice);
    //    }

    //    [Fact]
    //    public void ModdedSeed_ShouldAppearOnlyInAriShop()
    //    {
    //        var shops = FakeShop.Create(
    //            ("AriShop", FakeShop.Shop(
    //                FakeShop.Item("(O)9000", 150)
    //            ))
    //        );

    //        var result = PurchaseInfoLogic.BuildPurchaseInfo("9000", shops);

    //        Assert.Single(result);
    //        Assert.Equal("AriShop", result[0].VendorId);
    //        Assert.Equal(150, result[0].GoldPrice);
    //    }
    //    [Fact]
    //    public void CranberrySeeds_ShouldAppearInMultipleShops()
    //    {
    //        var shops = FakeShop.Create(
    //            ("SeedShop", FakeShop.Shop(
    //                FakeShop.Item("(O)493", 240)
    //            )),
    //            ("AriShop", FakeShop.Shop(
    //                FakeShop.Item("(O)493", 300)
    //            ))
    //        );

    //        var result = PurchaseInfoLogic.BuildPurchaseInfo("493", shops);

    //        Assert.Equal(2, result.Count);

    //        Assert.Contains(result, r => r.VendorId == "SeedShop" && r.GoldPrice == 240);
    //        Assert.Contains(result, r => r.VendorId == "AriShop" && r.GoldPrice == 300);
    //    }
    //    [Fact]
    //    public void ModdedSeed_ShouldApplyPriceModifiers()
    //    {
    //        var item = FakeShop.Item("(O)8888", 100);
    //        item.PriceModifiers = new[]
    //        {
    //    new PriceModifierData
    //    {
    //        Modification = ModificationType.Add,
    //        Amount = 20
    //    }
    //};

    //        var shops = FakeShop.Create(
    //            ("AriShop", FakeShop.Shop(item))
    //        );

    //        var result = PurchaseInfoLogic.BuildPurchaseInfo("8888", shops);

    //        Assert.Single(result);
    //        Assert.Equal(120, result[0].GoldPrice);
    //    }


}
