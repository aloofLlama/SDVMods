using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDVCommon.OBSGift
{
    public static class ModCompat
    {
        public static class GiftOverrides
        {
            /// <summary>
            /// NPCs that should be treated as NOT giftable,
            /// even if they appear in Utility.getAllCharacters().
            /// </summary>
            public static readonly HashSet<string> NonGiftableNPCs = new()
            {
                // Sunberry Valley placeholders in game data but not used
                "NadiaSBV",
                "PanSBV"

            };
        }

    }
}
