using System;
using System.Diagnostics;

namespace ConstraintThingy
{
    /// <summary>
    /// Contains one variable to be the sum of two others
    /// </summary>
    class RealSumConstraint : Constraint<RealVariable>
    {
        /// <summary>
        /// Creates a new interval sum constraint
        /// </summary>
        public RealSumConstraint(RealVariable sum, RealVariable x, RealVariable y) : base(new [] { sum, x, y })
        {
        }

        private new RealVariable Sum { get { return Variables[0]; } }

        private RealVariable X { get { return Variables[1]; } }

        private RealVariable Y { get { return Variables[2]; } }

        protected internal override void UpdateVariable(RealVariable variable, out bool success)
        {
            MultiInterval result;

            // sum = x + y
            // x = sum - y
            // y = sum - x
            if (variable == Sum)
            {
                result = X.AllowableValues.First + Y.AllowableValues.First;
            }
            else if (variable == X)
            {
                result = Sum.AllowableValues.First - Y.AllowableValues.First;
            }
            else if (variable == Y)
            {
                result = Sum.AllowableValues.First - X.AllowableValues.First;
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