using PlantingDay.Models;
using PlantingDay.RuntimeModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlantingDay
{
    public class PlantInfo
    {
        public PlantInfoData Data { get; }
        public PlantInfoRuntime Runtime { get; }

        public List<PurchaseInfoRuntime> PurchaseOptions { get; }
        public List<MonsterDropInfoRuntime> MonsterDrops { get; }

        public PlantInfo(PlantInfoData data)
        {
            Data = data;
            Runtime = new PlantInfoRuntime(data);

            PurchaseOptions = data.PurchaseOptions
                .Select(d => new PurchaseInfoRuntime(d))
                .ToList();

            MonsterDrops = data.MonsterDrops
                .Select(d => new MonsterDropInfoRuntime(d))
                .ToList();
        }
    }

}
