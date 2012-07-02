using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using CSharpUtils.Collections;

namespace CSharpUtils
{
    /// <summary>
    /// Contains several extension and utility methods for common or useful operations
    /// </summary>
    public static class Util
    {
        /// <summary>
        /// Creates an enumerable with all pairs in the two sequences
        /// </summary>
        public static IEnumerable<Tuple<T1, T2>> Pairs<T1, T2>(IEnumerable<T1> first, IEnumerable<T2> second)
        {
            foreach (var a in first)
            {
                foreach (var b in second)
                {
                    yield return new Tuple<T1, T2>(a, b);
                }
            }
        }

        /// <summary>
        /// Creates an infinite lazy sequence of values
        /// </summary>
        public static IEnumerable<T> Sequence<T>(T start, Func<T, T> getNext)
        {
            var current = start;

            while (true)
            {
                yield return current;

                current = getNext(current);
            }
        }

        /// <summary>
        /// Create a sequence of integers from <paramref name="min"/> to <paramref name="max"/> - 1
        /// </summary>
        public static IEnumerable<int> Range(int min, int max)
        {
            for (int i = min; i < max; i++)
            {
                yield return i;
            }
        }

        /// <summary>
        /// Creates a new pair
        /// </summary>
        public static Pair<T1, T2> PairedWith<T1, T2>(this T1 first, T2 second)
        {
            return new Pair<T1, T2>(first, second);
        }

        /// <summary>
        /// Takes the first <paramref name="count"/> items from the sequence, throwing an exception if not enough were found.
        /// </summary>
        public static IEnumerable<T> FirstElements<T>(this IEnumerable<T> sequence, int count)
        {
            if (count == 0) yield break;

            foreach (var item in sequence)
            {
                yield return item;

                count--;

                if (count <= 0) yield break;
            }

            throw new InvalidOperationException("There were not enough items in the sequence.");
        }

        /// <summary>
        /// True if <paramref name="min"/> &lt;= <paramref name="integer"/> &lt; <paramref name="max"/>
        /// </summary>
        public static bool InRange(this Int32 integer, Int32 min, Int32 max)
        {
            return integer >= min && integer < max;
        }

        /// <summary>
        /// True if <paramref name="min"/> &lt;= <paramref name="integer"/> &lt; <paramref name="max"/>
        /// </summary>
        public static bool InRange(this Int64 integer, Int64 min, Int64 max)
        {
            return integer >= min && integer < max;
        }

        /// <summary>
        /// True if <paramref name="min"/> &lt;= <paramref name="integer"/> &lt; <paramref name="max"/>
        /// </summary>
        public static bool InRange(this UInt32 integer, UInt32 min, UInt32 max)
        {
            return integer >= min && integer < max;
        }

        /// <summary>
        /// True if <paramref name="min"/> &lt;= <paramref name="integer"/> &lt; <paramref name="max"/>
        /// </summary>
        public static bool InRange(this UInt64 integer, UInt64 min, UInt64 max)
        {
            return integer >= min && integer < max;
        }

        /// <summary>
        /// True if <paramref name="min"/> &lt;= <paramref name="integer"/> &lt; <paramref name="max"/>
        /// </summary>
        public static bool InRange(this float integer, float min, float max)
        {
            return integer >= min && integer < max;
        }

        /// <summary>
        /// True if <paramref name="min"/> &lt;= <paramref name="integer"/> &lt; <paramref name="max"/>
        /// </summary>
        public static bool InRange(this double integer, double min, double max)
        {
            return integer >= min && integer < max;
        }

        // deterministic choices when we aren't in release mode

        /// <summary>
        /// Can be used to generate pseudo-random numbers
        /// </summary>
#if !RELEASE
        public static readonly Random Random = new Random(0);
#else
        public static readonly Random Random = new Random();
#endif

        /// <summary>
        /// Applies an action to an object and returns its instance. Intended to be used as a more general form of object initializers.
        /// </summary>
        public static T With<T>(this T obj, Action<T> action)
        {
            if (obj != null)
                action(obj);
            return obj;
        }

        /// <summary>
        /// Swaps the values in two variables.
        /// </summary>
        public static void Swap<T>(ref T first, ref T second)
        {
            T temp = first;
            first = second;
            second = temp;
        }

        /// <summary>
        /// Swaps the items at indices i and j in an array
        /// </summary>
        public static void Swap<T>(this T[] array, int i, int j)
        {
            T tmp = array[j];
            array[j] = array[i];
            array[i] = tmp;
        }

        /// <summary>
        /// Swaps the items at indices i and j in a list
        /// </summary>
        public static void Swap<T>(this List<T> list, int i, int j)
        {
            T tmp = list[j];
            list[j] = list[i];
            list[i] = tmp;
        }

        /// <summary>
        /// Returns <paramref name="count"/> distinct random items from the collection. Runs in O(n) time, where n is the number of items in the collection.
        /// </summary>
        public static IEnumerable<T> RandomElements<T>(this IEnumerable<T> items, int count)
        {
            T[] array = items.ToArray();
            array.Shuffle();

            if (count >= array.Length) throw new InvalidOperationException(String.Format("There were not {0} items in the collection.", count));

            for (int i = 0; i < count; i++)
            {
                yield return array[i];
            }
        }

        /// <summary>
        /// Returns a random element from the list.
        /// </summary>
        public static T RandomElement<T>(this List<T> list)
        {
            return list[Random.Next(0, list.Count)];
        }

