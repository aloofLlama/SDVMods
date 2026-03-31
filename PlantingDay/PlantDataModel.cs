using System.Collections.Generic;

namespace PlantingDay
{

        public class PlantInfo
        {
            // Identity
            public string Id { get; set; } = "";
            public string SeedName { get; set; } = "";
            public string? SeedDescription { get; set; }

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
            public int MultiSprite { get; set; } //number of different colors (e.g. poppy)m 0 if just one


        public class DropInfo
            {
                public string ItemId { get; set; } = "";
                public float Chance { get; set; } = 1f;
                public int MinStack { get; set; } = 1;
                public int MaxStack { get; set; } = 1;
            }
        }
    }
    


