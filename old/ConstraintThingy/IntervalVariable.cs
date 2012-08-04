using System;
using System.Collections.Generic;
using System.Linq;
using Intervals;

namespace ConstraintThingy
{
    /// <summary>
    /// Interval variables represent a closed set of real numbers.
    /// </summary>
    public class IntervalVariable : Variable<Interval>
    {
        /// <summary>
        /// Creates a new interval variable with the specified name and allowable range.
        /// </summary>
        public IntervalVariable(string name, Interval allowableRange)
            : base(name, allowableRange)
        {
        }

        /// <summary>
        /// How small the width of an interval must be to be considered a unique value.
        /// </summary>
        public const float UniqueWidth = 0.00001f;

        /// <summary>
        /// If true, then the interval has a unique value and cannot be subdivided any further.
        /// </summary>
        public override bool IsUnique
        {
            get { return Value.Range <= UniqueWidth; }
        }

        /// <summary>
        /// If true, then the interval is empty and has no allowable values.
        /// </summary>
        public override bool IsEmpty
        {
            get { return Value.IsEmpty; }
        }

        /// <summary>
        /// Narrows variable to the intersection of its current value and RESTRICTION.
        /// </summary>
        public void NarrowTo(Interval restriction, ref bool success)
        {
            // TODO: FIGURE OUT WHY THIS ISNT OPTIMIZED TO ONLY DO ONE INTERSECTION.
            if (!Interval.Intersects(Value, restriction))
            {
                success = false;
                return;
            }
            TrySetValue(Interval.Intersection(Value, restriction), ref success);
        }

        /// <summary>
        /// Enumerates over possible values.
        /// </summary>
        public override IEnumerable<bool> UniqueValues()
        {
            throw new NotImplementedException();
            //yield return true;
          
            //Stack<Interval> candidates = new Stack<Interval>();
            //candidates.Push(Value);

            //// continue while there are still possibilities
            //while (candidates.Count > 0)
            //{
            //    var candidate = candidates.Pop();

            //    int mark = SaveValues();

            //    bool success = false;
            //    try
            //    {
            //        Value = candidate;
            //        success = true;
            //    }
            //    catch (Failure) { }

            //    if (success) 
            //    {
            //        yield return false;

            //        if (!IsUnique)
            //        {
            //            candidates.Push(candidate.UpperHalf);
            //            candidates.Push(candidate.LowerHalf);
            //        }
            //    }

            //    RestoreValues(mark);
            // }
        }

        /// <summary>
        /// Creates a new variable that is pre-constrained to be the sum of VARS
        /// </summary>
        public static IntervalVariable Sum(IEnumerable<IntervalVariable> vars)
        {
            IntervalVariable sum = new IntervalVariable("sum", new Interval(float.MinValue, float.MaxValue));
            new IntervalSumConstraint(sum, vars.ToArray());
            return sum;
        }
    }
}