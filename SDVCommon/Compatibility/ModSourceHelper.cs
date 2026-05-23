using SDVCommon.GameData;
using SDVCommon.Helpers;
using SDVData;
using StardewModdingAPI;
using StardewValley;

namespace SDVCommon.Compatibility
{
    public static class ModSourceHelper
    {
        public static ModSource GetModSource(string canonicalId)
        {
            var meta = ItemRegistry.GetMetadata(canonicalId);

            if (IdHelper.IsVanillaStardew(canonicalId))
                return ModSource.Stardew;

            // Special case: Nature in the Valley uses dot notation
            if (canonicalId.StartsWith("NatInValley.", StringComparison.OrdinalIgnoreCase))
                return ModSource.NatureInTheValley;

            // Extract mod prefix
            string modPrefix = canonicalId.Split('_')[0];

            ModSource result = modPrefix switch
            {
                // Expansions
                "skellady.SBVCP" => ModSource.Sunberry,
                "FlashShifter.StardewValleyExpandedCP" => ModSource.Expanded,
                "supert" => ModSource.AdventureGuild,

                // Crops/cooking/goods/etc
                "slimerrain.uncleirohapprovedteacp" => ModSource.Slimerrain_UncleIrohTea,
                "slimerrain.grainsoverhullcp" => ModSource.Slimerain_GrainsOverhull,
                "Cornucopia" => ModSource.Cornucopia,
                "HeyKatu.CulinaryDelight" => ModSource.CulinaryDelight,
                "bb.moreTea" => ModSource.SimpleTea,
                "MNF.MoreNewFish" => ModSource.MoreNewFish,

                _ => ModSource.Unknown
            };

            if (result == ModSource.Unknown)
            {
                // Try to get object data so we can log the raw category
                var info = GameObject.FromObject(canonicalId);
                int category = info?.Category ?? -999; // fallback if null

                SDVCommonLog.Log(
                    $"Unknown mod prefix '{modPrefix}' for item '{canonicalId}' (Category: {category})", 
                    LogHelper.DebugWarnOrTrace
                );
            }

            return result;

        }

    }

}
