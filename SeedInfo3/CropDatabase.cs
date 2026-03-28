using SeedInfo;
using SeedInfo.Models;
using StardewModdingAPI;
using StardewValley;
using StardewValley.GameData.Buffs;
using StardewValley.GameData.Crops;
using StardewValley.GameData.FruitTrees;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace SeedInfo
{
    public static class CropDatabase
    {
        private static IModHelper _helper = null!;
        public static Dictionary<string, CropDataModel> Crops { get; private set; } = new();

        public static void Load(IModHelper helper)
        {
            _helper = helper;
            Crops = new Dictionary<string, CropDataModel>();

            // Vanilla (strongly typed)
            LoadVanillaCrops();
            LoadFruitTrees();
            //LoadBushes();

            // Modded (JSON)
            LoadModded("Mods/cornucopia.crops/Data/Crops");
            LoadModded("Mods/raccoon.crops/Data/Crops");
            LoadModded("Mods/sunberryvillage/Data/Crops");



        }

        //private static void LoadDynamic(string path, Action<string, JsonElement> handler)
        //{
        //    var assetName = _helper.GameContent.ParseAssetName(path);

        //    if (!_helper.GameContent.DoesAssetExist<Dictionary<string, JsonElement>>(assetName))
        //        return;

        //    var raw = _helper.GameContent.Load<Dictionary<string, CropData>>("Data/Crops");

        //    foreach (var (key, entry) in raw)
        //        handler(key, entry);
        //}

        private static void LoadVanillaCrops()
        {
            var raw = _helper.GameContent.Load<Dictionary<string, CropData>>("Data/Crops");

            foreach (var (key, crop) in raw)
            {
                Crops[key] = new CropDataModel
                {
                    Seasons = crop.Seasons?
                        .Select(s => s.ToString())
                        .ToList(),
                    Trellis = crop.IsRaised,
                    Paddy = crop.IsPaddyCrop
                };
            }
        }

        private static void LoadFruitTrees()
        {
            var raw = _helper.GameContent.Load<Dictionary<string, FruitTreeData>>("Data/FruitTrees");

            foreach (var (key, tree) in raw)
            {
                string? season = tree.Seasons?
                    .Select(s => s.ToString().ToLowerInvariant())
                    .FirstOrDefault();

                Crops[key] = new CropDataModel
                {
                    FruitSeason = season
                };
            }
        }

        //private static void LoadBushes()
        //{
        //    //var assetName = _helper.GameContent.ParseAssetName("Data/Bushes");

        //    //if (!_helper.GameContent.DoesAssetExist<Dictionary<string, JsonElement>>(assetName))
        //    //    return;

        //    var raw = _helper.GameContent.Load<Dictionary<string, JsonElement>>("Data/Bushes");

        //    foreach (var (key, bush) in raw)
        //    {
        //        List<string>? seasons = null;

        //        if (bush.TryGetProperty("HarvestSeasons", out var hsProp))
        //        {
        //            if (hsProp.ValueKind == JsonValueKind.Array)
        //            {
        //                seasons = hsProp.EnumerateArray()
        //                    .Select(e => e.GetString() ?? "")
        //                    .Where(s => !string.IsNullOrWhiteSpace(s))
        //                    .ToList();
        //            }
        //            else if (hsProp.ValueKind == JsonValueKind.String)
        //            {
        //                seasons = hsProp.GetString()?
        //                    .Split(' ', StringSplitOptions.RemoveEmptyEntries)
        //                    .ToList();
        //            }
        //        }

        //        Crops[key] = new CropDataModel
        //        {
        //            HarvestSeasons = seasons
        //        };
        //    }
        //}
        

        private static void LoadModded(string path)
        {
            var assetName = _helper.GameContent.ParseAssetName(path);

            if (!_helper.GameContent.DoesAssetExist<Dictionary<string, JsonElement>>(assetName))
                return;

            var raw = _helper.GameContent.Load<Dictionary<string, JsonElement>>(path);

            foreach (var (key, entry) in raw)
                LoadModdedCropFields(key, entry);
        }

        private static void LoadModdedCropFields(string key, JsonElement crop)
        {
            if (!Crops.ContainsKey(key))
                Crops[key] = new CropDataModel();

            // GrowSeasons
            if (crop.TryGetProperty("GrowSeasons", out var gsProp))
            {
                if (gsProp.ValueKind == JsonValueKind.Array)
                    Crops[key].GrowSeasons = gsProp.EnumerateArray().Select(e => e.GetString() ?? "").ToList();
                else if (gsProp.ValueKind == JsonValueKind.String)
                    Crops[key].GrowSeasons = gsProp.GetString()?.Split(' ', StringSplitOptions.RemoveEmptyEntries).ToList();
            }

            // ExtraSeasons
            if (crop.TryGetProperty("ExtraSeasons", out var esProp))
            {
                if (esProp.ValueKind == JsonValueKind.Array)
                    Crops[key].ExtraSeasons = esProp.EnumerateArray().Select(e => e.GetString() ?? "").ToList();
                else if (esProp.ValueKind == JsonValueKind.String)
                    Crops[key].ExtraSeasons = esProp.GetString()?.Split(' ', StringSplitOptions.RemoveEmptyEntries).ToList();
            }
        }

        public static List<string> GetSeasons(string itemId)
        {
            if (!Crops.TryGetValue(itemId, out var data))
                return new List<string>();

            return GetAllSeasons(data);
        }

        private static List<string> GetAllSeasons(CropDataModel data)
        {
            var seasons = new List<string>();

            if (data.Seasons != null)
                seasons.AddRange(data.Seasons);

            if (data.GrowSeasons != null)
                seasons.AddRange(data.GrowSeasons);

            if (data.ExtraSeasons != null)
                seasons.AddRange(data.ExtraSeasons);

            if (!string.IsNullOrEmpty(data.FruitSeason))
                seasons.Add(data.FruitSeason);

            if (data.HarvestSeasons != null)
                seasons.AddRange(data.HarvestSeasons);

            return seasons
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Select(s => s.Trim().ToLowerInvariant())
                .Distinct()
                .Select(s => char.ToUpper(s[0]) + s.Substring(1))
                .ToList();
        }
    }
}


