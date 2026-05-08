using System;
using System.Collections.Generic;
using System.Text;

namespace GiftDiscovery.Models
{
    public class GiftKnowledgeInfo
    {
        public string ItemId { get; }
        public Dictionary<string, string> GlobalKnownTastes { get; }
        public Dictionary<string, string> LocalKnownTastes { get; }

        public GiftKnowledgeInfo(
            string itemId,
            Dictionary<string, string>? global,
            Dictionary<string, string>? local)
        {
            ItemId = itemId;
            GlobalKnownTastes = global ?? new Dictionary<string, string>();
            LocalKnownTastes = local ?? new Dictionary<string, string>();
        }
        public bool IsKnownGlobally(string npcName)
            => GlobalKnownTastes.ContainsKey(npcName);

        public bool IsKnownLocally(string npcName)
            => LocalKnownTastes.ContainsKey(npcName);

        public string? GetGlobalTaste(string npcName)
            => GlobalKnownTastes.TryGetValue(npcName, out var t) ? t : null;

        public string? GetLocalTaste(string npcName)
            => LocalKnownTastes.TryGetValue(npcName, out var t) ? t : null;
    }
}



