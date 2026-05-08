using SDVCommon.Models.Runtime;
using SDVData;


namespace SDVCommon.Models.Wrappers
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

