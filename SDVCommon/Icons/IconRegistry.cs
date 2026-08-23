using SDVCommon.Helpers;
using SDVCommon.Icons;
using System.Security.Cryptography;

internal static class IconRegistry
{
    private static readonly Dictionary<string, Icon?> Cache = new();
    private static readonly List<IIconProvider> Providers = new();

    static IconRegistry()
    {
        Providers.Add(new StaticIconProvider());
        Providers.Add(new ItemIconProvider());
    }

    public static Icon? GetIcon(string id)
    {
        // Accepts both qualified and unqualified IDs, as ItemRegistry can resolve either
        string qId = IdHelper.ToQualifiedId(id);

        if (Cache.TryGetValue(qId, out var cached))
            return cached;

        foreach (var provider in Providers)
        {
            if (provider.CanHandle(qId))
            {
                var icon = provider.LoadIcon(qId);
                Cache[qId] = icon;
                return icon;
            }
        }

        Cache[qId] = null;
        return null;
    }
}