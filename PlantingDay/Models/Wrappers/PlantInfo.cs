using PlantingDay.Models.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlantingDay.Models.Wrappers
{
    public class PlantInfo
    {
        public SDVData.PlantInfoData Data { get; }
        public PlantInfoRuntime Runtime { get; }

        public PlantInfo(SDVData.PlantInfoData data)
        {
            Data = data;
            Runtime = new PlantInfoRuntime(data);
        }

        public List<PurchaseInfoRuntime> PurchaseOptions =>
                Data.PurchaseOptions
                    .Select(d => new PurchaseInfoRuntime(d))
                    .ToList();

        public List<MonsterDropInfoRuntime> MonsterDrops =>
            Data.MonsterDrops
                .Select(d => new MonsterDropInfoRuntime(d))
                .ToList();

    }

}
