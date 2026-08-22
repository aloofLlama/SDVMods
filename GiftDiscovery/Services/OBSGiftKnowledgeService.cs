//using GiftDiscovery.GameData;
//using GiftDiscovery.Models;
//using SDVCommon;
//using SDVCommon.Services;
//using StardewModdingAPI;
//using StardewValley;

//namespace GiftDiscovery.Services
//{
//    // This service manages the knowledge of gift tastes for NPCs and items.

//    public static class OBSGiftKnowledgeService
//    {
//        //public static GiftTaste? GetLearnedTasteGlobal(string itemId, NPC npc)
//        //{
//        //    if (GiftKnowledgeService.TryGetGlobalKnownTaste(itemId, npc.Name, out var t))
//        //        return t;

//        //    return null;
//        //}

//        //public static GiftTaste? GetLearnedTasteLocal(string itemId, NPC npc)
//        //{
//        //    if (GiftKnowledgeService.TryGetLocalKnownTaste(itemId, npc.Name, out var t))
//        //        return t;

//        //    return null;
//        //}



//        public static GiftTaste? GetTasteForNPCItemPair(string qualifiedItemId, NPC npc)
//        {
//            var map = TasteMap.GetTasteMap(npc);

//            if (map.TryGetValue(qualifiedItemId, out var taste))
//                return taste;

//            return null;
//        }

//        //public static Dictionary<string, GiftTaste> GetCanonicalTasteMap(NPC npc)
//        //{
//        //    string name = npc.Name;

//        //    if (!CanonicalTasteCache.TryGetValue(name, out var map))
//        //    {
//        //        map = BuildCanonicalTasteMap(npc);
//        //        CanonicalTasteCache[name] = map;
//        //    }
//        //    return map;
//        //}

//        //private static Dictionary<string, GiftTaste> BuildCanonicalTasteMap(NPC npc)
//        //{
//        //    var map = new Dictionary<string, GiftTaste>();

//        //    string timer = "Taste Map";  // Logs {name} took {ms} ms
//        //    SDVCommonServices.PerfBegin(timer);

//        //    foreach (var obj in GiftableObjectList.GetAllGiftableObjects())
//        //    {
//        //        try
//        //        {
//        //            GiftTaste t = (GiftTaste)npc.getGiftTasteForThisItem(obj);
//        //            map[obj.QualifiedItemId] = t;
//        //        }
//        //        catch
//        //        {
//        //            SDVCommonLog.Log($"Missing Gift Info: {npc.displayName} | {obj.DisplayName}",
//        //                LogHelper.Warn);
//        //        }
//        //    }

//        //        SDVCommonServices.PerfEnd(timer, $"{npc.displayName}", 100);

//        //    return map;
//        //}



//    }
//}
