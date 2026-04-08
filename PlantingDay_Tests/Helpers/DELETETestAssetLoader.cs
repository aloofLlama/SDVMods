//using StardewModdingAPI;
//using StardewModdingAPI.Framework.Content;

//public class TestAssetLoader : IAssetLoader
//{
//    private readonly Dictionary<string, object> _assets;

//    public TestAssetLoader(Dictionary<string, object> assets)
//    {
//        _assets = assets;
//    }

//    public bool CanLoad<T>(IAssetInfo asset)
//    {
//        return _assets.ContainsKey(asset.AssetName);
//    }

//    public T Load<T>(IAssetInfo asset)
//    {
//        return (T)_assets[asset.AssetName];
//    }
//}