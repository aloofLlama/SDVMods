using StardewValley;

namespace GiftDiscovery.Models
{
    internal class NPCGiftStatusData
    {
        internal NPC NPC { get; set; } = null!;
        internal string Name { get; set; } = "";

        internal bool IsAvailable { get; set; }
        internal bool IsMet { get; set; }
        internal bool IsUnmet => IsAvailable && !IsMet;

        internal bool CanGiftToday { get; set; }
        internal bool IsMaxHeart { get; set; }
    }

}
