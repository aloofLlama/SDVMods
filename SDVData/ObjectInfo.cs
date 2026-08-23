namespace SDVData
{
    // Object info from the game
    // See: https://stardewvalleywiki.com/Modding:Objects
    // The game key's on unqualified Ids, but we use qualified Ids otherwise the info returns other item type results
    public class ObjectInfo
    {
        public string QId { get; set; } = "";
        public string DisplayName { get; set; } = "";
        public string Description { get; set; } = "";
        public string Type { get; set; } = ""; // e.g. Crafting, Fish, Cooking, Seeds, Ring 

        public int Price { get; set; }
        public int Category { get; set; } //e.g. seeds, vegetable,fruit
        public int Edibility { get; set; }
        public List<string>? ContextTags { get; set; } //e.g. seedmaker_banned

    }

}
