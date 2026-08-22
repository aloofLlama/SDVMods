using GiftDiscovery.Compatibility;
using SDVCommon;
using SDVCommon.Helpers;
using SDVCommon.Services;
using StardewModdingAPI;
using StardewValley;


namespace GiftDiscovery.GameData
{
    internal class GiftableNPC
    {
        private static List<NPC>? GiftableNPCs;

        /// NPCs that can receive gifts at some point in the game. 
        /// NPCs may be currently available or not (e.g. Leo/Sandy)
        public static List<NPC> GetAllGiftableNPCs()
        {
            if (GiftableNPCs == null)
                GiftableNPCs = BuildGiftableNPCList();

            return GiftableNPCs;
        }

        public static void Reset()
        {
            GiftableNPCs = null;
        }


        private static List<NPC> BuildGiftableNPCList()
        {
            string timer = "Build Giftable NPC List";
            LogLevel logLevel = LogHelper.DebugOrTrace;
            SDVCommonServices.PerfBegin(timer);

            var list = new List<NPC>();

            foreach (var npc in Utility.getAllCharacters().OfType<NPC>())
            {
                string name = npc.Name;

                if (!Game1.NPCGiftTastes.ContainsKey(name))
                    continue;

                if (ModCompat.GiftOverrides.NonGiftableNPCs.Contains(name))
                    continue;

                list.Add(npc);
            }

            SDVCommonServices.PerfEnd(timer, $"Giftable NPCs: {list.Count}", 0, logLevel);

            return list;
        }


    }
}
