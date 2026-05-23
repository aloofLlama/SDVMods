using StardewValley;

namespace GiftDiscovery.Models
{
    public enum GiftTaste
    {
        Love = NPC.gift_taste_love,
        Like = NPC.gift_taste_like,
        Neutral = NPC.gift_taste_neutral,
        Dislike = NPC.gift_taste_dislike,
        Hate = NPC.gift_taste_hate,
        StardropTea = NPC.gift_taste_stardroptea,

    }

    public enum TasteSourceMode
    {
        All, //all tastes direct from game data, regardless of player knowledge
        Global, // tastes the player has learned globally across all farm files
        Local, // tastes the player has learned in the current farm file
    }

}
