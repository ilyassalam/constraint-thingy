using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using CSharpUtils;
using CSharpUtils.Collections;

namespace ConstraintThingy
{
    /// <summary>
    /// An closed interval is a set of real numbers such that any number between the lower and upper bounds, including the endpoints, is also included in the set.
    /// </summary>
    public struct Interval
    {
        /// <summary>
        /// Creates a new interval with upper and lower bound equal to <paramref name="value"/>
        /// </summary>
        public Interval(double value) : this(value, value)
        {
        }

        /// <summary>
        /// Creates a new interval with the specified lower and upper bounds
        /// </summary>
        public Interval(double lowerBound, double upperBound)
            : this()
        {
            if (lowerBound > upperBound) throw new ArgumentOutOfRangeException(String.Format("The upper bound of {0} must be at least the lower bound of {1}", upperBound, lowerBound));

            LowerBound = lowerBound;
            UpperBound = upperBound;
        }

        /// <summary>
        /// The smallest possible value of members in the interval
        /// </summary>
        public double LowerBound;

        /// <summary>
        /// The largest possible value of members in the interval
        /// </summary>
        public double UpperBound;

        /// <summary>
        /// The absolute difference of the endpoints.
        /// </summary>
        public double Range { get { return UpperBound - LowerBound; } }

        /// <summary>
        /// True if UpperBound == LowerBound.
        /// </summary>
        public bool IsUnique { get { return UpperBound == LowerBound; } }

        /// <summary>
        /// The upper half of the interval.
        /// </summary>
        public Interval UpperHalf { get { return new Interval(Center, UpperBound); } }

        /// <summary>
        /// Splits an interval in half, guaranteeing that the two intervals do not overlap.
        /// </summary>
        public void Split(double spot, out Interval upper, out Interval lower)
        {
            // special case of width = 2
            if (spot == UpperBound || spot == LowerBound)
            {
                Debug.Assert(Range <= .01f);
                upper = new Interval(UpperBound, UpperBound);
                lower = new Interval(LowerBound, LowerBound);

                return;
            }

            Debug.Assert(UpperBound > LowerBound);

            long center;
            long upperBound;
            long lowerBound;

            unsafe
            {
                center = *((long*)(double*)(&spot));

                fixed (double * ptr = &UpperBound)
                {
                    upperBound = *((long*)(double*) ptr);
                }

                fixed (double* ptr = &LowerBound)
                {
                    lowerBound = *((long*)(double*) ptr);
                }
            }

            Debug.Assert(spot >= LowerBound && spot <= UpperBound);


            long lowerUpperBound = (center >= 0 ? center - 1 : center + 1);

            unsafe
            {
                double lowerUpperBoundD = *((double*)&lowerUpperBound);
                if (double.IsNaN(lowerUpperBoundD)) 
                    lowerUpperBoundD = BitConverter.Int64BitsToDouble(-9223372036854775807);
                    
                lower = new Interval(
                    *((double*)&lowerBound),
                    lowerUpperBoundD);
                upper = new Interval(
                    *((double*)&center),
                    *((double*)&upperBound));
            }

            Debug.Assert(!double.IsNaN(lower.LowerBound));
            Debug.Assert(!double.IsNaN(lower.UpperBound));
            Debug.Assert(!double.IsNaN(upper.LowerBound));
            Debug.Assert(!double.IsNaN(upper.UpperBound));

            Debug.Assert(!Intersects(upper, lower));
        }

        /// <summary>
        /// The lower half of the interval
        /// </summary>
        public Interval LowerHalf { get { return new Interval(LowerBound, Center); } }

        /// <summary>
        /// The center value of the interval.
        /// </summary>
        public double Center
        {
            get
            {
                return (LowerBound + UpperBound) * .5f;
            }
        }

        /// <summary>
        /// Extends the interval to include a new value.
        /// </summary>
        public Interval Extend(double value)
        {
            Interval result = new Interval(LowerBound, UpperBound);
            
            if (value < LowerBound)
                result.LowerBound = value;
            else if (value > UpperBound)
                result.UpperBound = value;

            return result;
        }

        /// <summary>
        /// Represents the empty interval
        /// </summary>
        public static readonly Interval Empty = new Interval(double.NaN, double.NaN);

        /// <summary>
        /// True if the interval contains no values.
        /// </summary>
        public bool IsEmpty
        {
            get
            {
                return double.IsNaN(UpperBound) || double.IsNaN(LowerBound);
            }
        }

        /// <summary>
        /// True if the interval contains <param name="value"></param>
        /// </summary>
        [Pure]
        public bool Contains(double value)
        {
            return value <= UpperBound && value >= LowerBound;
        }

        /// <summary>
        /// True if the interval contains <paramref name="value"/>, excluding the bounds
        /// </summary>
        public bool ContainsWithin(double value)
        {
            return value < UpperBound && value > LowerBound;
        }

        /// <summary>
        /// True if the interval contains <param name="value"></param>
        /// </summary>
        [Pure]
        public bool Contains(Interval value)
        {
            return value.UpperBound <= UpperBound && value.LowerBound >= LowerBound;
        }

        #region Operators

        /// <summary>
        /// Adds two intervals.
        /// </summary>
        [Pure]
        public static Interval Add(Interval a, Interval b)
        {
            return new Interval(a.LowerBound + b.LowerBound, a.UpperBound + b.UpperBound);
        }

