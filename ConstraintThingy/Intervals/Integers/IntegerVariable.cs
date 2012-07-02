using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using CSharpUtils;

namespace ConstraintThingy
{
    /// <summary>
    /// Integer variables represent a sequential sequence of integers
    /// </summary>
    public class IntegerVariable : Variable<IntegerInterval>
    {
        /// <summary>
        /// Creates a new integer variable with the specified name and allowable range
        /// </summary>
        public IntegerVariable(ConstraintThingySolver constraintThingySolver, string name, IntegerInterval allowableValues)
            : base(constraintThingySolver, name, allowableValues)
        {
        }

        /// <summary>
        /// The default range of integer variables
        /// </summary>
        public static readonly IntegerInterval DefaultRange = new IntegerInterval(-OneMillion, OneMillion);

        private const int OneMillion = 1000000;

        /// <summary>
        /// True if the variable has been narrowed to a single, unique value.
        /// </summary>
        public override bool IsUnique
        {
            get { return AllowableValues.IsUnique; }
        }

        public IntegerInterval CurrentValue { get { return AllowableValues; } }

        /// <summary>
        /// True if the variable has been narrowed to the empty set.
        /// </summary>
        public override bool IsEmpty
        {
            get { return AllowableValues.IsEmpty; }
        }

        /// <summary>
        /// Narrows an interval such that it is within <paramref name="restriction"/>. Simply a wrapper arround TrySetValue(Interval.Intersection(Value,<paramref name="restriction"/>)).
        /// </summary>
        internal void NarrowTo(IntegerInterval restriction, out bool success)
        {
            TrySetValue(IntegerInterval.Intersection(AllowableValues, restriction), out success);
        }
        
        private readonly Stack<IntegerInterval> _candidates = new Stack<IntegerInterval>();

        /// <summary>
        /// Narrows the variable until it has a unique value.
        /// </summary>
        internal override IEnumerable<IntegerInterval> UniqueValues
        {
            get
            {
                int mark = ConstraintThingySolver.SaveValues();

                // first resolve the constraints we have
                // (this gets us around the equality comparision in the TrySetValue which would force us
                // to try out a different value.

                bool success;

                ResolveInitialContraints(out success);

                if (success)
                {
                    // if our constraints can be satisfied, start with the resulting interval
                    _candidates.Push(AllowableValues);

                    ConstraintThingySolver.Restore(mark);
                }
                else
                {
                    
                    ConstraintThingySolver.Restore(mark);

                    if (_candidates.Count == 0)
                    {
                        yield break;
                    }
                }

                // while we still have candidate intervals
                while (_candidates.Count > 0)
                {
                    IntegerInterval value = _candidates.Pop();

                    // try it out
                    TrySetAndResolveConstraints(value, out success);

                    // if it worked
                    if (success)
                    {
                        // we can keep splitting down if required
                        if (!IsUnique)
                        {
                            IntegerInterval upper;
                            IntegerInterval lower;

                            AllowableValues.Split(AllowableValues.Center, out upper, out lower);

                            // and if we have random settings, we decide which one goes in first
                            if (ConstraintThingySolver.ExpansionOrder == ExpansionOrder.Random && ConstraintThingySolver.Random.CoinFlip())
                                Util.Swap(ref upper, ref lower);

                            // and push on the candidate stack to "recurse" through later
                            if (AllowableValues != upper) _candidates.Push(upper);

                            if (AllowableValues != lower) _candidates.Push(lower);
                        }
                        // if we're at a unique value, spit out a solution
                        else
                        {
                            // we found a valid, unique interval (a solution!)
                            yield return AllowableValues;
                        }
                    }

                    ConstraintThingySolver.Restore(mark);
                }
            }
        }

        private void ResolveInitialContraints(out bool success)
        {
            foreach (var constraint in Constraints)
            {
                constraint.QueueAllArcs();
            }

            ConstraintThingySolver.ResolveConstraints(out success);
        }
    }
}