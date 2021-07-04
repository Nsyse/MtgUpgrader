using System.Collections.Generic;
using System.Linq;

public static class HashSetExtensions
{
    public static void AddNewElementsFrom<T>(this HashSet<T> set, IEnumerable<T> list)
    {
        foreach (var element in list)
            set.Add(element);
    }
}