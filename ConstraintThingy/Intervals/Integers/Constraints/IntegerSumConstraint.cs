using System;
using System.Diagnostics;

namespace ConstraintThingy
{
    /// <summary>
    /// Contains one variable to be the sum of two others
    /// </summary>
    class IntegerSumConstraint : Constraint<IntegerVariable>
    {
        /// <summary>
        /// Creates a new interval sum constraint
        /// </summary>
        public IntegerSumConstraint(IntegerVariable sum, IntegerVariable x, IntegerVariable y)
            : base(new[] { sum, x, y })
        {
        }

        private new IntegerVariable Sum { get { return Variables[0]; } }

        private IntegerVariable X { get { return Variables[1]; } }

        private IntegerVariable Y { get { return Variables[2]; } }

        protected internal override void UpdateVariable(IntegerVariable variable, out bool success)
        {
            IntegerInterval result;

            // sum = x + y
            // x = sum - y
            // y = sum - x
            if (variable == Sum)
            {
                result = X.AllowableValues + Y.AllowableValues;
            }
            else if (variable == X)
            {
                result = Sum.AllowableValues - Y.AllowableValues;
            }
            else if (variable == Y)
            {
                result = Sum.AllowableValues - X.AllowableValues;
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