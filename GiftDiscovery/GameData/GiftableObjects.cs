using GiftDiscovery.Models;
using GiftDiscovery.Helpers;
using SDVCommon.Models.Wrappers;
using StardewModdingAPI;
using StardewValley;


namespace GiftDiscovery.GameData
{
    public static class GiftableObjectList
    {
        private static bool _isInitialized;

        public static readonly List<StardewValley.Object> AllGiftable = new();
        public static readonly HashSet<string> GiftableIds = new();

        public static void Initialize()
        {
            if (_isInitialized)
                return;

            foreach (var (id, data) in Game1.objectData)
            {
                var obj = ItemRegistry.Create(id) as StardewValley.Object;
                if (obj == null)
                    continue;

                if (!obj.canBeGivenAsGift())
                    continue;

                // Explicit exclusions
                if (id == "434") // Stardrop is a game object but not a real item to get
                    continue;

                bool hasLoveOrLike = GiftableNPC.GetAllGiftableNPCs()
                    .Any(npc =>
                    {
                        try
                        {
                            GiftTaste t = (GiftTaste)npc.getGiftTasteForThisItem(obj);
                            return t == GiftTaste.Love || t == GiftTaste.Like;
                        }
                        catch
                        {
                            return false;
                        }
                    });

                if (!hasLoveOrLike)
                    continue;

                // Add to list
                AllGiftable.Add(obj);

                // Add to HashSet
                GiftableIds.Add(obj.QualifiedItemId);
            }
        }
        public static void Reset()
        {
            _isInitialized = false;
            AllGiftable.Clear();
            GiftableIds.Clear();
        }


        public static bool IsGiftableObject(StardewValley.Object obj)
        {
            return GiftableObjectList.AllGiftable
                .Any(o => o.QualifiedItemId == obj.QualifiedItemId);
        }

    }
}
