using GiftDiscovery.Compatibility;
using SDVCommon.Helpers;
using StardewValley;


namespace GiftDiscovery.GameData
{
    internal class GiftableNPC
    {
        /// NPCs that can receive gifts at some point in the game. 
        /// NPCs may be currently available or not (e.g. Leo/Sandy)
        public static IEnumerable<NPC> GetAllGiftableNPCs()
        {
            //SDVCommonLog.Log($"start", LogHelper.DebugAlert);

            return Utility.getAllCharacters()
                .OfType<NPC>()
                .Where(npc =>
                {
                    string name = npc.Name;

                    // must have canonical gift taste data
                    if (!Game1.NPCGiftTastes.ContainsKey(name))
                        return false;

                    // skip explicitly non-giftable NPCs
                    if (ModCompat.GiftOverrides.NonGiftableNPCs.Contains(name))
                        return false;

                    //SDVCommonLog.Log($"{name}",LogHelper.DebugOrTrace);

                    return true;
                });
        }
    }
}
