using StardewValley;
using HarmonyLib;
using StardewModdingAPI;
using GiftDiscovery.Models;
using GiftDiscovery.Services;

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

            ModEntry.Instance.Monitor.Log(
                $"Learned taste: {npcName} → {itemName} = {taste} | {qualifiedItemId}",
                LogLevel.Debug
                );

            GiftKnowledgeService.LearnTaste(qualifiedItemId, npcName, taste);
        }
    }
}


