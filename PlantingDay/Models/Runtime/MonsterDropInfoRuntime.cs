using SDVCommon.Icons;
using SDVData;

namespace PlantingDay.Models.Runtime;

public class MonsterDropInfoRuntime
{
    public MonsterDropInfoData Data { get; }
    public Icon? MonsterIcon { get; set; }

    public MonsterDropInfoRuntime(MonsterDropInfoData data)
    {
        Data = data;
    }
}
