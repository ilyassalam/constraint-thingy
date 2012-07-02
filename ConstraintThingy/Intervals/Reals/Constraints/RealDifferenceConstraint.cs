using System;
using System.Diagnostics;

namespace ConstraintThingy
{
    class RealDifferenceConstraint : Constraint<RealVariable>
    {
        public RealDifferenceConstraint(RealVariable difference, RealVariable x, RealVariable y)
            : base(new[] { difference, x, y })
        {
        }

        private new RealVariable Difference { get { return Variables[0]; } }

        private RealVariable X { get { return Variables[1]; } }

        private RealVariable Y { get { return Variables[2]; } }

        protected internal override void UpdateVariable(RealVariable variable, out bool success)
        {
            MultiInterval result;

            // difference = x - y
            // x = difference + y
            // y = x - difference
            if (variable == Difference)
            {
                result = X.AllowableValues.First - Y.AllowableValues.First;
            }
            else if (variable == X)
            {
                result = Difference.AllowableValues.First + Y.AllowableValues.First;
            }
            else if (variable == Y)
            {
                result = X.AllowableValues.First - Difference.AllowableValues.First;
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