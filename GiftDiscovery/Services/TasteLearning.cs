using GiftDiscovery.GameData;
using GiftDiscovery.Models;
using SDVCommon;
using StardewModdingAPI;
using StardewValley;

namespace GiftDiscovery.Services
{
    // Learn and save gift tastes
    // Load learned gift tastes for use
    internal class TasteLearning
    {
        // Global is available for all farm files
        private const string GlobalDataKey = "GiftKnowledge";
        private static GiftKnowledgeData _globalData = null!;

        // Local is specific to each farm file
        private const string LocalDataKey = "LocalGiftKnowledge";
        private static GiftKnowledgeData _localData = null!;

        private static IModHelper _helper = null!;
        public static int GiftVersion = 0; //used for cache update

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


        public static void Initialize(IModHelper helper)
        {
            InitializeGlobal(helper);
            InitializeLocal(helper);
        }

        public static void Reset()
        {
            _globalData = null!;
            _localData = null!;
            GiftVersion++;
        }


        private static void InitializeGlobal(IModHelper helper)
        {
            _helper = helper;

            string timer = "Initializing Global Data";
            SDVCommonServices.PerfBegin(timer);

            _globalData = helper.Data.ReadGlobalData<GiftKnowledgeData>(GlobalDataKey)
                    ?? new GiftKnowledgeData();

            SDVCommonServices.PerfEnd(timer, 10);

        }

        private static void InitializeLocal(IModHelper helper)
        {
            _helper = helper;

            string timer = "Initializing Local Data";
            SDVCommonServices.PerfBegin(timer);

            _localData = helper.Data.ReadSaveData<GiftKnowledgeData>(LocalDataKey)
                    ?? new GiftKnowledgeData();

            SDVCommonServices.PerfEnd(timer, 10);

        }

        public static void LearnTaste(string qId, string npcName, GiftTaste taste)
        {
            if (!_globalData.KnownTastes.TryGetValue(qId, out var npcDict))
            {
                npcDict = new Dictionary<string, string>();
                _globalData.KnownTastes[qId] = npcDict;
            }

            if (!_localData.KnownTastes.TryGetValue(qId, out var localNPCDict))
            {
                localNPCDict = new Dictionary<string, string>();
                _localData.KnownTastes[qId] = localNPCDict;
            }
            localNPCDict[npcName] = taste.ToString();

            npcDict[npcName] = taste.ToString();

            Save(); //saves both global and local data
            GiftVersion++;

        }

        private static void Save()
        {
            _helper.Data.WriteGlobalData(GlobalDataKey, _globalData);
            _helper.Data.WriteSaveData(LocalDataKey, _localData);
        }

    }
}
