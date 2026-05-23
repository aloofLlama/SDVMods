using GiftDiscovery.GameData;
using GiftDiscovery.Helpers;
using GiftDiscovery.Models;
using SDVCommon.Helpers;
using SDVCommon.Models.Builders;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Characters;

namespace GiftDiscovery.Services
{
    // This service manages the knowledge of gift tastes for NPCs and items.
    // Saves the learned information
    // Reads the information for use
    public static class GiftKnowledgeService
    {
        private static readonly Dictionary<string, Dictionary<string, GiftTaste>> CanonicalTasteCache
    = new();

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

        public static void ResetCanonicalTasteCache()
        {
            CanonicalTasteCache.Clear();
            GiftVersion++;
        }


        public static void LearnTaste(string itemId, string npcName, GiftTaste taste)
        {
            if (!_globalData.KnownTastes.TryGetValue(itemId, out var npcDict))
            {
                npcDict = new Dictionary<string, string>();
                _globalData.KnownTastes[itemId] = npcDict;
            }

            if (!_localData.KnownTastes.TryGetValue(itemId, out var localNPCDict))
            {
                localNPCDict = new Dictionary<string, string>();
                _localData.KnownTastes[itemId] = localNPCDict;
            }
            localNPCDict[npcName] = taste.ToString();

            npcDict[npcName] = taste.ToString();

            Save(); //saves both global and local data
            GiftVersion++;

        }

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

        public static GiftTaste? GetCanonicalTasteForItem(string qualifiedItemId, NPC npc)
        {
            var obj = GiftableObjectList.AllGiftable
                .FirstOrDefault(o => o.QualifiedItemId == qualifiedItemId);

            if (obj is null)
                return null;

            try
            {
                return (GiftTaste)npc.getGiftTasteForThisItem(obj);
            }
            catch
            {
                return null;
            }
        }

        public static Dictionary<string, GiftTaste> GetCanonicalTasteMap(NPC npc)
        {
            string name = npc.Name;

            if (!CanonicalTasteCache.TryGetValue(name, out var map))
            {
                map = BuildCanonicalTasteMap(npc);
                CanonicalTasteCache[name] = map;
            }
            return map;
        }

        private static Dictionary<string, GiftTaste> BuildCanonicalTasteMap(NPC npc)
        {
            var map = new Dictionary<string, GiftTaste>();

            foreach (var obj in GiftableObjectList.AllGiftable)
            {
                try
                {
                    GiftTaste t = (GiftTaste)npc.getGiftTasteForThisItem(obj);
                    map[obj.QualifiedItemId] = t;
                }
                catch
                {
                    SDVCommonLog.Log($"Missing Gift Info: {npc.displayName} | {obj.DisplayName}",
                        LogLevel.Warn);
                }
            }

            return map;
        }


    }
}
