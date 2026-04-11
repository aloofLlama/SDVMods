using PlantingDay.Helpers.Icons;
using PlantingDay.Models;

namespace PlantingDay.RuntimeModels;

public class MonsterDropInfoRuntime
{
    public MonsterDropInfoData Data { get; }
    public Icon? MonsterIcon { get; set; }

    public MonsterDropInfoRuntime(MonsterDropInfoData data)
    {
        Data = data;
    }
}
