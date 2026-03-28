
using System.Collections.Generic;

namespace SeedInfo.Models
{
    public class CropDataModel
    {
        public List<string>? Seasons { get; set; }
        public List<string>? GrowSeasons { get; set; }
        public List<string>? ExtraSeasons { get; set; }

        public string? FruitSeason { get; set; }
        public List<string>? HarvestSeasons { get; set; }

        public bool Trellis { get; set; }
        public bool Paddy { get; set; }
    }
}

//using System.Collections.Generic;

//namespace SeedInfo
//{
//    public class CropDataModel
//    {
//        // Vanilla crops
//        public List<string> Seasons { get; set; } = new();
//        public bool Trellis { get; set; }
//        public bool Paddy { get; set; }

//        // Fruit trees
//        public string? FruitSeason { get; set; }

//        // Custom bushes
//        public List<string>? HarvestSeasons { get; set; }

//        // Modded crop fields
//        public List<string>? GrowSeasons { get; set; }
//        public List<string>? ExtraSeasons { get; set; }

//    }
//}

