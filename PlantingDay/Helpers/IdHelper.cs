using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlantingDay.Helpers
{
    public static class IdHelper
    {
        //Seed and Harvest Ids are stored raw from game data:
        //e.g. 890, CarrotSeeds, Cornucopia_BasilSeeds, 889, Carrot, Cornucopia_Basil, (O)638 [for fruit tree fruit]
        //Shop data lists seeds as (O)890, Carrot Seeds, Cornucopia_BasilSeeds
        public static string CanonicalItemId(string? raw)
        {
            if (string.IsNullOrEmpty(raw))
                return string.Empty;

            // Vanilla object: (O)### → ###
            if (raw.StartsWith("(O)") && int.TryParse(raw.AsSpan(3), out int num))
                return num.ToString();

            // Modded object: (O)StringId → StringId
            if (raw.StartsWith("(O)"))
                return raw.Substring(3);

            // Already canonical (modded seeds, JA, DGA, etc.)
            return raw;
        }
        public static string ToGameId(string id)
        {
            // Modded IDs contain no digits → return raw
            if (!id.Any(char.IsDigit))
                return id;

            // Vanilla IDs → return numeric form
                return new string(id.Where(char.IsDigit).ToArray());
            ;
        }

        //public static string NormalizeItemId(string raw)
        //{
        //    if (string.IsNullOrWhiteSpace(raw))
        //        return "";

        //    raw = raw.Trim();

        //    // Strip (X)### → ###
        //    if (raw.StartsWith("(") && raw.Contains(')'))
        //    {
        //        int close = raw.IndexOf(')');
        //        return raw.Substring(close + 1);
        //    }

        //    // Strip X:### → ###
        //    int colonIndex = raw.IndexOf(':');
        //    if (colonIndex > 0)
        //        return raw.Substring(colonIndex + 1);

        //    // Already numeric
        //    if (int.TryParse(raw, out _))
        //        return raw;

        //    return raw;
        //}
    }


}
