using GiftDiscovery.GameData;
using GiftDiscovery.Models;
using GiftDiscovery.Tooltip;
using SDVCommon;
using StardewModdingAPI;
using StardewValley;

namespace GiftDiscovery.Services
{
    // Learn and save gift tastes
    // Load learned gift tastes
    // Provides methods to return whether a taste is known for a given item and NPC
    // Actual gift/npc/taste data is stored in TasteMap, this class only tracks what the player has learned
    internal class TasteLearning
    {
        private class GiftKnowledgeData
        {
            // QualifiedItemId → NPCName → GiftTaste
            public Dictionary<string, Dictionary<string, string>> KnownTastes { get; set; }
                = new();
        }

        // Global is available for all farm files
        private const string GlobalDataKey = "GiftKnowledge";
        private static GiftKnowledgeData _globalData = null!;

        // Local is specific to each farm file
        private const string LocalDataKey = "LocalGiftKnowledge";
        private static GiftKnowledgeData _localData = null!;

        private static IModHelper _helper = null!;


        public static bool IsKnownGlobal(string qId, NPC npc)
        {
            return _globalData.KnownTastes.TryGetValue(qId, out var npcDict)
                && npcDict.ContainsKey(npc.Name);
        }

        public static bool IsKnownLocal(string qId, NPC npc)
        {
            return _localData.KnownTastes.TryGetValue(qId, out var npcDict)
                && npcDict.ContainsKey(npc.Name);
        }

        //------------------------------------------------
        // Learn and save taste
        //------------------------------------------------
        public static void LearnTaste(string qId, string npcName, GiftTaste taste)
        {
            if (!_globalData.KnownTastes.TryGetValue(qId, out var globalNPCDict))
            {
                globalNPCDict = new Dictionary<string, string>();
                _globalData.KnownTastes[qId] = globalNPCDict;
            }

            if (!_localData.KnownTastes.TryGetValue(qId, out var localNPCDict))
            {
                localNPCDict = new Dictionary<string, string>();
                _localData.KnownTastes[qId] = localNPCDict;
            }

            globalNPCDict[npcName] = taste.ToString();
            localNPCDict[npcName] = taste.ToString();

            Save();
            GiftTooltipBuilder.TooltipInvalidated = true;
            NPCGiftTooltipBuilder.TooltipInvalidated = true;
        }

        private static void Save()
        {
            _helper.Data.WriteGlobalData(GlobalDataKey, _globalData);
            _helper.Data.WriteSaveData(LocalDataKey, _localData);
        }


        //------------------------------------------------
        // Data lifecycle methods
        //------------------------------------------------

        internal static void Initialize(IModHelper helper)
        {
            _helper = helper;
            InitializeGlobal();
            InitializeLocal();
        }

        internal static void Reset()
        {
            _globalData = new GiftKnowledgeData();
            _localData = new GiftKnowledgeData();
        }


        private static void InitializeGlobal()
        {
            string timer = "Initialize Global Data";
            SDVCommonServices.PerfBegin(timer);

            _globalData = _helper.Data.ReadGlobalData<GiftKnowledgeData>(GlobalDataKey)
                    ?? new GiftKnowledgeData();

            SDVCommonServices.PerfEnd(timer, 0);
        }

        private static void InitializeLocal()
        {
            string timer = "Initialize Local Data";
            SDVCommonServices.PerfBegin(timer);

            _localData = _helper.Data.ReadSaveData<GiftKnowledgeData>(LocalDataKey)
                    ?? new GiftKnowledgeData();

            SDVCommonServices.PerfEnd(timer, 0);
        }
    }
}
