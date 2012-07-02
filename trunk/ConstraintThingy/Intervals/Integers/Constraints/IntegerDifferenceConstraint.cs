using System;
using System.Diagnostics;

namespace ConstraintThingy
{
    class IntegerDifferenceConstraint : Constraint<IntegerVariable>
    {
        public IntegerDifferenceConstraint(IntegerVariable difference, IntegerVariable x, IntegerVariable y)
            : base(new[] { difference, x, y })
        {
        }

        private new IntegerVariable Difference { get { return Variables[0]; } }

        private IntegerVariable X { get { return Variables[1]; } }

        private IntegerVariable Y { get { return Variables[2]; } }

        protected internal override void UpdateVariable(IntegerVariable variable, out bool success)
        {
            IntegerInterval result;

            // difference = x - y
            // x = difference + y
            // y = x - difference
            if (variable == Difference)
            {
                result = X.AllowableValues - Y.AllowableValues;
            }
            else if (variable == X)
            {
                result = Difference.AllowableValues + Y.AllowableValues;
            }
            else if (variable == Y)
            {
                result = X.AllowableValues - Difference.AllowableValues;
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