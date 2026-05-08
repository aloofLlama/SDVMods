using GiftDiscovery.Models;
using StardewModdingAPI;
using StardewValley;
using System.Data;



namespace GiftDiscovery.Services
{
    public static class GiftKnowledgeService
    {
        // Global is available for all farm files
        private const string GlobalDataKey = "GiftKnowledge";
        private static GiftKnowledgeData _globalData = null!;

        // Local is specific to each farm file
        private const string LocalDataKey = "LocalGiftKnowledge";
        private static GiftKnowledgeData _localData = null!;

        private static IModHelper _helper = null!;

        public static void InitializeGlobal(IModHelper helper)
        {
            _helper = helper;

            _globalData = helper.Data.ReadGlobalData<GiftKnowledgeData>(GlobalDataKey)
                    ?? new GiftKnowledgeData();
        }

        public static void InitializeLocal(IModHelper helper)
        {
            _helper = helper;

            _localData = helper.Data.ReadSaveData<GiftKnowledgeData>(LocalDataKey)
                    ?? new GiftKnowledgeData();

        }


        public static void Save()
        {
            _helper.Data.WriteGlobalData(GlobalDataKey, _globalData);
            _helper.Data.WriteSaveData(LocalDataKey, _localData);

        }

        public static int GiftVersion = 0; //used for cache update

        // ⭐ Accept enum
        public static void LearnTaste(string itemId, string npcName, GiftTaste taste)
        {
            if (!_globalData.KnownTastes.TryGetValue(itemId, out var npcDict))
            {
                npcDict = new Dictionary<string, string>();
                _globalData.KnownTastes[itemId] = npcDict;
            }

            if (!_localData.KnownTastes.TryGetValue(itemId, out var localNpcDict))
            {
                localNpcDict = new Dictionary<string, string>();
                _localData.KnownTastes[itemId] = localNpcDict;
            }
            localNpcDict[npcName] = taste.ToString();

            // ⭐ Store enum as string
            npcDict[npcName] = taste.ToString();

            Save(); //saves both global and local data
            GiftVersion++;

        }

        // ⭐ Return enum instead of string
        public static bool TryGetGlobalKnownTaste(string itemId, string npcName, out GiftTaste? taste)
        {
            taste = null;

            if (_globalData.KnownTastes.TryGetValue(itemId, out var npcDict) &&
                npcDict.TryGetValue(npcName, out var s) &&
                Enum.TryParse<GiftTaste>(s, out var parsed))
            {
                taste = parsed;
                return true;
            }

            return false;
        }

        public static bool TryGetLocalKnownTaste(string itemId, string npcName, out GiftTaste? taste)
        {
            taste = null;

            if (_localData.KnownTastes.TryGetValue(itemId, out var npcDict) &&
                npcDict.TryGetValue(npcName, out var s) &&
                Enum.TryParse<GiftTaste>(s, out var parsed))
            {
                taste = parsed;
                return true;
            }

            return false;
        }


        // ⭐ Return dictionary of enums
        public static Dictionary<string, GiftTaste>? GetGlobalKnownTastesForItem(string itemId)
        {
            if (!_globalData.KnownTastes.TryGetValue(itemId, out var npcDict))
                return null;

            return npcDict
                .Where(kvp => Enum.TryParse<GiftTaste>(kvp.Value, out _))
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => Enum.Parse<GiftTaste>(kvp.Value)
                );
        }

        public static Dictionary<string, GiftTaste>? GetLocalKnownTastesForItem(string itemId)
        {
            if (!_localData.KnownTastes.TryGetValue(itemId, out var npcDict))
                return null;

            return npcDict
                .Where(kvp => Enum.TryParse<GiftTaste>(kvp.Value, out _))
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => Enum.Parse<GiftTaste>(kvp.Value)
                );
        }


        //public static GiftTasteInfo GetTasteInfo(string itemId, string npcName)
        //{
        //    GiftTaste canonical = GiftHelper.GetCanonicalTaste(npcName, itemId);

        //    bool global = TryGetKnownTaste(itemId, npcName, out var globalTaste);
        //    bool local = TryGetLocalTaste(itemId, npcName, out var localTaste);

        //    return new GiftTasteInfo
        //    {
        //        Canonical = canonical,
        //        LearnedGlobally = global,
        //        LearnedLocally = local
        //    };
        //}


    }
}
