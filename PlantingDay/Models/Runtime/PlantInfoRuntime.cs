using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SDVCommon.Icons;

namespace PlantingDay.Models.Runtime;

public class PlantInfoRuntime
{
    private readonly SDVData.PlantInfoData _data;

    public PlantInfoRuntime(SDVData.PlantInfoData data)
    {
        _data = data;
    }

    // Runtime-only icons (Texture2D, Rectangle, etc.)
    public Icon? SeedIcon { get; set; }
    //public Icon? HarvestIcon { get; set; }


    public class PurchaseInfo
    {
        public Icon? CurrencyIcon { get; set; }
    }


    public class MonsterDropInfo
    {
        public Icon? MonsterIcon { get; set; }
    }

}
