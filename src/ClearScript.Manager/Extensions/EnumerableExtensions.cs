using System.Collections.Generic;

namespace ClearScript.Manager.Extensions
{
    internal static class EnumerableExtensions
    {
        internal static bool SafeAny<T>(this IList<T> list)
        {
            return list != null && list.Count > 0;
        }
    }
}