
namespace SDVCommon.Helpers
{
    public static class IdHelper
    {
        //Removes the (O) prefix from seed/harvest Ids
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
            
        }

        public static string ToQualifiedId(string id)
        {
            if (string.IsNullOrEmpty(id))
                return id;

            if (id.StartsWith("(O)"))
                return id;

            return "(O)" + id;
        }
    }
}
