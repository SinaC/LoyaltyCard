using System;
using System.Collections.Generic;
using System.Linq;

namespace LoyaltyCard.Common.Extensions
{
    public static class IEnumerableExtensions
    {
        public static T WhereMax<T, U>(this IEnumerable<T> items, Func<T, U> selector)
        {
            if (!items.Any())
            {
                //throw new InvalidOperationException("Empty input sequence");
                return default(T);
            }

            var comparer = Comparer<U>.Default;
            T maxItem = items.First();
            U maxValue = selector(maxItem);

            foreach (T item in items.Skip(1))
            {
                // Get the value of the item and compare it to the current max.
                U value = selector(item);
                if (comparer.Compare(value, maxValue) > 0)
                {
                    maxValue = value;
                    maxItem = item;
                }
            }

            return maxItem;
        }

        public static int? SumNullIfEmpty<T>(this IEnumerable<T> items, Func<T, int> filterFunc)
        {
            if (!items.Any())
                return null;
            return items.Sum(filterFunc);
        }

        public static decimal? SumNullIfEmpty<T>(this IEnumerable<T> items, Func<T,decimal> filterFunc)
        {
            if (!items.Any())
                return null;
            return items.Sum(filterFunc);
        }
    }
}
