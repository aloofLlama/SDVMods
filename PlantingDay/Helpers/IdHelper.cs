using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlantingDay.Helpers
{
    public static class IdHelper
    {
        public static string NormalizeItemId(string raw)
        {
            if (string.IsNullOrWhiteSpace(raw))
                return "";

            raw = raw.Trim();

            // Convert (O)### → O:###
            if (raw.StartsWith("(") && raw.Contains(')'))
            {
                int close = raw.IndexOf(')');
                string prefix = raw.Substring(1, close - 1);
                string number = raw.Substring(close + 1);
                return $"{prefix}:{number}";
            }

            // Already qualified (O:###, S:###, modded IDs)
            if (raw.Contains(":"))
                return raw;

            // Numeric → assume O:###
            if (int.TryParse(raw, out _))
                return $"O:{raw}";

            // Modded string IDs (Cornucopia_*)
            return raw;
        }

        public static string ExtractNumericId(string id)
        {
            // "(O)638" → "638"
            // "O:638"  → "638"
            // "638"    → "638"
            return new string(id.Where(char.IsDigit).ToArray());
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
