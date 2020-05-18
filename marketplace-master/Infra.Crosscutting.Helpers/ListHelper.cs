using System.Linq;

namespace System.Collections.Generic
{
    public static class ListHelper
    {
        public static IEnumerable<TSource> ExceptBy<TSource, TTarget>(
            this IEnumerable<TSource> source,
            IEnumerable<TTarget> target,
            Func<TSource, TTarget, bool> selector)
        {
            return source
                .Where(i => !target
                    .Any(j => selector(i, j)))
                .Distinct();
        }

        public static IList<T> ForEach<T>(this IList<T> list, Action<T> action)
        {
            foreach (var item in list)
            {
                action.Invoke(item);
            }

            return list;
        }

        public static IEnumerable<TSource> DistinctBy<TSource, TKey>
            (this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            var knownKeys = new HashSet<TKey>();

            foreach (var element in source)
            {
                if (knownKeys.Add(keySelector(element)))
                {
                    yield return element;
                }
            }
        }
    }
}
