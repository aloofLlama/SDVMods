using StardewValley;
using System;
using System.Collections.Generic;
using System.Text;

namespace SDVCommon.GameData
{
    internal class DisplayNames
    {
        private static readonly Dictionary<string, string> _cache = new();

        public static string GetDisplayName(string itemId)
        {
            if (_cache.TryGetValue(itemId, out var name))
                return name;

            var item = ItemRegistry.Create(itemId);
            name = item?.DisplayName ?? itemId;

            _cache[itemId] = name;
            return name;
        }
    }
}
