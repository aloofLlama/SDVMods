using PlantingDay;
using PlantingDay.Models;
using PlantingDay_Tests.Helpers;
using Newtonsoft.Json;

namespace PlantingDay_Tests.Helpers
{
    public static class TestPlantDatabase
    {
        public static IReadOnlyList<PlantInfo> Load()
        {
            var json = File.ReadAllText("TestData/PlantDatabase.json");
            return JsonConvert.DeserializeObject<List<PlantInfo>>(json)!;
        }
    }
}
