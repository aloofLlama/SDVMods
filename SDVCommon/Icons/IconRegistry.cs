using SDVCommon.Icons;
using SDVCommon.Helpers;

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
        id = IdHelper.ToQualifiedId(id);

        if (Cache.TryGetValue(id, out var cached))
            return cached;

        foreach (var provider in Providers)
        {
            if (provider.CanHandle(id))
            {
                var icon = provider.LoadIcon(id);
                Cache[id] = icon;
                return icon;
            }
        }

        Cache[id] = null;
        return null;
    }
}