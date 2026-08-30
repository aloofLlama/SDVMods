using GiftDiscovery.GameData;
using GiftDiscovery.Models;
using SDVCommon;
using SDVCommon.Services;
using StardewModdingAPI;
using StardewValley;

namespace GiftDiscovery.GameData
{
    internal class TasteMap
    {
        private static readonly Dictionary<string, Dictionary<string, GiftTaste>> _tasteMap = new();

        public static GiftTaste? GetTasteForNPCItemPair(string qualifiedItemId, NPC npc)
        {
            var map = GetTasteMap(npc);

            if (map.TryGetValue(qualifiedItemId, out var taste))
                return taste;

            return null;
        }

        public static void Initialize(IModHelper helper)
        {
            string timer = "Build Taste Map";
            LogLevel logLevel = LogHelper.DebugOrTrace;
            SDVCommonServices.PerfBegin(timer);

            foreach (var npc in NPCGiftStatus.GetAllGiftableNPCs())
            {
                GetTasteMap(npc);
            }

            SDVCommonServices.PerfEnd(timer, 0, logLevel);
        }

        public static void Reset()
        {
            _tasteMap.Clear();
        }

        public static Dictionary<string, GiftTaste> GetTasteMap(NPC npc)
        {
            string name = npc.Name;

            if (!_tasteMap.TryGetValue(name, out var map))
            {
                map = BuildTasteMap(npc);
                _tasteMap[name] = map;
            }
            return map;
        }

        private static Dictionary<string, GiftTaste> BuildTasteMap(NPC npc)
        {
            var map = new Dictionary<string, GiftTaste>();

            string timer = "Taste Map";  // Logs {name} took {ms} ms
            SDVCommonServices.PerfBegin(timer);

            foreach (var obj in GiftableObjectList.GetAllGiftableObjects())
            {
                try
                {
                    GiftTaste t = (GiftTaste)npc.getGiftTasteForThisItem(obj);
                    map[obj.QualifiedItemId] = t;
                }
                catch
                {
                    SDVCommonLog.Log($"Missing Gift Info: {npc.displayName} | {obj.DisplayName}",
                        LogHelper.Warn);
                }
            }

            SDVCommonServices.PerfEnd(timer, $"{npc.displayName}", 100);

            return map;
        }

    }
}
