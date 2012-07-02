using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace CSharpUtils
{
    /// <summary>
    /// Contains several extension and utility methods for common or useful operations
    /// </summary>
    public static class Util
    {
        /// <summary>
        /// Null checks an action delegate before executing it.
        /// </summary>
        public static void SafeInvoke(this Action action)
        {
            if (action != null) action();
        }

        /// <summary>
        /// Null checks an action delegate before executing it.
        /// </summary>
        public static void SafeInvoke<T>(this Action<T> action, T arg1)
        {
            if (action != null) action(arg1);
        }

        /// <summary>
        /// Null checks an action delegate before executing it.
        /// </summary>
        public static void SafeInvoke<T1, T2>(this Action<T1, T2> action, T1 arg1, T2 arg2)
        {
            if (action != null) action(arg1, arg2);
        }

        /// <summary>
        /// Null checks an action delegate before executing it.
        /// </summary>
        public static void SafeInvoke<T1, T2, T3>(this Action<T1, T2, T3> action, T1 arg1, T2 arg2, T3 arg3)
        {
            if (action != null) action(arg1, arg2, arg3);
        }

        /// <summary>
        /// Null checks an action delegate before executing it.
        /// </summary>
        public static void SafeInvoke<T1, T2, T3, T4>(this Action<T1, T2, T3, T4> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            if (action != null) action(arg1, arg2, arg3, arg4);
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
            for (int i = array.Length; i > 1; i--)
            {
                // Pick random element to swap.
                int j = Random.Next(i); // 0 <= j <= i-1
                // Swap.
                array.Swap(j, i - 1);
            }
        }

        /// <summary>
        /// Standard implementation of an in-place fisher-yates shuffle. O(n) time
        /// </summary>
        public static void Shuffle<T>(this List<T> list)
        {
            for (int i = list.Count; i > 1; i--)
            {
                // Pick random element to swap.
                int j = Random.Next(i); // 0 <= j <= i-1
                // Swap.
                list.Swap(j, i - 1);
            }
        }

        /// <summary>
        /// Creates a new array with the elements shuffled.
        /// </summary>
        public static T[] ToShuffled<T>(this T[] array)
        {
            var newArray = array.ToArray();
            newArray.Shuffle();
            return newArray;
        }

        /// <summary>
        /// Creates a new array with the elements shuffled.
        /// </summary>
        public static T[] ToShuffled<T>(this IEnumerable<T> items)
        {
            var newArray = items.ToArray();
            newArray.Shuffle();
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
            return (float)random.NextNormal(mean, stdev);
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
            return (float)random.NextExponential(mean);
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
            return (float)(random.NextDouble() * (max - min) + min);
        }

        /// <summary>
        /// Returns true with probability P=<paramref name="probability"/>
        /// </summary>
        public static bool Chance(this Random random, double probability)
        {
            Debug.Assert(probability <= 1.0 && probability >= 0.0);
            return Random.NextDouble() <= probability;
        }
    }
}
