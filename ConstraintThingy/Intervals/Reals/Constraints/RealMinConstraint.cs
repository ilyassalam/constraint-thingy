using System;
using System.Diagnostics;

namespace ConstraintThingy
{
    /// <summary>
    /// Contains one variable to be the minimum of two others
    /// </summary>
    class RealMinConstraint : Constraint<RealVariable>
    {
        /// <summary>
        /// Creates a new interval min constraint
        /// </summary>
        public RealMinConstraint(RealVariable min, RealVariable x, RealVariable y)
            : base(new[] { min, x, y })
        {
        }

        private new RealVariable Min { get { return Variables[0]; } }

        private RealVariable X { get { return Variables[1]; } }

        private RealVariable Y { get { return Variables[2]; } }

        protected internal override void UpdateVariable(RealVariable variable, out bool success)
        {
            MultiInterval result;

            if (variable == Min)
            {
                result = MultiInterval.Min(X.AllowableValues.First, Y.AllowableValues.First);
            }
            else if (variable == X)
            {
                // we can't be less than the 'min', so we bound ourselves to the range    (min, +infinity]
                result = Min.AllowableValues.First.Extend(double.PositiveInfinity);
            }
            else if (variable == Y)
            {
                // we can't be less than the 'min', so we bound ourselves to the range    (min, +infinity]
                result = Min.AllowableValues.First.Extend(double.PositiveInfinity);
            }
            else
            {
                throw new InvalidOperationException("Should not be able to reach this point.");
            }

            variable.NarrowTo(result, out success);

            return;
        }
    }
}