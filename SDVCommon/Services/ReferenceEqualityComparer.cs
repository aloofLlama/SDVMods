using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace SDVCommon.Services
{

    public sealed class ReferenceEqualityComparer<T> : IEqualityComparer<T>
    where T : class
    {
        public static readonly ReferenceEqualityComparer<T> Instance = new();

        public bool Equals(T? x, T? y)
        {
            return ReferenceEquals(x, y);
        }

        public int GetHashCode(T obj)
        {
            // Uses object identity, not value-based GetHashCode
            return RuntimeHelpers.GetHashCode(obj);
        }
    }
}
