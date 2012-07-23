using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;

namespace ConstraintThingy
{
    /// <summary>
    /// Represents an immutable set of non-overlapping intervals
    /// </summary>
    // this can be made more efficient through an interval tree, but for small numbers of intervals this is probably fine
    public sealed class MultiInterval : IEnumerable<Interval>
    {
        // the set of non-overlapping intervals that represents this multi-interval
        private readonly List<Interval> _intervals = new List<Interval>(2);

        /// <summary>
        /// True if the multi-interval contains no values
        /// </summary>
        public bool IsEmpty { get { return _intervals.Count == 0; } }

        /// <summary>
        /// True if the multi-interval contains a single value
        /// </summary>
        public bool IsUnique { get { return _intervals.Count == 1 && _intervals[0].IsUnique; } }

        /// <summary>
        /// True if the multi-interval is disjoint (represented by two or more disjoint intervals)
        /// </summary>
        public bool IsDisjoint { get { return _intervals.Count > 1; } }

        /// <summary>
        /// The total range represented by the multi-interval
        /// </summary>
        public double Range
        {
            get
            {
                double sum = 0;
                foreach (var interval in _intervals)
                {
                    sum += interval.Range;
                }

                return sum;
            }
        }

        /// <summary>
        /// Creates a new empty multi-interval
        /// </summary>
        public MultiInterval()
        {
            
        }

        /// <summary>
        /// Creates a new multi-interval
        /// </summary>
        public MultiInterval(Interval interval)
        {
            Debug.Assert(!double.IsNaN(interval.LowerBound));
            Debug.Assert(!double.IsNaN(interval.UpperBound));

            _intervals.Add(interval);
        }

        /// <summary>
        /// Creates a new multi-interval from the provided set of intervals
        /// </summary>
        public MultiInterval(List<Interval> intervals)
        {
            for (int i = 0; i < intervals.Count; i++)
            {
                Add(intervals[i]);
            }
        }

        /// <summary>
        /// Creates a new mutli-interval from the provided set of intervals
        /// </summary>
        public MultiInterval(params Interval[] intervals)
        {
            foreach (var interval in intervals)
            {
                Add(interval);
            }
        }

        private static readonly List<int> IntersectingIndices = new List<int>();

        public MultiInterval Clone()
        {
            MultiInterval multiInterval = new MultiInterval();
            foreach (var interval in _intervals)
            {
                multiInterval._intervals.Add(interval);
            }
            return multiInterval;
        }

        private void Add(Interval interval)
        {
            Debug.Assert(!double.IsNaN(interval.LowerBound));
            Debug.Assert(!double.IsNaN(interval.UpperBound));
            Debug.Assert(!interval.IsEmpty);

            for (int i = 0; i < _intervals.Count; i++)
            {
                if (Interval.Intersects(_intervals[i], interval))
                {
                    IntersectingIndices.Add(i);
                }
            }

            Interval merged = interval;

            for (int index = 0; index < IntersectingIndices.Count; index++)
            {
                var intersectingIndex = IntersectingIndices[index];
                merged = Interval.Union(_intervals[intersectingIndex], merged);
            }

            // remove backwards so all the indices stay valid
            for (int i = IntersectingIndices.Count - 1; i >= 0; i--)
            {
                _intervals.RemoveAt(IntersectingIndices[i]);
            }

            _intervals.Add(merged);

            IntersectingIndices.Clear();
        }

        /// <summary>
        /// Computes and returns the intersection between <paramref name="a"/> and <paramref name="b"/>
        /// </summary>
        public static MultiInterval Intersection(MultiInterval a, MultiInterval b)
        {
            List<Interval> aIntervals = a._intervals;
            List<Interval> bIntervals = b._intervals;
            
            MultiInterval multiInterval = new MultiInterval();

            for (int i = 0; i < aIntervals.Count; i++)
            {
                for (int j = 0; j < bIntervals.Count; j++)
                {
                    Interval intersection = Interval.Intersection(aIntervals[i], bIntervals[j]);
                    if (!intersection.IsEmpty)
                    {
                        multiInterval.Add(intersection);
                    }
                }
            }

            return multiInterval;
        }

        /// <summary>
        /// Splits an interval up into its pieces. If it has only once piece, it will be split in half.
        /// </summary>
        [Pure]
        public MultiInterval[] Split()
        {
            MultiInterval[] splits;
            // more than 1 interval -> just split up the intervals
            if (_intervals.Count > 1)
            {
                splits = new MultiInterval[_intervals.Count];

                for (int i = 0; i < _intervals.Count; i++)
                {
                    splits[i] = _intervals[i];
                }

                return splits;
            }
            else if (_intervals.Count == 1)
            {
                splits = new MultiInterval[2];

                Interval upper;
                Interval lower;

                Interval interval = _intervals[0];

                interval.Split(interval.Center, out upper, out lower);

                splits[0] = upper;
                splits[1] = lower;

                return splits;
            }

            throw new InvalidOperationException("Can't split an empty interval!");
        }

