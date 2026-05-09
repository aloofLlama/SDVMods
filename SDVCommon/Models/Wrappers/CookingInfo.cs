using SDVCommon.Models.Runtime;
using SDVData;

namespace SDVCommon.Models.Wrappers
{
    public class CookingInfo
    {
        public CookingInfoData Data { get; }
        public CookingInfoRuntime Runtime { get; }

        public CookingInfo(CookingInfoData data, CookingInfoRuntime runtime)
        {
            Data = data;
            Runtime = runtime;
        }
    }

}