//using SeedInfo;
//using StardewModdingAPI;
//using StardewValley.Objects;
//using StardewValley.GameData.Buffs;
////using StardewValley.GameData.Bushes;
//using StardewValley.GameData.Crops;
//using StardewValley.GameData.FruitTrees;
//using System.Collections.Generic;
//using System.Linq;
////using SeedInfo.Models;

//namespace SeedInfo.Data
//{
//    public static class CropDatabase
//    {
//        private static IModHelper _helper = null!;
//        public static Dictionary<string, CropDataModel> Crops { get; private set; } = new();

//        public static void Load(IModHelper helper)
//        {
//            _helper = helper;
//            Crops = new Dictionary<string, CropDataModel>();

//            LoadVanillaCrops();
//            LoadFruitTrees();
//            LoadBushes();

//            // Add mod packs here as needed
//            LoadModdedCrops("Mods/cornucopia.crops/Data/Crops");
//            LoadModdedCrops("Mods/raccoon.crops/Data/Crops");
//            LoadModdedCrops("Mods/sunberryvillage/Data/Crops");
//        }

//        private static void LoadVanillaCrops()
//        {
//            var raw = _helper.GameContent
//                .Load<Dictionary<string, CropData>>("Data/Crops");

//            foreach (var (key, crop) in raw)
//            {
//                Crops[key] = new CropDataModel
//                {
//                    Seasons = crop.Seasons?.Select(s => s.ToString()).ToList() ?? new(),
//                    Trellis = crop.IsRaised,
//                    Paddy = crop.IsPaddyCrop
//                };
//            }
//        }

//        private static void LoadFruitTrees()
//        {
//            var raw = _helper.GameContent
//                .Load<Dictionary<string, FruitTreeData>>("Data/FruitTrees");

//            foreach (var (key, tree) in raw)
//            {
//                Crops[key] = new CropDataModel
//                {
//                    FruitSeason = tree.FruitSeason
//                };
//            }
//        }

//        private static void LoadBushes()
//        {
//            var raw = _helper.GameContent
//                .Load<Dictionary<string, BushData>>("Data/Bushes");

//            foreach (var (key, bush) in raw)
//            {
//                Crops[key] = new CropDataModel
//                {
//                    HarvestSeasons = bush.HarvestSeasons?.ToList()
//                };
//            }
//        }

//        private static void LoadModdedCrops(string path)
//        {
//            if (!_helper.GameContent.DoesAssetExist(path))
//                return;

//            var raw = _helper.GameContent.Load<Dictionary<string, dynamic>>(path);

//            foreach (var (key, crop) in raw)
//            {
//                if (!Crops.ContainsKey(key))
//                    Crops[key] = new CropDataModel();

//                Crops[key].GrowSeasons = crop.GrowSeasons?.ToObject<List<string>>();
//                Crops[key].ExtraSeasons = crop.ExtraSeasons?.ToObject<List<string>>();
//            }
//        }

//        public static List<string> GetSeasons(string itemId)
//        {
//            if (!Crops.TryGetValue(itemId, out var data))
//                return new List<string>();

//            return GetAllSeasons(data);
//        }

//        private static List<string> GetAllSeasons(CropDataModel data)
//        {
//            var seasons = new List<string>();

//            if (data.Seasons != null)
//                seasons.AddRange(data.Seasons);

//            if (data.GrowSeasons != null)
//                seasons.AddRange(data.GrowSeasons);

//            if (data.ExtraSeasons != null)
//                seasons.AddRange(data.ExtraSeasons);

//            if (!string.IsNullOrEmpty(data.FruitSeason))
//                seasons.Add(data.FruitSeason);

//            if (data.HarvestSeasons != null)
//                seasons.AddRange(data.HarvestSeasons);

//            return seasons
//                .Where(s => !string.IsNullOrWhiteSpace(s))
//                .Select(s => s.Trim().ToLowerInvariant())
//                .Distinct()
//                .Select(s => char.ToUpper(s[0]) + s.Substring(1))
//                .ToList();
//        }
//    }
//}
