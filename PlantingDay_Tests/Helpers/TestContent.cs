using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StardewModdingAPI;

namespace PlantingDay_Tests.Helpers
{
    //loads the game content for use by testing

    public class TestContent : IContentHelper
    {
        private readonly string _gameContent;
        private readonly string _modsFolder;

        public TestContent(string gamePath, string modsPath)
        {
            _gameContent = Path.Combine(gamePath, "Content");
            _modsFolder = modsPath;
        }

        public T Load<T>(string key)
        {
            // key example: "Data/Crops"
            string vanillaPath = Path.Combine(_gameContent, key + ".json");

            if (!File.Exists(vanillaPath))
                throw new FileNotFoundException($"Vanilla asset not found: {vanillaPath}");

            // Load vanilla JSON
            JObject merged = JObject.Parse(File.ReadAllText(vanillaPath));

            // Merge modded JSON
            foreach (var modDir in Directory.GetDirectories(_modsFolder))
            {
                string modDataPath = Path.Combine(modDir, "assets", key + ".json");
                if (!File.Exists(modDataPath))
                    continue;

                JObject modJson = JObject.Parse(File.ReadAllText(modDataPath));

                merged.Merge(modJson, new JsonMergeSettings
                {
                    MergeArrayHandling = MergeArrayHandling.Union,
                    MergeNullValueHandling = MergeNullValueHandling.Ignore
                });
            }

            return merged.ToObject<T>();
        }

        // Everything else can throw until needed
        public IContentPack CreateContentPack(string path) => throw new NotImplementedException();
        public void InvalidateCache(string key) => throw new NotImplementedException();
        public void InvalidateCache() => throw new NotImplementedException();
        public void RegisterXnbReplacement(string key, string localAssetName) => throw new NotImplementedException();
        public void RegisterXnbReplacement(string key, Func<string> localAssetName) => throw new NotImplementedException();
        public void RegisterXnbReplacement(string key, Func<string> localAssetName, bool replaceEntireFile) => throw new NotImplementedException();
        public void RegisterXnbReplacement(string key, string localAssetName, bool replaceEntireFile) => throw new NotImplementedException();
    }
