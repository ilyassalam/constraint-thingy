using System;
using Intervals;

namespace ConstraintThingy
{
    public class AdditionConstraint : Constraint<IntervalVariable>
    {
        /// <summary>
        /// Constrains <paramref name="sum"/> = <paramref name="a"/> + <paramref name="b"/>
        /// </summary>
        public AdditionConstraint(IntervalVariable sum, IntervalVariable a, IntervalVariable b)
            : base(new[] { sum, a, b })
        {
        }

        private IntervalVariable Sum { get { return Variables[0]; } }
        private IntervalVariable A { get { return Variables[1]; } }
        private IntervalVariable B { get { return Variables[2]; } }

        /// <summary>
        /// Called when one of the variables participating in the constraint is narrowed.
        /// </summary>
        public override void Narrowed(Variable narrowedVariable)
        {
            Interval intersection = NarrowConstraint(narrowedVariable);

            if (intersection.IsEmpty) throw new Failure("Empty interval.");
        }

        // narrows the value of a variable involved in the constraint
        private Interval NarrowConstraint(Variable narrowedVariable)
        {
            Interval intersection;

            // Sum = A + B
            if (narrowedVariable == Sum)
            {
                Interval sum = A.Value + B.Value;
                intersection = Interval.Intersection(sum, Sum.Value);
            }
            // A = Sum - B
            else if (narrowedVariable == A)
            {
                Interval a = Sum.Value - B.Value;
                intersection = Interval.Intersection(a, A.Value);
            }
            // B = Sum - A
            else if (narrowedVariable == B)
            {
                Interval b = Sum.Value - A.Value;
                intersection = Interval.Intersection(b, B.Value);
            }
            else
            {
                throw new InvalidOperationException("Unknown variable.");
            }

            return intersection;
        }

        /// <summary>
        /// Tries to narrow the variable based on the constraint.  Returns false if variable cannot be narrowed.
        /// </summary>
        public override void UpdateVariable(IntervalVariable var)
        {
            Interval intersection = NarrowConstraint(var);

            if (intersection.IsEmpty) throw new Failure("Empty interval.");

            var.Value = intersection;
        }
    }
}