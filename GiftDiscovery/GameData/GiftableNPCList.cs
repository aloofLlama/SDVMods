using GiftDiscovery.Compatibility;
using SDVCommon;
using SDVCommon.Helpers;
using SDVCommon.Services;
using StardewModdingAPI;
using StardewValley;


namespace GiftDiscovery.GameData
{
    internal class GiftableNPCList
    {
        private static List<NPC>? _giftableNPCs;

        /// NPCs that can receive gifts at some point in the game. 
        /// NPCs may be currently available or not (e.g. Leo/Sandy)
        public static List<NPC> GetAllGiftableNPCs()
        {
            if (_giftableNPCs == null)
                _giftableNPCs = BuildGiftableNPCList();

            return _giftableNPCs;
        }
        public static bool IsGiftableNPC(NPC npc)
        {
            if (_giftableNPCs == null)
                BuildGiftableNPCList();

            return _giftableNPCs!.Contains(npc);
        }

        public static void Reset()
        {
            _giftableNPCs = null;
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
