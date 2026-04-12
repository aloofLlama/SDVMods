namespace SDVData
{
    public class PlantInfoData
    {
        public string SeedId { get; set; } = "";
        public string HarvestId { get; set; } = "";
        public PlantType PlantType { get; set; }

        public List<SeasonId> Seasons { get; set; } = new();

        public int? DaysToProduce { get; set; }
        public int? RegrowDays { get; set; }

        public bool Trellis { get; set; }
        public bool Paddy { get; set; }
        public int MultiSprite { get; set; }
        public bool NeedsWatering { get; set; }
        public bool NeedsScythe { get; set; }

        //public int HarvestPrice { get; set; }
        public List<PurchaseInfoData> PurchaseOptions { get; set; } = new();
        public List<MonsterDropInfoData> MonsterDrops { get; set; } = new();

        public ItemInfo? Seed { get; set; } = new ItemInfo();
        //public ItemInfo? Harvest { get; set; } = new ItemInfo();

        //public IconRef? SeedIconRef { get; set; }
        //public IconRef? HarvestIconRef { get; set; }
    }

    public enum PlantType { Crop, FruitTree, Bush, Unknown }
    public enum SeasonId { Spring, Summer, Fall, Winter }


}
