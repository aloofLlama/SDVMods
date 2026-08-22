//using GiftDiscovery.Models;
//using GiftDiscovery.GameData;
//using StardewValley;

//namespace GiftDiscovery.Services
//{
//    internal static class OBSTasteDiscovered
//    {

//        // ---------------------------------------------------------
//        // MODE-FILTERED KNOWN / UNKNOWN
//        // ---------------------------------------------------------
//        //public static bool IsKnown(string itemId, NPC npc, TasteSourceMode mode)
//        //{
//        //    switch (mode)
//        //    {
//        //        case TasteSourceMode.All:
//        //            return GiftKnowledgeService.GetTasteForNPCItemPair(itemId, npc) != null;

//        //        case TasteSourceMode.Global:
//        //            return TasteLearning.IsKnownGlobal(itemId, npc);

//        //        case TasteSourceMode.Local:
//        //            return TasteLearning.IsKnownLocal(itemId, npc);

//        //        default:
//        //            return false;
//        //    }
//        //}

//        //public static bool IsUnknown(string itemId, NPC npc, TasteSourceMode mode)
//        //{
//        //    return !IsKnown(itemId, npc, mode);
//        //}

//        //public static bool IsKnownGlobal(string itemId, NPC npc)
//        //{
//        //    return _globalData.KnownTastes.TryGetValue(itemId, out var npcDict)
//        //        && npcDict.ContainsKey(npc.Name);
//        //}

//        //public static bool IsKnownLocal(string itemId, NPC npc)
//        //{
//        //    return _localData.KnownTastes.TryGetValue(itemId, out var npcDict)
//        //        && npcDict.ContainsKey(npc.Name);
//        //}



//        //public static GiftTaste? GetLearnedTasteGlobal(string itemId, NPC npc)
//        //{
//        //    if (TryGetGlobalKnownTaste(itemId, npc.Name, out var t))
//        //        return t;

//        //    return null;
//        //}

//        //public static GiftTaste? GetLearnedTasteLocal(string itemId, NPC npc)
//        //{
//        //    if (TryGetLocalKnownTaste(itemId, npc.Name, out var t))
//        //        return t;

//        //    return null;
//        //}

//        //private static bool TryGetGlobalKnownTaste(string qualifiedItemId, string npcName, out GiftTaste? taste)
//        //{
//        //    taste = null;

//        //    if (_globalData.KnownTastes.TryGetValue(qualifiedItemId, out var npcDict) &&
//        //        npcDict.TryGetValue(npcName, out var s) &&
//        //        Enum.TryParse<GiftTaste>(s, out var parsed))
//        //    {
//        //        taste = parsed;
//        //        return true;
//        //    }

//        //    return false;
//        //}

//        //private static bool TryGetLocalKnownTaste(string qualifiedItemId, string npcName, out GiftTaste? taste)
//        //{
//        //    taste = null;

//        //    if (_localData.KnownTastes.TryGetValue(qualifiedItemId, out var npcDict) &&
//        //        npcDict.TryGetValue(npcName, out var s) &&
//        //        Enum.TryParse<GiftTaste>(s, out var parsed))
//        //    {
//        //        taste = parsed;
//        //        return true;
//        //    }

//        //    return false;
//        //}

//    }
//}
