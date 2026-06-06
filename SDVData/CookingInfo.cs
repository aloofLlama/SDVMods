namespace SDVData
{

    public class CookingInfo
    {
        public string RecipeName { get; set; } = "";
        public string OutputDisplayName { get; set; } = "";
        public string OutputId { get; set; } = "";
        public int OutputCount { get; set; }
        public List<RecipeIngredient> Ingredients { get; set; } = new();
    }

    public class RecipeIngredient
    {
        public string IngredientId { get; set; } = "";
        public int Count { get; set; }
    }
}
