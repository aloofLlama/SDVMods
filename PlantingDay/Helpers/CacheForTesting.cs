using Newtonsoft.Json;
using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlantingDay.Helpers
{
    public class CacheForTesting
    {
        public static void DumpPlantDataBaseToJson()
        {
            try
            {
                string json = JsonConvert.SerializeObject(
                            PlantDatabase.AllPlants,
                            Formatting.Indented
                        );

                // Dump directly into the mod folder
                string path = Path.Combine(
                    ModEntry.Instance.Helper.DirectoryPath,
                    "Cache",
                    "PlantDatabase.json"
                );

                Directory.CreateDirectory(Path.GetDirectoryName(path)!);

                File.WriteAllText(path, json);

                Directory.CreateDirectory(Path.GetDirectoryName(path)!);

                ModEntry.Instance.Monitor.Log($"PlantDatabase dumped to {path}", LogLevel.Info);
            }
            catch (Exception ex)
            {
                ModEntry.Instance.Monitor.Log($"Failed to dump PlantDatabase: {ex}", LogLevel.Error);
            }
        }

        private static string GetRepoRoot()
        {
            // Path to the compiled DLL inside your repo
            string assemblyPath = typeof(ModEntry).Assembly.Location;

            DirectoryInfo? dir = new DirectoryInfo(assemblyPath);

            while (dir != null)
            {
                // Look for your mod project file
                if (File.Exists(Path.Combine(dir.FullName, "PlantingDay.csproj")))
                {
                    // Parent of project folder = repo root
                    return dir.Parent!.FullName;
                }

                dir = dir.Parent;
            }

            throw new Exception("Could not locate repo root");
        }
        private static string GetRepoCachePath()
        {
            string repoRoot = GetRepoRoot();

            return Path.Combine(
                repoRoot,
                "PlantingDay_Tests",
                "TestData",
                "PlantDatabase.json"
            );
        }



    }
}
