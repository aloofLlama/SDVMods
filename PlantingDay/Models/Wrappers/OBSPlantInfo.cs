//using PlantingDay.Models.Runtime;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace PlantingDay.Models.Wrappers
//{
//    public class OBSPlantInfo
//    {
//        public SDVData.PlantInfoData Data { get; }
//        public OBSPlantInfoRuntime Runtime { get; }

//        public List<OBSPurchaseInfoRuntime> PurchaseOptions { get; }
//        public List<OBSMonsterDropInfoRuntime> MonsterDrops { get; }

//        public OBSPlantInfo(SDVData.PlantInfoData data)
//        {
//            Data = data;
//            Runtime = new PlantInfoRuntime(data);

//            PurchaseOptions = data.PurchaseOptions
//                .Select(d => new PurchaseInfoRuntime(d))
//                .ToList();

//            MonsterDrops = data.MonsterDrops
//                .Select(d => new MonsterDropInfoRuntime(d))
//                .ToList();
//        }



//    }

//}
