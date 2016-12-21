namespace TestConsole.Utilities
{
    using System.Collections.Generic;
    using System.Linq;

    public static class IEnumerableExtension
    {
        public static IEnumerable<T> WithoutLast<T>(this IEnumerable<T> source)
        {
            using (var e = source.GetEnumerator())
            {
                if (e.MoveNext())
                {
                    for (var value = e.Current; e.MoveNext(); value = e.Current)
                    {
                        yield return value;
                    }
                }
            }
        }

        public static IEnumerable<IEnumerable<T>> Partition<T>(this IEnumerable<T> collection, int size)
        {
            IList<T> nextbatch = new List<T>(size);
            foreach (T item in collection)
            {
                nextbatch.Add(item);
                if (nextbatch.Count == size)
                {
                    yield return nextbatch;
                    nextbatch = new List<T>(size);
                }
            }
            if (nextbatch.Count > 0)
                yield return nextbatch;
        }

        public static IEnumerable<IEnumerable<T>> GroupElements<T>(this IEnumerable<T> collection, int batchSize)
        {
            int total = 0;
            var enumerable = collection as T[] ?? collection.ToArray();
            while (total < enumerable.Count())
            {
                yield return enumerable.Skip(total).Take(batchSize);
                total += batchSize;
            }
        }

        public static IEnumerable<IEnumerable<T>> Split<T>(this IEnumerable<T> source, int size)
        {
            return source.Select((x, i) => new { Index = i, Value = x }).GroupBy(x => x.Index / size).Select(x => x.Select(v => v.Value).ToList());
        }
    }
}
