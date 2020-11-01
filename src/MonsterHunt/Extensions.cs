using System;
using System.Collections.Generic;

namespace MonsterHunt
{
    internal static class Extensions
    {
        internal static T Next<T>(this IEnumerator<T> enumerator)
        {
            var hasNext = enumerator.MoveNext();

            if (!hasNext)
            {
                return default;
            }

            return enumerator.Current;
        }

        internal static bool AreEqualIgnoreCase(this string value1, string value2)
        {
            return value1.Equals(value2, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}