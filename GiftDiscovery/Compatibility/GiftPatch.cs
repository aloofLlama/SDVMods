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
            NPC _instance,
            StardewValley.Object o
        )
        {
            if (o is null)
                return;

            string npcName = _instance.Name;
            string qId = o.QualifiedItemId;
            string itemName = o.DisplayName;

            int tasteValue = _instance.getGiftTasteForThisItem(o);
            GiftTaste taste = (GiftTaste)tasteValue;

            SDVCommonLog.Log($"Learned taste: {npcName} → {itemName} = {taste} | {qId}", LogHelper.Info);

            TasteLearning.LearnTaste(qId, npcName, taste);
        }
    }
}


