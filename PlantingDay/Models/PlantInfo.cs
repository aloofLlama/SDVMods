using Microsoft.Xna.Framework.Graphics;
using PlantingDay.Helpers;
using StardewModdingAPI;
using StardewValley;
using StardewValley.GameData.Crops;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PlantingDay.Models
{

    public class PlantInfo
    {

        public string Id { get; set; } = ""; //Seed Object ID
        public PlantType PlantType { get; set; } // Crop, FruitTree, Bush

        //Object info for the seed and harvest items
        public ItemInfo? Seed { get; set; } = new ItemInfo();
        public ItemInfo? Harvest { get; set; } = new ItemInfo();


        // TODO Need to migrate this to the new Harvest above
        public string HarvestId { get; set; } = "";

        //----
        // Planting the crop data
        //----
        public List<SeasonId> Seasons { get; set; } = new();

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
            public double ExtraHarvestChance { get; set; } = 1f; //e.g. potato
            public int MinStack { get; set; } = 1;
            public int MaxStack { get; set; } = 1;
        }

        // TODO Need to migrate this to the new Harvest above

        //// Usable icon reference after database has been initialized
        ////[JsonIgnore]
        //public IconRef? HarvestIcon { get; set; }

        ////[JsonIgnore]
        //public IconRef? SeedIcon { get; set; }
        public Texture2D? SeedIconTexture { get; set; }
        public Texture2D? HarvestIconTexture { get; set; }

        public void InitializeIcons()
        {
            if (Seed != null)
            {
                var item = ItemRegistry.Create(Seed.Id);
                if (item != null)
                    SeedIconTexture = PlantDatabase.RenderItemIcon(item, 16);

                ModEntry.Instance.Monitor.Log(
$"Assigning SeedIconTexture for : {(SeedIconTexture != null)}",
LogLevel.Alert);

            }

            if (Harvest != null)
            {
                var item = ItemRegistry.Create(Harvest.Id);
                if (item != null)
                    HarvestIconTexture = PlantDatabase.RenderItemIcon(item, 32);
                ModEntry.Instance.Monitor.Log(
    $"Assigning HarvestIconTexture for : {(HarvestIconTexture != null)}",
    LogLevel.Alert
);
            }
        }

    }
    public enum PlantType
    {
        Crop,
        FruitTree,
        Bush,
        Unknown
    }
    public enum SeasonId
    {
        Spring,
        Summer,
        Fall,
        Winter
    }



}



