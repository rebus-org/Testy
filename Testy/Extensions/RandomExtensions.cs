using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
// ReSharper disable UnusedMember.Global

namespace Testy.Extensions
{
    /// <summary>
    /// Useful extensions
    /// </summary>
    public static class RandomExtensions
    {
        static readonly ThreadLocal<Random> Random = new ThreadLocal<Random>(() => new Random(DateTime.Now.GetHashCode()));

        /// <summary>
        /// Returns the <paramref name="items"/> in random order
        /// </summary>
        public static IReadOnlyList<TItem> InRandomOrder<TItem>(this IEnumerable<TItem> items)
        {
            if (items == null) throw new ArgumentNullException(nameof(items));
            
            var list = items.ToList();
            var random = Random.Value;

            void Swap(int idx1, int idx2) => (list[idx1], list[idx2]) = (list[idx2], list[idx1]);

            int RandomIndex() => random.Next(list.Count);

            for (var counter = 0; counter < list.Count * 2; counter++)
            {
                Swap(RandomIndex(), RandomIndex());
            }

            return list;
        }

        /// <summary>
        /// Returns a list consisting of <paramref name="count"/> randomly picked items from <paramref name="items"/>
        /// </summary>
        public static IReadOnlyList<TItem> RandomPicksFrom<TItem>(this int count, IEnumerable<TItem> items)
        {
            if (items == null) throw new ArgumentNullException(nameof(items));
            
            var list = items.ToList();

            if (!list.Any())
            {
                throw new InvalidOperationException($"Cannot make {count} random picks from list of {typeof(TItem)} because it is empty");
            }

            int RandomIndex() => Random.Value.Next(list.Count);
          
            TItem PickItem() => list[RandomIndex()];

            return Enumerable.Repeat<Func<TItem>>(PickItem, count).Select(fn => fn()).ToList();
        }
    }
}