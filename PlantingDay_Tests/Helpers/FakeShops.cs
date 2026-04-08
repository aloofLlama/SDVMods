using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlantingDay;
using PlantingDay.Models;
using PlantingDay.Helpers;


namespace PlantingDay_Tests.Helpers
{
    public static class FakeShop
    {
        public static FakeShopItem ItemFromPlantDatabase(string seedId, string vendorId)
        {
            var plant = PlantDatabase.AllPlants.First(p => p.SeedId == seedId);
            var option = plant.PurchaseOptions.First(o => o.VendorId == vendorId);

            return new FakeShopItem(seedId, option.GoldPrice!.Value);
        }
    }
