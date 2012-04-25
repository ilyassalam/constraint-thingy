using System;
using System.Collections;
using System.Collections.Generic;
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
        /// Enumerates over possible values.
        /// </summary>
        public override IEnumerable<bool> UniqueValues()
        {
            yield return true;
          
            Stack<Interval> candidates = new Stack<Interval>();
            candidates.Push(Value);

            // continue while there are still possibilities
            while (candidates.Count > 0)
            {
                var candidate = candidates.Pop();

                int mark = SaveValues();

                bool success = false;
                try
                {
                    Value = candidate;
                    success = true;
                }
                catch (Failure) { }

                if (success) 
                {
                    yield return false;

                    if (!IsUnique)
                    {
                        candidates.Push(candidate.UpperHalf);
                        candidates.Push(candidate.LowerHalf);
                    }
                }

                RestoreValues(mark);
            }
        }
    }
}