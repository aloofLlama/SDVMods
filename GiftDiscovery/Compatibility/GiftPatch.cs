using GiftDiscovery.Models;
using GiftDiscovery.Services;
using HarmonyLib;
using SDVCommon.Services;
using StardewModdingAPI;
using StardewValley;

namespace GiftDiscovery.Compatibility
{
    public static class GiftPatch
    {
        public static void Postfix(
            NPC __instance,
            StardewValley.Object o
        )
        {
            if (o is null)
                return;

            string npcName = __instance.Name;
            string qualifiedItemId = o.QualifiedItemId;
            string itemName = o.DisplayName;

            int tasteValue = __instance.getGiftTasteForThisItem(o);
            GiftTaste taste = (GiftTaste)tasteValue;

            SDVCommonLog.Log($"Learned taste: {npcName} → {itemName} = {taste} | {qualifiedItemId}", LogHelper.Info);

            TasteLearning.LearnTaste(qualifiedItemId, npcName, taste);
        }
    }
}


