using StardewValley;

namespace GiftDiscovery.Models
{
    public class NPCGiftStatus
    {
        public NPC NPC { get; set; } = null!;
        public string Name { get; set; } = "";

        public bool IsGiftable { get; set; }
        public bool IsAvailable { get; set; }
        public bool IsUnavailable => IsGiftable && !IsAvailable;

        public bool IsMet { get; set; }
        public bool IsUnmet => IsAvailable && !IsMet;

        public bool CanGiftToday { get; set; }
        public bool IsMaxHeart { get; set; }
    }


}
