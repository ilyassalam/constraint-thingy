using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;

namespace ConstraintThingy
{
    /// <summary>
    /// Represents a range of possible integer values
    /// </summary>
    public struct IntegerInterval : IEquatable<IntegerInterval>
    {
        /// <summary>
        /// The smallest possible value in the interval
        /// </summary>
        public int LowerBound { get; private set; }

        /// <summary>
        /// The largest possible value in the interval
        /// </summary>
        public int UpperBound { get; private set; }

        /// <summary>
        /// Represents a contiguous sequence of integers with an upper and lower bound
        /// </summary>
        public IntegerInterval(int lowerBound, int upperBound) : this()
        {
            LowerBound = lowerBound;
            UpperBound = upperBound;
        }

        /// <summary>
        /// Adds two intervals
        /// </summary>
        [Pure]
        public static IntegerInterval Add(IntegerInterval a, IntegerInterval b)
        {
            return new IntegerInterval(a.LowerBound + b.LowerBound, a.UpperBound + b.UpperBound);
        }

        /// <summary>
        /// Subtracts two intervals
        /// </summary>
        [Pure]
        public static IntegerInterval Subtract(IntegerInterval a, IntegerInterval b)
        {
            return new IntegerInterval(a.LowerBound - b.UpperBound, a.UpperBound - b.LowerBound);
        }

        /// <summary>
        /// Adds two intervals
        /// </summary>
        public static IntegerInterval operator +(IntegerInterval left, IntegerInterval right)
        {
            return Add(left, right);
        }

        /// <summary>
        /// Subtracts <param name="right"></param> from <param name="left"></param>.
        /// </summary>
        public static IntegerInterval operator -(IntegerInterval left, IntegerInterval right)
        {
            return Subtract(left, right);
        }

        /// <summary>
        /// The difference between the largest and smallest value in the interval
        /// </summary>
        public int Range
        {
            get { return UpperBound - LowerBound; }
        }

        /// <summary>
        /// True if the interval contains a single value
        /// </summary>
        public bool IsUnique
        {
            get { return UpperBound == LowerBound; }
        }

        /// <summary>
        /// True if the interval contains no values
        /// </summary>
        public bool IsEmpty
        {
            // we use an invalid interval to signify empty intervals
            get { return UpperBound < LowerBound; }
        }

        /// <summary>
        /// Returns an interval that represents the intersection between <paramref name="a"/> and <paramref name="b"/>
        /// </summary>
        public static IntegerInterval Intersection(IntegerInterval a, IntegerInterval b)
        {
            int start = Math.Max(a.LowerBound, b.LowerBound);
            int end = Math.Min(a.UpperBound, b.UpperBound);

            return new IntegerInterval(start, end);
        }

        /// <summary>
        /// True if the intervals share at least one value.
        /// </summary>
        [Pure]
        public static bool Intersects(IntegerInterval a, IntegerInterval b)
        {
            int start = Math.Max(a.LowerBound, b.LowerBound);
            int end = Math.Min(a.UpperBound, b.UpperBound);

            return start <= end;
        }

        /// <summary>
        /// True if the integer interval contains the specified value
        /// </summary>
        public bool Contains(int value)
        {
            return value <= UpperBound && value >= LowerBound;
        }

        /// <summary>
        /// True if the integer interval contains the specified value, not including its upper and lower bounds
        /// </summary>
        public bool ContainsWithin(int value)
        {
            return value < UpperBound && value > LowerBound;
        }

        /// <summary>
        /// Splits an interval in half
        /// </summary>
        public void Split(int spot, out IntegerInterval upper, out IntegerInterval lower)
        {
            Debug.Assert(UpperBound > LowerBound);

            Debug.Assert(Contains(spot));

            if (spot == UpperBound)
            {
                upper = new IntegerInterval(spot, UpperBound);
                lower = new IntegerInterval(LowerBound, spot - 1);
            }
            else
            {
                upper = new IntegerInterval(spot + 1, UpperBound);
                lower = new IntegerInterval(LowerBound, spot);
            }
        }

        /// <summary>
        /// Tests for equality between two intervals.
        /// </summary>
        public static bool operator ==(IntegerInterval left, IntegerInterval right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Tests for inequality between two intervals.
        /// </summary>
        public static bool operator !=(IntegerInterval left, IntegerInterval right)
        {
            return !(left == right);
        }


        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <returns>
        /// true if <paramref name="obj"/> and this instance are the same type and represent the same value; otherwise, false.
        /// </returns>
        /// <param name="obj">Another object to compare to. </param><filterpriority>2</filterpriority>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (obj.GetType() != typeof (IntegerInterval)) return false;
            return Equals((IntegerInterval) obj);
        }

        /// <summary>
        /// The center value of the interval.
        /// </summary>
        public int Center
        {
            get
            {
                return (LowerBound & UpperBound) + ((LowerBound ^ UpperBound) >> 1);
            }
        }

        /// <summary>
        /// Integer intervals are equal if they have the same upper and lower bounds
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(IntegerInterval other)
        {
            return (!IsEmpty) && (!other.IsEmpty) && other.LowerBound == LowerBound && other.UpperBound == UpperBound;
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>
        /// A 32-bit signed integer that is the hash code for this instance.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override int GetHashCode()
        {
            unchecked
            {
                return (LowerBound*397) ^ UpperBound;
            }
        }

        /// <summary>
        /// Returns the fully qualified type name of this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> containing a fully qualified type name.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return IsUnique ? String.Format("[{0}]", LowerBound) : String.Format("[{0}, {1}]", LowerBound, UpperBound);
        }
    }
}