        /// <summary>
        /// True if the two multi-intervals intersect
        /// </summary>
        public static bool Intersects(MultiInterval a, MultiInterval b)
        {
            List<Interval> aIntervals = a._intervals;
            List<Interval> bIntervals = b._intervals;

            for (int i = 0; i < aIntervals.Count; i++)
            {
                for (int j = 0; j < bIntervals.Count; j++)
                {
                    Interval intersection = Interval.Intersection(aIntervals[i], bIntervals[j]);
                    if (!intersection.IsEmpty)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Adds two multi-intervals
        /// </summary>
        public static MultiInterval Add(MultiInterval a, MultiInterval b)
        {
            List<Interval> aIntervals = a._intervals;
            List<Interval> bIntervals = b._intervals;

            MultiInterval multiInterval = new MultiInterval();

            for (int i = 0; i < aIntervals.Count; i++)
            {
                for (int j = 0; j < bIntervals.Count; j++)
                {
                    Interval sum = Interval.Add(aIntervals[i], bIntervals[j]);
                    multiInterval.Add(sum);
                }
            }
            
            return multiInterval;
        }

        /// <summary>
        /// Subtracts two multi-intervals
        /// </summary>
        public static MultiInterval Subtract(MultiInterval a, MultiInterval b)
        {
            List<Interval> aIntervals = a._intervals;
            List<Interval> bIntervals = b._intervals;

            MultiInterval multiInterval = new MultiInterval();
            for (int i = 0; i < aIntervals.Count; i++)
            {
                for (int j = 0; j < bIntervals.Count; j++)
                {
                    Interval difference = Interval.Subtract(aIntervals[i], bIntervals[j]);
                    multiInterval.Add(difference);
                }
            }

            return multiInterval;
        }

        /// <summary>
        /// Computes the quotient of two multi-intervals
        /// </summary>.
        public static MultiInterval Divide(MultiInterval dividend, MultiInterval divisor)
        {
            List<Interval> aIntervals = dividend._intervals;
            List<Interval> bIntervals = divisor._intervals;

            MultiInterval multiInterval = new MultiInterval();

            for (int i = 0; i < aIntervals.Count; i++)
            {
                for (int j = 0; j < bIntervals.Count; j++)
                {
                    Interval intervalDivisor = bIntervals[j];

                    if (intervalDivisor.Contains(0))
                    {
                        multiInterval.Add(Interval.Divide(aIntervals[i], new Interval(0, intervalDivisor.UpperBound)));
                        multiInterval.Add(Interval.Divide(aIntervals[i], new Interval(intervalDivisor.LowerBound, 0)));
                    }
                    else
                    {
                        multiInterval.Add(Interval.Divide(aIntervals[i], intervalDivisor));
                    }
                }
            }

            return multiInterval;
        }

        /// <summary>
        /// Computes the maximum of two multi-intervals
        /// </summary>
        public static MultiInterval Max(MultiInterval a, MultiInterval b)
        {
            List<Interval> aIntervals = a._intervals;
            List<Interval> bIntervals = b._intervals;

            MultiInterval multiInterval = new MultiInterval();

            for (int i = 0; i < aIntervals.Count; i++)
            {
                for (int j = 0; j < bIntervals.Count; j++)
                {
                    Interval max = Interval.Max(aIntervals[i], bIntervals[j]);
                    multiInterval.Add(max);
                }
            }

            return multiInterval;
        }

        /// <summary>
        /// Computes the maximum of two multi-intervals
        /// </summary>
        public static MultiInterval Min(MultiInterval a, MultiInterval b)
        {
            List<Interval> aIntervals = a._intervals;
            List<Interval> bIntervals = b._intervals;

            MultiInterval multiInterval = new MultiInterval();

            for (int i = 0; i < aIntervals.Count; i++)
            {
                for (int j = 0; j < bIntervals.Count; j++)
                {
                    Interval min = Interval.Min(aIntervals[i], bIntervals[j]);
                    multiInterval.Add(min);
                }
            }

            return multiInterval;
        }

        /// <summary>
        /// Extends the multi-interval to contain the specified value.
        /// </summary>
        public MultiInterval Extend(double value)
        {
            MultiInterval multiInterval = new MultiInterval();

            for (int i = 0; i < _intervals.Count; i++)
            {
                Interval extended = _intervals[i].Extend(value);
                multiInterval.Add(extended);
            }

            return multiInterval;
        }

        /// <summary>
        /// Computes the product of two multi-intervals
        /// </summary>
        public static MultiInterval Multiply(MultiInterval a, MultiInterval b)
        {
            List<Interval> aIntervals = a._intervals;
            List<Interval> bIntervals = b._intervals;

            MultiInterval multiInterval = new MultiInterval();

            for (int i = 0; i < aIntervals.Count; i++)
            {
                for (int j = 0; j < bIntervals.Count; j++)
                {
                    Interval product = Interval.Multiply(aIntervals[i], bIntervals[j]);
                    multiInterval.Add(product);
                }
            }

            return multiInterval;
        }

        /// <summary>
        /// True if the multi-interval contains the specified value
        /// </summary>
        public bool Contains(double value)
        {
            foreach (var interval in _intervals)
            {
                if (interval.Contains(value)) return true;
            }

            return false;
        }

        /// <summary>
        /// True if the multi-interval contains the specified value, excluding any interval boundaries
        /// </summary>
        public bool ContainsWithin(double value)
        {
            foreach (var interval in _intervals)
            {
                if (interval.ContainsWithin(value)) return true;
            }

            return false;
        }

        /// <summary>
        /// Automatically convert from intervals to multi-intervals
        /// </summary>
        public static implicit operator MultiInterval(Interval interval)
        {
            return new MultiInterval(interval);
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
            if (obj.GetType() != typeof(MultiInterval)) return false;
            return Equals((MultiInterval)obj);
        }

        /// <summary>
        /// MultiIntervals are equal if they contain the same values
        /// </summary>
        public bool Equals(MultiInterval other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            if (_intervals.Count != other._intervals.Count) return false;

            // this is an O(n^2)...

            for (int i = 0; i < _intervals.Count; i++)
            {
                if (!other._intervals.Contains(_intervals[i])) return false;
            }

            return true;
        }

        public override int GetHashCode()
        {
            int hashCode = 0;

            for (int i = 0; i < _intervals.Count; i++)
            {
                hashCode ^= _intervals[i].GetHashCode();
            }

            return hashCode;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<Interval> GetEnumerator()
        {
            return _intervals.GetEnumerator();
        }

        public override string ToString()
        {
            return "{" + _intervals.Select(i => i.ToString()).Aggregate((a, b) => a + "," + b) + "}";
        }

        #region Overloaded Operators

        /// <summary>
        /// Adds two intervals
        /// </summary>
        public static MultiInterval operator +(MultiInterval left, MultiInterval right)
        {
            return Add(left, right);
        }

        /// <summary>
        /// Subtracts <param name="right"></param> from <param name="left"></param>.
        /// </summary>
        public static MultiInterval operator -(MultiInterval left, MultiInterval right)
        {
            return Subtract(left, right);
        }

        /// <summary>
        /// Multiplies two intervals
        /// </summary>
        public static MultiInterval operator *(MultiInterval left, MultiInterval right)
        {
            return Multiply(left, right);
        }

        /// <summary>
        /// Divides <param name="dividend"></param> by <param name="divisor"></param>
        /// </summary>
        public static MultiInterval operator /(MultiInterval dividend, MultiInterval divisor)
        {
            return Divide(dividend, divisor);
        }

        /// <summary>
        /// Computes the union of two multi-intervals
        /// </summary>
        public static MultiInterval Union(MultiInterval a, MultiInterval b)
        {
            MultiInterval multiInterval = a.Clone();

            foreach (var interval in b._intervals)
            {
                multiInterval.Add(interval);
            }

            return multiInterval;
        }

        /// <summary>
        /// Tests for equality between two intervals.
        /// </summary>
        public static bool operator ==(MultiInterval left, MultiInterval right)
        {
            if (ReferenceEquals(left, null))
            {
                if (ReferenceEquals(right, null)) return true;

                return false;
            }
            else if (ReferenceEquals(right, null))
            {
                return false;
            }
            else return left.Equals(right);
        }

        /// <summary>
        /// Tests for inequality between two intervals.
        /// </summary>
        public static bool operator !=(MultiInterval left, MultiInterval right)
        {
            return !(left == right);
        }

        #endregion

        /// <summary>
        /// Returns the i'th element of the multi-interval
        /// </summary>
        public Interval this[int i]
        {
            get { return _intervals[i]; }
        }

        /// <summary>
        /// The number of intervals in the multi-interval
        /// </summary>
        public int Count { get { return _intervals.Count; } }
    }
}