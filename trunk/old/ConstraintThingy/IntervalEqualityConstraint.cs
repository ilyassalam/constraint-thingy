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
            InitializeVariables();
        }

        private IntervalVariable Left { get { return Variables[0]; } }
        private IntervalVariable Right { get { return Variables[1]; } }

        private void InitializeVariables()
        {
            Interval intersection = Interval.Intersection(Left.Value, Right.Value);

            if (intersection.IsEmpty) throw new Failure("There are no shared possible values between the two intervals.");

            Left.Value = intersection;
            Right.Value = intersection;
        }

        /// <summary>
        /// Updates variables when required
        /// </summary>
        public override void UpdateVariable(IntervalVariable var)
        {
            IntervalVariable otherVariable = (var == Left) ? Right : Left;

            if (var.Value.Contains(otherVariable.Value))
            {
                var.Value = otherVariable.Value;
            }
            else
            {
                throw new Failure("The other variable's value does not intersect this variable's range");
            }
        }
    }
}