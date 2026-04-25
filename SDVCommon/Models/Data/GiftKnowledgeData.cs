namespace SDVCommon.Models.Data
{
    public class GiftKnowledgeData
    {
        // ItemId → NPCName → GiftTaste
        public Dictionary<string, Dictionary<string, string>> KnownTastes { get; set; }
            = new();
    }
}
