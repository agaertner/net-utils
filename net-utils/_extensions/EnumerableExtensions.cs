namespace agaertner.NetUtils
{
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Indicates whether the specified enumerable is null or empty (ie. contains any elements).
        /// </summary>
        /// <typeparam name="T">Type of the items.</typeparam>
        /// <param name="source">The target enumerable.</param>
        /// <returns><see langword="True"/> if the enumerable is null or empty; Otherwise <see langword="false"/>.</returns>
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> source)
        {
            return !source?.Any() ?? true;
        }

        /// <summary>
        /// Returns the item with the maximum value given by a specified selector using the default equality comparer.
        /// </summary>
        /// <typeparam name="TSource">Type of the items.</typeparam>
        /// <typeparam name="TKey">Type of property to compare.</typeparam>
        /// <param name="source">The target enumerable.</param>
        /// <param name="selector">Selector function.</param>
        /// <returns>The item with the maximum value.</returns>
        public static TSource MaxBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector)
        {
            return source.MaxBy(selector, null!);
        }

        /// <summary>
        /// Returns the item with the maximum value given by a specified selector using a specified equality comparer.
        /// </summary>
        /// <typeparam name="TSource">Type of the items.</typeparam>
        /// <typeparam name="TKey">Type of property to compare.</typeparam>
        /// <param name="source">The target enumerable.</param>
        /// <param name="selector">Selector function.</param>
        /// <param name="comparer">Custom equality comparer.</param>
        /// <returns>The item with the maximum value.</returns>
        public static TSource MaxBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector, IComparer<TKey> comparer)
        {
            return source.MostBy(selector, comparer, true);
        }

        /// <summary>
        /// Returns the item with the minimum value given by a specified selector using the default equality comparer.
        /// </summary>
        /// <typeparam name="TSource">Type of the items.</typeparam>
        /// <typeparam name="TKey">Type of property to compare.</typeparam>
        /// <param name="source">The target enumerable.</param>
        /// <param name="selector">Selector function.</param>
        /// <returns>The item with the minimum value.</returns>
        public static TSource MinBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector)
        {
            return source.MinBy(selector, null!);
        }

        /// <summary>
        /// Returns the item with the minimum value given by a specified selector using a specified equality comparer.
        /// </summary>
        /// <typeparam name="TSource">Type of the items.</typeparam>
        /// <typeparam name="TKey">Type of property to compare.</typeparam>
        /// <param name="source">The target enumerable.</param>
        /// <param name="selector">Selector function.</param>
        /// <param name="comparer">Custom equality comparer.</param>
        /// <returns>The item with the minimum value.</returns>
        public static TSource MinBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector, IComparer<TKey> comparer)
        {
            return source.MostBy(selector, comparer, false);
        }

        private static TSource MostBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector, IComparer<TKey> comparer, bool max)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            if (selector == null)
            {
                throw new ArgumentNullException(nameof(selector));
            }
            comparer ??= Comparer<TKey>.Default;
            var factor = max ? -1 : 1;
            using var sourceIterator = source.GetEnumerator();
            if (!sourceIterator.MoveNext())
            {
                throw new InvalidOperationException("Sequence contains no elements");
            }
            var most = sourceIterator.Current;
            var mostKey = selector(most);
            while (sourceIterator.MoveNext())
            {
                var candidate = sourceIterator.Current;
                var candidateProjected = selector(candidate);
                if (comparer.Compare(candidateProjected, mostKey) * factor >= 0) continue;
                most = candidate;
                mostKey = candidateProjected;
            }
            var result = most;
            return result;
        }

        /// <summary>
        /// Extracts unique items given by a key selector using the default equality comparer.
        /// </summary>
        /// <typeparam name="T">Type of the items.</typeparam>
        /// <typeparam name="TKey">Type of property to compare.</typeparam>
        /// <param name="items">Target enumerable.</param>
        /// <param name="selector">Selector function.</param>
        /// <returns>A copy of the original list without duplicates.</returns>
        public static IEnumerable<T> DistinctBy<T, TKey>(this IEnumerable<T> items, Func<T, TKey> selector)
        {
            return items.GroupBy(selector).Select(x => x.First());
        }
    }
}