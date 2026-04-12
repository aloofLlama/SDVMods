using SDVCommon.RuntimeModels;
using SDVData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlantingDay.Models.Wrappers
{
    public class HarvestInfo
    {
        public HarvestInfoData Data { get; }
        public HarvestInfoRuntime Runtime { get; }

        public HarvestInfo(HarvestInfoData data)
        {
            Data = data;
            Runtime = new HarvestInfoRuntime();
        }
    }
}
