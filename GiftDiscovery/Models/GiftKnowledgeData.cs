namespace GiftDiscovery.Models
{
    public class GiftKnowledgeData
    {
        // QualifiedItemId → NPCName → GiftTaste
        public Dictionary<string, Dictionary<string, string>> KnownTastes { get; set; }
            = new();
    }
}
