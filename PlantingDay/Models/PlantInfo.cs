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
        public string SeedId { get; set; } = ""; // e.g. 890, CarrotSeeds, Cornucopia_BasilSeeds
        public string HarvestId { get; set; } = ""; // e.g. 889, Carrot, Cornucopia_Basil, (O)638 [for fruit tree fruit]
        public PlantType PlantType { get; set; } // Crop, FruitTree, Bush

        // Output items (crops, fruit, bush drops)
        //public IReadOnlyList<DropInfo> Drops { get; set; } = Array.Empty<DropInfo>();


        //----
        // Planting the crop data
        //----
        public List<SeasonId> Seasons { get; set; } = new();

        // Production timing
        public int? DaysToProduce { get; set; }
        public int? RegrowDays { get; set; }


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



        //Raw Object info for the seed and harvest items
        public ItemInfo? Seed { get; set; } = new ItemInfo(); //Has raw icons
        public ItemInfo? Harvest { get; set; } = new ItemInfo(); //Has raw icons, sell price

        //---------
        // ICONS (use these ones, not the raw ones from ItemInfo)
        //---------

        public IconRef? SeedIconRef { get; set; }
        public IconRef? HarvestIconRef { get; set; }

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

    //public class DropInfo
    //{
    //    public string ItemId { get; set; } = "";
    //    public double ExtraHarvestChance { get; set; } = 1f; //e.g. potato
    //    public int MinStack { get; set; } = 1;
    //    public int MaxStack { get; set; } = 1;
    //}

    public class PurchaseInfo
    {
        public string VendorId { get; set; } = "";     // e.g. "SeedShop", "AriShop", "EggFestival"
        public string VendorName { get; set; } = "";   // e.g. "Pierre", "Ari", "Egg Festival"

        public int? GoldPrice { get; set; }            // null if it's a trade
        public string? TradeItemId { get; set; }       // e.g. "(O)852"
        public IconRef? CurrencyIconRef { get; set; }   // what you need to trade for 
        public int TradeAmount { get; set; }           // e.g. 4

        public string? Condition { get; set; }         // e.g. "EVENT eggFestival", "SEASON spring"
    }

    public class MonsterDropInfo
    {
        public string? MonsterName { get; set; }
        public float Chance { get; set; }
        public IconRef? MonsterIconRef { get; set; }
    }


    }



