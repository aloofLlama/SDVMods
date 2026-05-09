
namespace SDVCommon.Helpers
{
    public static class EconomicsHelper
    {
        public static int GetHarvestSellPriceFromSeed(string seedId)
        {
            var harvest = GameDataHelper.GetHarvestObjectFromSeedId(seedId);
            return harvest?.sellToStorePrice() ?? 0;
        }

    }
}
