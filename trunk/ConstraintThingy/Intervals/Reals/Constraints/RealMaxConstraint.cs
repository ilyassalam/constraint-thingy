using System;
using System.Diagnostics;

namespace ConstraintThingy
{
    /// <summary>
    /// Contains one variable to be the maximum of two others
    /// </summary>
    class RealMaxConstraint : Constraint<RealVariable>
    {
        /// <summary>
        /// Creates a new interval max constraint
        /// </summary>
        public RealMaxConstraint(RealVariable max, RealVariable x, RealVariable y)
            : base(new[] { max, x, y })
        {
        }

        private new RealVariable Max { get { return Variables[0]; } }

        private RealVariable X { get { return Variables[1]; } }

        private RealVariable Y { get { return Variables[2]; } }

        protected internal override void UpdateVariable(RealVariable variable, out bool success)
        {
            MultiInterval result;

            if (variable == Max)
            {
                result = MultiInterval.Max(X.AllowableValues.First, Y.AllowableValues.First);
            }
            else if (variable == X)
            {
                // we can't be greater than the 'max', so we bound ourselves to the range    (-infinity, max]
                result = Max.AllowableValues.First.Extend(double.NegativeInfinity);
            }
            else if (variable == Y)
            {
                // we can't be greater than the 'max', so we bound ourselves to the range    (-infinity, max]
                result = Max.AllowableValues.First.Extend(double.NegativeInfinity);
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