using System.Collections.Generic;

namespace System.Linq
{
    public static class SelectExtensions
    {
        public static List<TResult> SelectList<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector) =>
            source.Select(selector).ToList();
        public static List<TResult> SelectList<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, int, TResult> selector) =>
            source.Select(selector).ToList();
        public static List<TResult> SelectListMany<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, int, IEnumerable<TResult>> selector) =>
            source.SelectMany(selector).ToList();
        public static List<TResult> SelectListMany<TSource, TCollection, TResult>(this IEnumerable<TSource> source, Func<TSource, int, IEnumerable<TCollection>> collectionSelector, Func<TSource, TCollection, TResult> resultSelector) =>
            source.SelectMany(collectionSelector, resultSelector).ToList();
        public static List<TResult> SelectListMany<TSource, TCollection, TResult>(this IEnumerable<TSource> source, Func<TSource, IEnumerable<TCollection>> collectionSelector, Func<TSource, TCollection, TResult> resultSelector) =>
            source.SelectMany(collectionSelector, resultSelector).ToList();
        public static List<TResult> SelectListMany<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, IEnumerable<TResult>> selector) =>
            source.SelectMany(selector).ToList();
    }
}