using System;
using System.Diagnostics;

namespace ConstraintThingy
{
    /// <summary>
    /// Constraints one variable to be the product of two others
    /// </summary>
    class RealProductConstraint : Constraint<RealVariable>
    {
        /// <summary>
        /// Creates a new interval product constraint
        /// </summary>
        public RealProductConstraint(RealVariable product, RealVariable x, RealVariable y)
            : base(new[] { product, x, y })
        {
        }

        private new RealVariable Product { get { return Variables[0]; } }

        private RealVariable X { get { return Variables[1]; } }

        private RealVariable Y { get { return Variables[2]; } }

        protected internal override void UpdateVariable(RealVariable variable, out bool success)
        {
            MultiInterval result;

            // product = x * y
            if (variable == Product)
            {
                result = X.AllowableValues.First * Y.AllowableValues.First;
            }

            // x = product / y
            else if (variable == X)
            {
                result = Product.AllowableValues.First / Y.AllowableValues.First;
            }

            // y = product / x
            else if (variable == Y)
            {
                result = Product.AllowableValues.First / X.AllowableValues.First;
            }
            else
            {
                throw new InvalidOperationException("Should not be able to reach this point.");
            }

            variable.NarrowTo(result, out success);
        }
    }
}