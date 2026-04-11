using PlantingDay.Helpers.Icons;

namespace PlantingDay.Helpers
{
    public interface IIconProvider
    {
        bool CanHandle(string id);
        Icon? LoadIcon(string id);
    }
}
