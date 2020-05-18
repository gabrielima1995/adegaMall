using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace System.Linq
{
    public static class LinqExtensions
    {
        public static ReadOnlyCollection<TSource> ToReadOnly<TSource>(this IEnumerable<TSource> source) => source.ToList().AsReadOnly();
    }
}
