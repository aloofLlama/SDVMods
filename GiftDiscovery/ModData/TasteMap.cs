using GiftDiscovery.GameData.Static;
using GiftDiscovery.Models;
using SDVCommon;
using SDVCommon.Services;
using StardewValley;

namespace GiftDiscovery.ModData
{
    internal class TasteMap
    {
        private static Dictionary<string, Dictionary<string, GiftTaste>>? _tasteMap;
        private static bool _isInitialized;

        public static GiftTaste? GetTasteForNPCItemPair(string qId, NPC npc)
        {
            EnsureInitialized();

            if (_tasteMap!.TryGetValue(npc.Name, out var npcMap) &&
                npcMap.TryGetValue(qId, out var taste))
            {
                return taste;
            }

            return null;
        }

        //------------------------------------------------
        // Data lifecycle methods
        //------------------------------------------------
        private static void EnsureInitialized()
        {
            if (!_isInitialized)
                Initialize();
        }

        internal static void Initialize()
        {
            Build();
            _isInitialized = true;
        }

        internal static void Reset()
        {
            _tasteMap = null;
            _isInitialized = false;
        }


        //------------------------------------------------
        // Builder
        //------------------------------------------------
        private static void Build()
        {
            string timer = "Build Taste Maps";
            SDVCommonServices.PerfBegin(timer);

            _tasteMap = new Dictionary<string, Dictionary<string, GiftTaste>>();

            foreach (var npc in NPCGiftStatus.GetAllGiftableNPCs())
            {
                var map = BuildIndividualTasteMap(npc);
                _tasteMap![npc.Name] = map;

            }

            SDVCommonServices.PerfEnd(timer, 0);

        }

        private static Dictionary<string, GiftTaste> BuildIndividualTasteMap(NPC npc)
        {
            string timer = "Taste Map";  // Logs {name} took {ms} ms
            SDVCommonServices.PerfBegin(timer);

            var map = new Dictionary<string, GiftTaste>();

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
