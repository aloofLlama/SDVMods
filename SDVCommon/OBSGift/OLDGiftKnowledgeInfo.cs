using System;
using System.Collections.Generic;
using System.Text;

namespace SDVCommon.OBSGift
{
    public class GiftKnowledgeInfoOLD
    {
        public string ItemId { get; }
        public Dictionary<string, string> KnownTastes { get; }

        public GiftKnowledgeInfoOLD(string itemId, Dictionary<string, string>? known)
        {
            ItemId = itemId;
            KnownTastes = known ?? new Dictionary<string, string>();
        }

        public bool IsKnown(string npcName)
            => KnownTastes.ContainsKey(npcName);

        public string? GetTaste(string npcName)
            => KnownTastes.TryGetValue(npcName, out var taste) ? taste : null;
    }
}



