using PlantingDay;
using SDVCommon.Icons.IconProviders;
using StardewModdingAPI;
using System.Collections.Generic;


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
            //ModEntry.Instance.Monitor.Log($"[IconRegistry] GetIcon called with id='{id}'", LogLevel.Warn);

            if (Cache.TryGetValue(id, out var cached))
                return cached;

            foreach (var provider in Providers)
            {

                if (provider.CanHandle(id))
                {
                    ModEntry.Instance.Monitor.Log($"[Can Handle] {id}", LogLevel.Warn);
                    var icon = provider.LoadIcon(id);
                    Cache[id] = icon;
                    return icon;
                }
                else
                    ModEntry.Instance.Monitor.Log($"[Can't Handle] {id}", LogLevel.Warn);

            }

            Cache[id] = null;
            return null;
        }
    }
}
