using SDVCommon.Models;
using SDVCommon.Models.Runtime;
using SDVCommon.Services;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.GameData.Characters;

namespace SDVCommon.Helpers
{
    public static class GiftDetection
    {
        public static void Initialize(IModHelper helper)
        {
            // Hook into the input events needed for detection
            helper.Events.Input.ButtonPressed += OnButtonPressed;
            helper.Events.Input.ButtonReleased += OnButtonReleased;

            // Reset daily tracking
            helper.Events.GameLoop.DayStarted += OnDayStarted;
        }

        private static StardewValley.Object? _heldGift;
        private static uint _priorGiftsGiven;
        private static readonly Dictionary<string, int> _giftsGivenToday = new();

        private static void OnDayStarted(object? sender, DayStartedEventArgs e)
        {
            _priorGiftsGiven = Game1.stats.GiftsGiven;
            _heldGift = null;

            _giftsGivenToday.Clear();
            foreach (var pair in Game1.player.friendshipData.Pairs)
                _giftsGivenToday[pair.Key] = pair.Value.GiftsToday;
        }

        public static void OnButtonPressed(object? sender, ButtonPressedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            // Capture the held gift BEFORE the gift is processed
            if (Game1.player.ActiveObject != null &&
                Game1.player.ActiveObject.canBeGivenAsGift())
            {
                _heldGift = Game1.player.ActiveObject;
            }

            _priorGiftsGiven = Game1.stats.GiftsGiven;
        }

        public static void OnButtonReleased(object? sender, ButtonReleasedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            // Did a gift actually happen?
            if (Game1.stats.GiftsGiven == _priorGiftsGiven)
                return;

            if (_heldGift == null)
                return;

            SDVCommonLog.Log("[GIFT] Gift counter changed", LogLevel.Info);

            // Identify which NPC received the gift
            string? npcGivenTo = null;

            foreach (var pair in Game1.player.friendshipData.Pairs)
            {
                int giftsToday = pair.Value.GiftsToday;
                int previous = _giftsGivenToday.TryGetValue(pair.Key, out int prev) ? prev : 0;

                if (giftsToday != previous)
                {
                    npcGivenTo = pair.Key;
                    _giftsGivenToday[pair.Key] = giftsToday;
                    break;
                }
            }

            if (npcGivenTo != null)
            {
                SDVCommonLog.Log($"[GIFT] NPC detected: {npcGivenTo}", LogLevel.Info);

                NPC npc = Game1.getCharacterFromName(npcGivenTo);
                int tasteValue = npc.getGiftTasteForThisItem(_heldGift);
                GiftTaste taste = (GiftTaste)tasteValue;

                SDVCommonLog.Log(
                    $"[GIFT] Learned taste: {npcGivenTo} → {_heldGift.QualifiedItemId} = {taste}",
                    LogLevel.Alert
                );

                GiftKnowledgeService.LearnTaste(_heldGift.QualifiedItemId, npcGivenTo, taste);
            }

            _heldGift = null;
        }





    }
}
