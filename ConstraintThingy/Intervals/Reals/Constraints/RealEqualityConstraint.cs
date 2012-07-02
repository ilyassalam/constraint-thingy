using System.Diagnostics;

namespace ConstraintThingy
{
    /// <summary>
    /// Constrains two variables to be equal to one another
    /// </summary>
    class RealEqualityConstraint : Constraint<RealVariable>
    {
        /// <summary>
        /// Creates a interval equality constraint
        /// </summary>
        public RealEqualityConstraint(RealVariable a, RealVariable b)
            : base(new[] { a, b })
        {
        }

        private RealVariable Left { get { return Variables[0]; } }

        private RealVariable Right { get { return Variables[1]; } }

        protected internal override void UpdateVariable(RealVariable variable, out bool success)
        {
            RealVariable otherVariable = (variable == Left) ? Right : Left;

            // narrow it to the value of the other variable (which will then propogate and make the other variable narrow to this variable)
            variable.NarrowTo(otherVariable.AllowableValues.First, out success);

            return;
        }
    }
}