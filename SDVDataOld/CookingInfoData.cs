namespace SDVData
{

    public class CookingInfoData
    {
        public string RecipeName { get; set; } = "";
        public string DisplayName { get; set; } = "";
        public string OutputId { get; set; } = "";
        public int OutputCount { get; set; }
        public List<RecipeIngredientData> Ingredients { get; set; } = new();
    }

    public class RecipeIngredientData
    {
        public string IngredientId { get; set; } = "";
        public int Count { get; set; }
    }
}
