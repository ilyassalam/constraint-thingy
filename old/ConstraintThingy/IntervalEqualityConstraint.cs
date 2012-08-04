using System;
using Intervals;

namespace ConstraintThingy
{
    /// <summary>
    /// Ensures that two variables are equal.
    /// </summary>
    public sealed class IntervalEqualityConstraint :  Constraint<IntervalVariable>
    {
        /// <summary>
        /// Creates a new equality constraint, ensuring that <paramref name="left"/> = <paramref name="right"/>.
        /// </summary>
        public IntervalEqualityConstraint(IntervalVariable left, IntervalVariable right) : base(new [] { left, right })
        {
            bool succeeded = true;
            InitializeVariables(ref succeeded);
            if (!succeeded)
                throw new Exception("IntervalEqualityConstraint is unsatisfiable even before narrowing.");
        }

        private IntervalVariable Left { get { return Variables[0]; } }
        private IntervalVariable Right { get { return Variables[1]; } }

        private void InitializeVariables(ref bool succeeded)
        {
            Interval intersection = Interval.Intersection(Left.Value, Right.Value);

            if (intersection.IsEmpty)
            {
                succeeded = false;
                return;
            }

            Left.TrySetValue(intersection, ref succeeded);
            if (!succeeded) return;
            Right.TrySetValue(intersection, ref succeeded);
        }

        /// <summary>
        /// Updates variables when required
        /// </summary>
        public override void UpdateVariable(IntervalVariable var, ref bool succeeded)
        {
            IntervalVariable otherVariable = (var == Left) ? Right : Left;

            if (var.Value.Contains(otherVariable.Value))
            {
                var.TrySetValue(otherVariable.Value, ref succeeded);
            }
            else
            {
                succeeded = false;
            }
        }
    }
}