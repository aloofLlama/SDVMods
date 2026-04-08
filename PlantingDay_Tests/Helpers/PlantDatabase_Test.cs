using Newtonsoft.Json.Linq;
using PlantingDay.Helpers;
using PlantingDay_Tests.Helpers;

public class PlantDatabase_Test
{
    public PlantDatabase_Test()
    {
        var config = JObject.Parse(File.ReadAllText("testpaths.json"));
        string gamePath = config["GamePath"]!.ToString();
        string modsPath = config["ModsPath"]!.ToString();

        var testContent = new TestContent(gamePath, modsPath);

        Initialize.ForTests(testContent);
    }

}