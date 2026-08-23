
using Microsoft.Xna.Framework;
using Newtonsoft.Json.Linq;
using SDVCommon.GameData;
using System.Reflection.PortableExecutable;

namespace SDVCommon.Helpers
{
    public static class NameHelper
    {
        //Gets the localized display name for (O) objects
        public static string GetObjectName(string? itemId)
        {
            if (string.IsNullOrEmpty(itemId))
                return string.Empty;

            var objInfo = GameObject.GetObjectInfo(itemId);

            if (objInfo != null)
                return objInfo.DisplayName;

            return itemId;

        }

        //Gets the localized display name for (BC) objects
        public static string GetBigCraftableName(string? itemId)
        {
            if (string.IsNullOrEmpty(itemId))
                return string.Empty;

            var itemInfo = new StardewValley.Object(Vector2.Zero, itemId);

            if (itemInfo != null)
                return itemInfo.DisplayName;

            return itemId;

        }


    }
}