        /// <summary>
        /// Subtracts <param name="b"></param> from <param name="a"></param>.
        /// </summary>
        [Pure]
        public static Interval Subtract(Interval a, Interval b)
        {
            return new Interval(a.LowerBound - b.UpperBound, a.UpperBound - b.LowerBound);
        }

        /// <summary>
        /// Multiplies two intervals.
        /// </summary>
        [Pure]
        public static Interval Multiply(Interval a, Interval b)
        {
            double a1b1 = (a.LowerBound == 0) || (b.LowerBound == 0) ? 0 : a.LowerBound * b.LowerBound;
            double a1b2 = (a.LowerBound == 0) || (b.UpperBound == 0) ? 0 : a.LowerBound * b.UpperBound;
            double a2b1 = (a.UpperBound == 0) || (b.LowerBound == 0) ? 0 : a.UpperBound * b.LowerBound;
            double a2b2 = (a.UpperBound == 0) || (b.UpperBound == 0) ? 0 : a.UpperBound * b.UpperBound;

            return new Interval(FastMin(a1b1, FastMin(a1b2, FastMin(a2b1, a2b2))), FastMax(a1b1, FastMax(a1b2, FastMax(a2b1, a2b2))));
        }

        // these are inlined and are significantly faster than Math.Min / Max

        private static double FastMin(double a, double b)
        {
            return a < b ? a : b;
        }

        private static double FastMax(double a, double b)
        {
            return a > b ? a : b;
        }

        /// <summary>
        /// Divides <param name="dividend"></param> by <param name="divisor"></param>
        /// </summary>
        [Pure]
        public static Interval Divide(Interval dividend, Interval divisor)
        {
            if (divisor.ContainsWithin(0f)) throw new ArgumentOutOfRangeException(String.Format("The divisor, {0}, cannot contain 0 within the interval.", divisor));

            return Multiply(dividend, new Interval(divisor.UpperBound == 0.0 ? double.NegativeInfinity : 1 / divisor.UpperBound, divisor.LowerBound == 0.0 ? double.PositiveInfinity : 1 / divisor.LowerBound));
        }

        /// <summary>
        /// True if the intervals share at least one value.
        /// </summary>
        [Pure]
        public static bool Intersects(Interval a, Interval b)
        {
            double start = Math.Max(a.LowerBound, b.LowerBound);
            double end = Math.Min(a.UpperBound, b.UpperBound);

            return start <= end;
        }

        /// <summary>
        /// Computes an interval representing the intersection of <param name="a"></param> and <param name="b"></param>
        /// </summary>
        [Pure]
        public static Interval Intersection(Interval a, Interval b)
        {
            double start = Math.Max(a.LowerBound, b.LowerBound);
            double end = Math.Min(a.UpperBound, b.UpperBound);

            if (start > end) return Empty;

            return new Interval(start, end);
        }

        /// <summary>
        /// Computes an interval representing the union of <param name="a"></param> and <param name="b"></param>
        /// </summary>
        [Pure]
        public static Interval Union(Interval a, Interval b)
        {
            if (a.IsEmpty && b.IsEmpty) return Empty;

            if (a.IsEmpty) return b;

            if (b.IsEmpty) return a;

            return new Interval(Math.Min(a.LowerBound, b.LowerBound), Math.Max(a.UpperBound, b.UpperBound));
        }

        /// <summary>
        /// Computes an interval representing the minimum of <param name="a"></param> and <param name="b"></param>
        /// </summary>
        [Pure]
        public static Interval Min(Interval a, Interval b)
        {
            return new Interval(Math.Min(a.LowerBound, b.LowerBound), Math.Min(a.UpperBound, b.UpperBound));
        }

        /// <summary>
        /// Computes an interval representing the maximum of <param name="a"></param> and <param name="b"></param>
        /// </summary>
        [Pure]
        public static Interval Max(Interval a, Interval b)
        {
            return new Interval(Math.Max(a.LowerBound, b.LowerBound), Math.Max(a.UpperBound, b.UpperBound));
        }

        #endregion

        #region Overloaded Operators

        /// <summary>
        /// Adds two intervals
        /// </summary>
        public static Interval operator +(Interval left, Interval right)
        {
            return Add(left, right);
        }

        /// <summary>
        /// Subtracts <param name="right"></param> from <param name="left"></param>.
        /// </summary>
        public static Interval operator -(Interval left, Interval right)
        {
            return Subtract(left, right);
        }

        /// <summary>
        /// Multiplies two intervals
        /// </summary>
        public static Interval operator *(Interval left, Interval right)
        {
            return Multiply(left, right);
        }

        /// <summary>
        /// Divides <param name="dividend"></param> by <param name="divisor"></param>
        /// </summary>
        public static Interval operator /(Interval dividend, Interval divisor)
        {
            return Divide(dividend, divisor);
        }

        /// <summary>
        /// Tests for equality between two intervals.
        /// </summary>
        public static bool operator ==(Interval left, Interval right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Tests for inequality between two intervals.
        /// </summary>
        public static bool operator !=(Interval left, Interval right)
        {
            return !(left == right);
        }

        #endregion

        #region Equality

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
            if (obj.GetType() != typeof(Interval)) return false;
            return Equals((Interval)obj);
        }

        /// <summary>
        /// Intervals are equal if their lower and upper bounds are the same.
        /// </summary>
        public bool Equals(Interval other)
        {
            return other.LowerBound.Equals(LowerBound) && other.UpperBound.Equals(UpperBound);
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
                return (LowerBound.GetHashCode() * 397) ^ UpperBound.GetHashCode();
            }
        }
        #endregion

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