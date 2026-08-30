using GiftDiscovery.Compatibility;
using GiftDiscovery.GameData.Dynamic;
using GiftDiscovery.Models;
using SDVCommon;
using SDVCommon.Services;
using StardewModdingAPI;
using StardewValley;

namespace GiftDiscovery.GameData.Static
{
    internal static class NPCGiftStatus
    {
        private static Dictionary<string, NPCGiftStatusData>? _npcGiftStatus;
        private static HashSet<NPC>? _giftableNPCs;
        private static bool _isInitialized;

        internal static NPCGiftStatusData GetNPCGiftStatus(NPC npc)
        {
            EnsureInitialized();
            return _npcGiftStatus![npc.Name];
        }

        internal static HashSet<NPC> GetAllGiftableNPCs()
        {
            EnsureInitialized();
            return _giftableNPCs!;
        }

        internal static bool IsGiftableNPC(NPC npc)
        {
            EnsureInitialized();
            return _giftableNPCs!.Contains(npc);
        }

        //------------------------------------------------
        // Data lifecycle methods
        //------------------------------------------------
        private static void EnsureInitialized()
        {
            if (!_isInitialized)
                Initialize();
        }

        internal static void Initialize()
        {
            Build();
            _isInitialized = true;
        }
        internal static void Reset()
        {
            _npcGiftStatus = null;
            _giftableNPCs = null;
            _isInitialized = false;
        }

        //------------------------------------------------
        // Builder
        //------------------------------------------------
        private static void Build()
        {
            string timer = "Build Giftable NPC List";
            LogLevel logLevel = LogHelper.DebugOrTrace;
            SDVCommonServices.PerfBegin(timer);

            _npcGiftStatus = new Dictionary<string, NPCGiftStatusData>();
            _giftableNPCs = new HashSet<NPC>();

            foreach (var npc in Utility.getAllCharacters().OfType<NPC>())
            {
                string name = npc.Name;

                if (!Game1.NPCGiftTastes.ContainsKey(name))
                    continue;

                if (ModCompat.GiftOverrides.NonGiftableNPCs.Contains(name))
                    continue;

                _giftableNPCs.Add(npc);

                // Build the gift status data for this NPC
                bool isAvailable =
                    npc.CanSocialize &&
                    npc.CanReceiveGifts() &&
                    npc.currentLocation != null;

                bool isMet =
                    isAvailable &&
                    Game1.player.friendshipData.ContainsKey(name);

                bool canGiftToday = isAvailable && !MaxGiftsReached(npc);
                bool isMaxHeart = isAvailable && isMet && HeartStatus.IsMaxHearts(npc);

                _npcGiftStatus[name] = new NPCGiftStatusData
                {
                    NPC = npc,
                    Name = name,
                    IsAvailable = isAvailable,
                    IsMet = isMet,
                    CanGiftToday = canGiftToday,
                    IsMaxHeart = isMaxHeart
                };

            }

            SDVCommonServices.PerfEnd(timer, $"Giftable NPCs: {_giftableNPCs.Count}", 0, logLevel);
        }

        private static bool MaxGiftsReached(NPC npc)
        {
            if (!Game1.player.friendshipData.TryGetValue(npc.Name, out var f))
                return false;

            // Daily limit (everyone)
            if (f.GiftsToday >= 1)
                return true;

            // Weekly limit bypass for spouse, roommate or birthday
            bool isMovedIn = f.IsMarried(); //roommate is also flagged as married
            bool isBirthday = npc.isBirthday();

            if (isMovedIn || isBirthday)
                return false;

            // Weekly limit (everyone else)
            if (f.GiftsThisWeek >= 2)
                return true;

            return false;
        }
    }
}
