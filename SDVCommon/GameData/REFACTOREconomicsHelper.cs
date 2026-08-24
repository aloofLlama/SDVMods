
using SDVCommon.GameData;

namespace SDVCommon.Helpers
{
    public static class REFACTOREconomicsHelper
    {
        public static int GetHarvestSellPriceFromSeed(string seedQId)
        {
            SeedHarvestMap.TryGetHarvestId(seedQId, out string? harvestQId);

            if (harvestQId == null)
                return 0;

            var harvest = GameObject.GetObjectInstance(harvestQId);

            return harvest?.sellToStorePrice() ?? 0;
        }


    }
}
