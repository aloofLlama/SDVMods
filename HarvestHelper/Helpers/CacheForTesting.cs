using Newtonsoft.Json;
using SDVCommon;
using StardewModdingAPI;
using System;
using System.IO;
using System.Linq;

namespace HarvestHelper.Helpers
{
    public class CacheForTesting
    {
        public static void DumpHarvestInfoToJson()
        {
            // Serialize ONLY the data portion
            var dataOnly = HarvestInfoBuilder.AllHarvests
                .Select(h => h.Data)
                .ToList();

            string json = JsonConvert.SerializeObject(
                dataOnly,
                Formatting.Indented,
                new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                }
            );

            // Write into the mod folder under /Cache/
            string cacheDir = Path.Combine(ModEntry.Instance.Helper.DirectoryPath, "Cache");
            Directory.CreateDirectory(cacheDir);

            string stablePath = Path.Combine(cacheDir, "HarvestInfo.json");

            // 1. Write stable file for tests
            File.WriteAllText(stablePath, json);

            // 2. Write timestamped snapshot for debugging
            //string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            //string snapshotPath = Path.Combine(cacheDir, $"HarvestInfo_{timestamp}.json");

            //File.WriteAllText(snapshotPath, json);
        }
    }
}
