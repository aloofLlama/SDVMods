using Newtonsoft.Json.Linq;
using StardewModdingAPI;

namespace SDVCommon.Compatibility
{
    public static class CustomBushCompat
    {
        public static ICustomBushApi? Api { get; internal set; }

        private const string DataAsset = "furyx639.CustomBush/Data";

        //public static JObject? LoadRawJson(IContentHelper content)
        //{
        //    try
        //    {
        //        return content.Load<JObject>(DataAsset);
        //    }
        //    catch
        //    {
        //        return null;
        //    }
        //}
    }
}
