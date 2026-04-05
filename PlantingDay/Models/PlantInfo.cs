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

        public string SeedId { get; set; } = ""; 
        public PlantType PlantType { get; set; } // Crop, FruitTree, Bush

        //Object info for the seed and harvest items
        public ItemInfo? Seed { get; set; } = new ItemInfo();
        public ItemInfo? Harvest { get; set; } = new ItemInfo();


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


        // Economics
        public int HarvestPrice { get; set; }
        public List<PurchaseInfo> PurchaseOptions { get; set; } = new();
        public List<MonsterDropInfo> MonsterDrops { get; set; } = new();




        //---------
        // ICONS
        //---------

        public Texture2D? SeedIconTexture { get; set; }
        public Texture2D? HarvestIconTexture { get; set; }

        

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

    public class DropInfo
    {
        public string ItemId { get; set; } = "";
        public double ExtraHarvestChance { get; set; } = 1f; //e.g. potato
        public int MinStack { get; set; } = 1;
        public int MaxStack { get; set; } = 1;
    }

    public class PurchaseInfo
    {
        public string VendorId { get; set; } = "";     // e.g. "SeedShop", "AriShop", "EggFestival"
        public string VendorName { get; set; } = "";   // e.g. "Pierre", "Ari", "Egg Festival"

        public int? GoldPrice { get; set; }            // null if it's a trade
        public string? TradeItemId { get; set; }       // e.g. "(O)852"
        public IconRef? CurrencyIconRef { get; set; }   // what you need to trade for 
        public int TradeAmount { get; set; }           // e.g. 4

        public string? Condition { get; set; }         // e.g. "EVENT eggFestival", "SEASON spring"
        public bool IsNightMarket { get; set; }
    }

    public class MonsterDropInfo
    {
        public string? MonsterName { get; set; }
        public float Chance { get; set; }
        public IconRef? MonsterIconRef { get; set; }
}


    }



