namespace SDVData
{
    public class ItemInfo
    {
        public string Id { get; set; } = ""; //unqualified ID (do not have (O) prefix)
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public string Type { get; set; } = ""; // e.g. Crafting, Fish, Cooking, Seeds, Ring 

        public int Price { get; set; }
        public int Category { get; set; } //e.g. seeds, vegetable,fruit
        public int Edibility { get; set; }
        public List<string>? ContextTags { get; set; } //e.g. seedmaker_banned

    }

}
