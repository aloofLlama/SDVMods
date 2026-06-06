using SDVCommon.Models.Runtime;
using SDVData;

namespace SDVCommon.Models.Wrappers
{
    public class PlantInfo
    {
        public PlantInfoData Data { get; }
       //public PlantInfoRuntime Runtime { get; }

        public List<PurchaseInfo> PurchaseOptions { get; }
        public List<MonsterDropInfo> MonsterDrops { get; }
        public PlantInfo(PlantInfoData data)
        {
            {
                Data = data;
                //Runtime = new PlantInfoRuntime();

                PurchaseOptions = data.PurchaseOptions
                    .Select(d => new PurchaseInfo(d))
                    .ToList();

                MonsterDrops = data.MonsterDrops
                    .Select(d => new MonsterDropInfo(d))
                    .ToList();
            }
        }

    }

    public class PurchaseInfo
    {
        public PurchaseInfoData Data { get; }
        //public PurchaseInfoRuntime Runtime { get; }

        public PurchaseInfo(PurchaseInfoData data)
        {
            Data = data;
            //Runtime = new PurchaseInfoRuntime();
        }
    }

    public class MonsterDropInfo
    {
        public MonsterDropInfoData Data { get; }
        public MonsterDropInfoRuntime Runtime { get; }

        public MonsterDropInfo(MonsterDropInfoData data)
        {
            Data = data;
            Runtime = new MonsterDropInfoRuntime();
        }
    }

}
