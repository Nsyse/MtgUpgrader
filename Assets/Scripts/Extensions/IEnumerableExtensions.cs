using System.Collections.Generic;
using System.Linq;

public static class IEnumerableExtensions
{
    public static bool IsEmpty<T>(this IEnumerable<T> list)
    {
        return !list.Any();
    }
}
