using SDVCommon.Helpers;
using SDVCommon.Models.Data;
using SDVCommon.Models.Runtime;
using StardewModdingAPI;
using StardewValley;


namespace SDVCommon.Services
{
    public static class GiftKnowledgeService
    {
        private const string DataKey = "GiftKnowledge";
        private static GiftKnowledgeData _data = null!;
        private static IModHelper _helper = null!;

        public static void Initialize(IModHelper helper)
        {
            _helper = helper;
            _data = helper.Data.ReadGlobalData<GiftKnowledgeData>(DataKey)
                    ?? new GiftKnowledgeData();
        }

        public static void Save()
        {
            _helper.Data.WriteGlobalData(DataKey, _data);

        }

        // ⭐ Accept enum
        public static void LearnTaste(string itemId, string npcName, GiftTaste taste)
        {
            if (!_data.KnownTastes.TryGetValue(itemId, out var npcDict))
            {
                npcDict = new Dictionary<string, string>();
                _data.KnownTastes[itemId] = npcDict;
            }

            // ⭐ Store enum as string
            npcDict[npcName] = taste.ToString();
            Save();
        }

        // ⭐ Return enum instead of string
        public static bool TryGetKnownTaste(string itemId, string npcName, out GiftTaste? taste)
        {
            taste = null;

            if (_data.KnownTastes.TryGetValue(itemId, out var npcDict) &&
                npcDict.TryGetValue(npcName, out var s) &&
                Enum.TryParse<GiftTaste>(s, out var parsed))
            {
                taste = parsed;
                return true;
            }

            return false;
        }

        // ⭐ Return dictionary of enums
        public static Dictionary<string, GiftTaste>? GetKnownTastesForItem(string itemId)
        {
            if (!_data.KnownTastes.TryGetValue(itemId, out var npcDict))
                return null;

            return npcDict
                .Where(kvp => Enum.TryParse<GiftTaste>(kvp.Value, out _))
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => Enum.Parse<GiftTaste>(kvp.Value)
                );
        }
    }
}
