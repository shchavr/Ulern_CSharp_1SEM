using System;
using System.Collections.Generic;
using System.Linq;

namespace linq_slideviews
{
    public static class ExtensionsTask
    {
        public static double Median(this IEnumerable<double> inputItems)
        {
            var sortedItems = inputItems.OrderBy(item => item).ToList();
            if (!sortedItems.Any())
                throw new InvalidOperationException();

            var middleIndex = sortedItems.Count / 2;

            if (sortedItems.Count % 2 == 1)
                return sortedItems[middleIndex];

            return (sortedItems[middleIndex - 1] + sortedItems[middleIndex]) / 2;
        }

        public static IEnumerable<(T First, T Second)> Bigrams<T>(this IEnumerable<T> inputItems)
        {
            var previousItem = default(T);
            var isFirst = true;

            foreach (var element in inputItems)
            {
                if (isFirst) isFirst = false;
                else yield return (previousItem, element);

                previousItem = element;
            }
        }
    }
}


