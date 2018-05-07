using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Testy.Extensions
{
    public static class EnumerableExtensions
    {
        static readonly ThreadLocal<Random> Random = new ThreadLocal<Random>(() => new Random(DateTime.Now.GetHashCode()));

        public static List<TItem> InRandomOrder<TItem>(this IEnumerable<TItem> items)
        {
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
    }
}