        /// <summary>
        /// Returns a random element from the array.
        /// </summary>
        public static T RandomElement<T>(this T[] array)
        {
            return array[Random.Next(0, array.Length)];
        }

        private static class RandomStuff<T>
        {
            // we have separate lists for each type to prevent boxing and unboxing
            public static readonly List<T> Stuff = new List<T>();
        }

        /// <summary>
        /// Returns a random element from the enumerable. This operation runs in O(n) time.
        /// </summary>
        public static T RandomElement<T>(this IEnumerable<T> enumerable)
        {
            RandomStuff<T>.Stuff.AddRange(enumerable);

            T result = RandomStuff<T>.Stuff.RandomElement();
            RandomStuff<T>.Stuff.Clear();

            return result;
        }

        /// <summary>
        /// Standard implementation of an in-place fisher-yates shuffle. O(n) time
        /// </summary>
        public static void Shuffle<T>(this T[] array)
        {
            array.Shuffle(Random);
        }

        /// <summary>
        /// Standard implementation of an in-place fisher-yates shuffle. O(n) time
        /// </summary>
        public static void Shuffle<T>(this T[] array, Random random)
        {
            for (int i = array.Length; i > 1; i--)
            {
                // Pick random element to swap.
                int j = random.Next(i); // 0 <= j <= i-1
                // Swap.
                array.Swap(j, i - 1);
            }
        }

        /// <summary>
        /// Standard implementation of an in-place fisher-yates shuffle. O(n) time
        /// </summary>
        public static void Shuffle<T>(this List<T> list)
        {
            list.Shuffle(Random);
        }

        /// <summary>
        /// Standard implementation of an in-place fisher-yates shuffle. O(n) time
        /// </summary>
        public static void Shuffle<T>(this List<T> list, Random random)
        {
            for (int i = list.Count; i > 1; i--)
            {
                // Pick random element to swap.
                int j = random.Next(i); // 0 <= j <= i-1
                // Swap.
                list.Swap(j, i - 1);
            }
        }

        /// <summary>
        /// Creates a new array with the elements shuffled.
        /// </summary>
        public static T[] ToShuffled<T>(this T[] array)
        {
            return array.ToShuffled(Random);
        }

        /// <summary>
        /// Creates a new array with the elements shuffled.
        /// </summary>
        public static T[] ToShuffled<T>(this T[] array, Random random)
        {
            var newArray = array.ToArray();
            newArray.Shuffle(random);
            return newArray;
        }

        /// <summary>
        /// Creates a new array with the elements shuffled.
        /// </summary>
        public static T[] ToShuffled<T>(this IEnumerable<T> items)
        {
            return items.ToShuffled(Random);
        }

        /// <summary>
        /// Creates a new array with the elements shuffled.
        /// </summary>
        public static T[] ToShuffled<T>(this IEnumerable<T> items, Random random)
        {
            var newArray = items.ToArray();
            newArray.Shuffle(random);
            return newArray;
        }

        /// <summary>
        /// Creates a new set from the input enumerable. Duplicate elements are removed.
        /// </summary>
        public static ISet<T> ToSet<T>(this IEnumerable<T> input)
        {
            HashSet<T> items = new HashSet<T>();

            foreach (var item in input)
            {
                items.Add(item);
            }

            return items;
        }

        /// <summary>
        /// Returns an enumerable of a sequence with the duplicate elements removed.
        /// </summary>
        public static IEnumerable<T> Unique<T>(this IEnumerable<T> input)
        {
            return input.ToSet().ToArray();
        }

        /// <summary>
        /// Returns a normally distributed double with the specified mean and standard deviation
        /// </summary>
        public static double NextNormal(this Random random, double mean, double stdev)
        {
            double u = random.NextDouble();
            double v = random.NextDouble();
            double g = Math.Sqrt(-2 * Math.Log(u)) * Math.Cos(2 * Math.PI * v);
            return (mean + stdev * g);
        }

        /// <summary>
        /// Returns a normally distributed float with the specified mean and standard deviation
        /// </summary>
        public static float NextNormalF(this Random random, float mean, float stdev)
        {
            return (float) random.NextNormal(mean, stdev);
        }

        /// <summary>
        /// Returns an exponentially distributed double with the specified mean
        /// </summary>
        public static double NextExponential(this Random random, double mean)
        {
            return mean * (-Math.Log(random.NextDouble()));
        }

        /// <summary>
        /// Returns an exponentially distributed float with the specified mean
        /// </summary>
        public static float NextExponentialF(this Random random, float mean)
        {
            return (float) random.NextExponential(mean);
        }

        /// <summary>
        /// Returns the next double within a specified range.
        /// </summary>
        public static double NextDouble(this Random random, double min, double max)
        {
            return random.NextDouble() * (max - min) + min;
        }

        /// <summary>
        /// Returns the next float within a specified range.
        /// </summary>
        public static float NextFloat(this Random random, float min, float max)
        {
            return (float) (random.NextDouble() * (max - min) + min);
        }

        /// <summary>
        /// Returns true with probability P=<paramref name="probability"/>
        /// </summary>
        public static bool Chance(this Random random, double probability)
        {
            Debug.Assert(probability <= 1.0 && probability >= 0.0);
            return random.NextDouble() <= probability;
        }

        /// <summary>
        /// Returns true with probability P=0.5
        /// </summary>
        public static bool CoinFlip(this Random random)
        {
            return random.Next(2) == 0;
        }
    }
}
