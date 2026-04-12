using System.Collections.Generic;
using StardewModdingAPI;
using SDVCommon.Icons.IconProviders;


namespace SDVCommon.Icons
{
    internal static class IconRegistry
    {
        private static readonly Dictionary<string, Icon?> Cache = new();
        private static readonly List<IIconProvider> Providers = new();

        static IconRegistry()
        {
            Providers.Add(new SeedIconProvider());
            Providers.Add(new HarvestIconProvider());
            Providers.Add(new MonsterIconProvider());
            Providers.Add(new PurchaseIconProvider());

            // Later:
            // Providers.Add(new ArtisanGoodIconProvider());
            // Providers.Add(new MachineIconProvider());
            // Providers.Add(new ForageIconProvider());
            // Providers.Add(new FishIconProvider());
        }

        public static Icon? GetIcon(string id)
        {            
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
}
