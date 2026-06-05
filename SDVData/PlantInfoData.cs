namespace SDVData
{
    public enum PlantType { Crop, FruitTree, Bush, Unknown }
    public enum SeasonId { Spring, Summer, Fall, Winter }
    public enum Location { Unrestricted, Indoor, NoGardenPot }

    public class PlantInfoData
    {
        public string SeedId { get; set; } = ""; //unqualified ID (do not have (O) prefix)
        public string HarvestId { get; set; } = ""; //unqualified ID (do not have (O) prefix)
        public ItemInfo? Seed { get; set; } = new ItemInfo();


        public PlantType PlantType { get; set; }
        public List<SeasonId> Seasons { get; set; } = new();
        public Location Location { get; set; }

        public int? DaysToProduce { get; set; }
        public int? RegrowDays { get; set; }

        public bool Trellis { get; set; }
        public bool Paddy { get; set; }
        public bool NeedsWatering { get; set; }
        public bool NeedsScythe { get; set; }

        public int MultiSprite { get; set; }

        public List<PurchaseInfoData> PurchaseOptions { get; set; } = new();
        public List<MonsterDropInfoData> MonsterDrops { get; set; } = new();

    }



}
