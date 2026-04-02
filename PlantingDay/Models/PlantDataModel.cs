using PlantingDay.Helpers;
using StardewValley.GameData.Crops;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PlantingDay.Models
{

    public class PlantInfo
    {
        // Identity
        public string Id { get; set; } = "";
        public string SeedName { get; set; } = "";
        public string? SeedDescription { get; set; }
        public PlantType PlantType { get; set; }

        public string HarvestId { get; set; } = "";
        public string HarvestName { get; set; } = "";
        public string? HarvestDescription { get; set; }


        // Seasons where the plant produces something
        public IReadOnlyList<string> Seasons { get; set; } = Array.Empty<string>();

        // Production timing
        public int? DaysToProduce { get; set; }
        public int? RegrowDays { get; set; }

        // Output items (crops, fruit, bush drops)
        public IReadOnlyList<DropInfo> Drops { get; set; } = Array.Empty<DropInfo>();

        // Flags (optional)
        public bool Trellis { get; set; }
        public bool Paddy { get; set; }
        public int MultiSprite { get; set; } //number of different colors (e.g. poppy) 0 if just one
        public bool NeedsWatering { get; set; }
        public HarvestMethod Scythe { get; set; }



        public class DropInfo
        {
            public string ItemId { get; set; } = "";
            public float Chance { get; set; } = 1f;
            public int MinStack { get; set; } = 1;
            public int MaxStack { get; set; } = 1;
        }

        // References for the sprite image of the growing crop
        //public string? HarvestTexture { get; set; }
        //public int HarvestSpriteIndex { get; set; }

        // Usable icon reference after database has been initialized
        [JsonIgnore]
        public IconRef? HarvestIcon { get; set; }

        [JsonIgnore]
        public IconRef? SeedIcon { get; set; }

    }
    public enum PlantType
    {
        Crop,
        FruitTree,
        Bush,
        Unknown
    }
}



