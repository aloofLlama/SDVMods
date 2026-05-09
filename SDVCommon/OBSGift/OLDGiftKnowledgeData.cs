namespace SDVCommon.OBSGift
{
    public class GiftKnowledgeDataOLD
    {
        // ItemId → NPCName → GiftTaste
        public Dictionary<string, Dictionary<string, string>> KnownTastes { get; set; }
            = new();
    }
}
