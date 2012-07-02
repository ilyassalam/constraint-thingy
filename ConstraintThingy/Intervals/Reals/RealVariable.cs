using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using CSharpUtils;
using CSharpUtils.Collections;

namespace ConstraintThingy
{
    public class RealVariable : Variable<LL<MultiInterval>>
    {
        /// <summary>
        /// Creates a new real variable with the specified name and allowable range
        /// </summary>
        public RealVariable(ConstraintThingySolver constraintThingySolver, string name, Interval allowableValues) : base(constraintThingySolver, name, LL.Create<MultiInterval>(allowableValues))
        {
            Precision = DefaultPrecision;
        }

        /// <summary>
        /// The default range of real variables
        /// </summary>
        public static readonly Interval DefaultRange = new Interval(-OneBillion, OneBillion);

        private const double OneBillion = 1000000000;

        /// <summary>
        /// The default precision of real variables
        /// </summary>
        public const double DefaultPrecision = 0.00001;

        private double _precision;

        /// <summary>
        /// How small the width of interval representing the variable's current value can be in order to be marked as 'solved'.
        /// </summary>
        public double Precision
        {
            get { return _precision; }
            set
            {
                if (!(value >= 0)) throw new ArgumentException("Precision must be greater than or equal to 0.");

                _precision = value;
            }
        }

        /// <summary>
        /// True if the variable has been narrowed to a single, unique value.
        /// </summary>
        public override bool IsUnique
        {
            get { return !AllowableValues.Empty && AllowableValues.First.IsUnique; }
        }

        /// <summary>
        /// True if the variable has been narrowed to an acceptable range.
        /// </summary>
        public bool IsSolved
        {
            get
            {
                return !AllowableValues.Empty && !AllowableValues.First.IsDisjoint && AllowableValues.First.Range <= Precision;
            }
        }

        /// <summary>
        /// True if the variable has been narrowed to the empty set.
        /// </summary>
        public override bool IsEmpty
        {
            get { return AllowableValues.Empty || AllowableValues.First.IsEmpty; }
        }

        /// <summary>
        /// Narrows an interval such that it is within <paramref name="restriction"/>. Simply a wrapper arround TrySetValue(Interval.Intersection(Value,<paramref name="restriction"/>)).
        /// </summary>
        internal void NarrowTo(MultiInterval restriction, out bool success)
        {
            TrySetValue(MultiInterval.Intersection(AllowableValues.First, restriction), out success);
        }

        internal void TrySetValue(MultiInterval interval, out bool succuss)
        {
            TrySetValue(AllowableValues.Rest.AddFront(interval), out succuss);
        }

        /// <summary>
        /// The current value of the interval
        /// </summary>
        public Interval UniqueValue
        {
            get
            {
                if (!AllowableValues.First.IsDisjoint) return AllowableValues.First[0];

                throw new InvalidOperationException("The multi-interval is disjoint.");
            }
        }

        /// <summary>
        /// Narrows the variable until it has a unique value.
        /// </summary>
        internal override IEnumerable<LL<MultiInterval>> UniqueValues
        {
            get
            {
                Debug.Assert(AllowableValues.Length == 1, "The real variable started with a stack that was already populated.");

                int mark = ConstraintThingySolver.SaveValues();

                // first resolve the constraints we have
                // this gets us around the equality comparision in the TrySetValue which would force the initial interval to be split
                // before actually propagating any constraints

                bool success;

                // first try to narrow constraints
                NarrowConstraints(out success);

                // if failure, we're done
                if (!success)
                {
                    ConstraintThingySolver.Restore(mark);
                    yield break;
                }

                // then try to resolve constraints
                ConstraintThingySolver.ResolveConstraints(out success);

                // if failure, we're done
                if (!success)
                {
                    ConstraintThingySolver.Restore(mark);
                    yield break;
                }

                // otherwise, start with the narrowed interval
                // since we succeeded, don't bother resetting (e.g. having to redo the work)
                LL<MultiInterval> nextTry = AllowableValues;

                // while we still have candidate intervals
                while (!nextTry.Empty)
                {
                    // try it out
                    TrySetAndResolveConstraints(nextTry, out success);

                    // if it worked
                    if (success)
                    {
                        // if we're sufficiently narrowed, spit out a solution
                        if (IsSolved)
                        {
                            // we found a valid, unique interval (a solution!)
                            yield return AllowableValues;
                        }

                        // we can keep splitting down if required
                        if (!AllowableValues.First.IsUnique)
                        {
                            MultiInterval[] splits = AllowableValues.First.Split();

                            // and if we have random settings, we decide which one goes in first
                            if (ConstraintThingySolver.ExpansionOrder == ExpansionOrder.Random)
                                splits.Shuffle(ConstraintThingySolver.Random);

                            nextTry = AllowableValues.Rest;

                            for (int i = 0; i < splits.Length; i++)
                            {
                                if (AllowableValues.First != splits[i]) nextTry = nextTry.AddFront(splits[i]);
                            }
                        }
                        else
                        {
                            nextTry = nextTry.Rest;
                        }
                    }
                    else
                    {
                        nextTry = nextTry.Rest;
                    }

                    ConstraintThingySolver.Restore(mark);
                }
            }
        }

        /// <summary>
        /// Creates and returns an real variable that represents the sum of <paramref name="a"/> and <paramref name="b"/>
        /// </summary>
        public static RealVariable operator +(RealVariable a, RealVariable b)
        {
            return Constraint.Add(a, b);
        }

        /// <summary>
        /// Creates and returns an real variable that represents the product of <paramref name="a"/> and <paramref name="b"/>
        /// </summary>
        public static RealVariable operator *(RealVariable a, RealVariable b)
        {
            return Constraint.Multiply(a, b);
        }

        /// <summary>
        /// Creates and returns an real variable that represents the quotient of <paramref name="dividend"/> and <paramref name="divisor"/>
        /// </summary>
        public static RealVariable operator /(RealVariable dividend, RealVariable divisor)
        {
            return Constraint.Divide(dividend, divisor);
        }

        /// <summary>
        /// Creates and returns an real variable that represents the difference of <paramref name="a"/> and <paramref name="b"/>
        /// </summary>
        public static RealVariable operator -(RealVariable a, RealVariable b)
        {
            return Constraint.Subtract(a, b);
        }
    }
}