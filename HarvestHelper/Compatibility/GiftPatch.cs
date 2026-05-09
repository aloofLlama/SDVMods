using SDVCommon.Models.Runtime;
using SDVCommon.OBSGift;
using StardewValley;
using HarmonyLib;
using StardewModdingAPI;


namespace HarvestHelper.Compatibility
{

    public static class GiftPatch
    {
        public static void Postfix(
            NPC __instance,
            StardewValley.Object o,
            Farmer giver,
            bool updateGiftLimitInfo,
            float friendshipChangeMultiplier,
            bool showResponse
        )
        {
            if (o is null)
                return;

            string npcName = __instance.Name;
            string itemId = o.QualifiedItemId;
            string itemName = o.DisplayName;

            int tasteValue = __instance.getGiftTasteForThisItem(o);
            GiftTaste taste = (GiftTaste)tasteValue;

            ModEntry.Instance.Monitor.Log(
                $"[HH] Learned taste: {npcName} → {itemName} = {taste} | {itemId}",
                LogLevel.Debug
                );


            GiftKnowledgeServiceOLD.LearnTaste(itemId, npcName, taste);
        }
    }
}


