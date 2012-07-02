using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Text;

namespace ConstraintThingy
{
    /// <summary>
    /// A set of helper methods for managing unsigned longs as bit sets
    /// </summary>
    public static class BitHelper
    {
        /// <summary>
        /// Returns the number of set bits
        /// </summary>
        [Pure]
        public static int BitCount(this UInt64 set)
        {
            set -= (set >> 1) & 0x5555555555555555UL;
            set = (set & 0x3333333333333333UL) + ((set >> 2) & 0x3333333333333333UL);
            set = (set + (set >> 4)) & 0x0f0f0f0f0f0f0f0fUL;
            return (int)((set * 0x0101010101010101UL) >> 56);
        }

        /// <summary>
        /// Sets the <paramref name="i"/>'th bit of <paramref name="set"/>
        /// </summary>
        [Pure]
        public static UInt64 SetBit(this UInt64 set, int i)
        {
            Debug.Assert(i < MaxSize && i >= 0);

            return set | GetMask(i);
        }

        /// <summary>
        /// Clears the <paramref name="i"/>'th bit of <paramref name="set"/>
        /// </summary>
        [Pure]
        public static UInt64 ClearBit(this UInt64 set, int i)
        {
            Debug.Assert(i < MaxSize && i >= 0);

            return set | (~ GetMask(i));
        }

        /// <summary>
        /// Gets a mask for the <paramref name="i"/>'th bit
        /// </summary>
        [Pure]
        public static UInt64 GetMask(int i)
        {
            Debug.Assert(i < MaxSize && i >= 0);

            return 1UL << i;
        }

        /// <summary>
        /// True if <paramref name="set"/> contains any elements of <paramref name="other"/>.
        /// </summary>
        [Pure]
        public static bool ContainsAny(this UInt64 set, UInt64 other)
        {
            return (set & other) != 0UL;
        }

        /// <summary>
        /// True if this <paramref name="set"/> contains a set bit at index <paramref name="i"/>
        /// </summary>
        [Pure]
        public static bool ContainsIndex(this UInt64 set, int i)
        {
            Debug.Assert(i < MaxSize && i >= 0);

            return set.ContainsAny(GetMask(i));
        }

        /// <summary>
        /// True if the bit set is a singleton (has exactly one element)
        /// </summary>
        [Pure]
        public static bool IsSingleton(this UInt64 set)
        {
            return (set != 0UL) && ((set & (set - 1UL)) == 0UL);
        }

        /// <summary>
        /// True if the bit set is empty (has no elements)
        /// </summary>
        [Pure]
        public static bool IsEmpty(this UInt64 set)
        {
            return set == 0UL;
        }

        /// <summary>
        /// The maximum size of a bit set
        /// </summary>
        public const int MaxSize = 64;

        /// <summary>
        /// Maximum index in a bit set
        /// </summary>
        public const int MaxIndex = MaxSize - 1;

        /// <summary>
        /// Gets an array of the set indicies of the bit set
        /// </summary>
        [Pure]
        public static int[] GetSetIndices(this UInt64 set)
        {
            int[] indices = new int[set.BitCount()];

            int count = 0;
            for (int i = 0; i < MaxSize - 1; i++)
            {
                if (set.ContainsIndex(i))
                {
                    indices[count] = i;
                    count++;
                }
            }

            return indices;
        }

        /// <summary>
        /// Makes a string of the bit set
        /// </summary>
        [Pure]
        public static String MakeString<T>(this UInt64 set, FiniteDomain<T> domain)
        {
            StringBuilder builder = new StringBuilder();

            builder.Append('{');

            foreach (var index in set.GetSetIndices())
            {
                builder.Append(domain[index]);
                builder.Append(", ");
            }

            // remove the last comma
            if (builder.Length > 2) builder.Remove(builder.Length - 2, 2);

            builder.Append('}');

            return builder.ToString();
        }
    }
}