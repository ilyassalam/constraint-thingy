using System;
using System.Diagnostics;

namespace ConstraintThingy
{
    /// <summary>
    /// Constrains one variable to the the quotient of two others
    /// </summary>
    class RealQuotientConstraint : Constraint<RealVariable>
    {
        /// <summary>
        /// Creates a new interval quotient constraint
        /// </summary>
        public RealQuotientConstraint(RealVariable quotient, RealVariable dividend, RealVariable divisor)
            : base(new[] { quotient, dividend, divisor })
        {
        }

        private new RealVariable Quotient { get { return Variables[0]; } }

        private RealVariable Dividend { get { return Variables[1]; } }

        private RealVariable Divisor { get { return Variables[2]; } }
        
        protected internal override void UpdateVariable(RealVariable variable, out bool success)
        {
            MultiInterval result;

            // quotient * divisor = dividend
            
            if (variable == Dividend)
            {
                result = Quotient.AllowableValues.First * Divisor.AllowableValues.First;
            }

            // quotient = dividend / divisor
            else if (variable == Quotient)
            {
                result = Dividend.AllowableValues.First / Divisor.AllowableValues.First;
            }

            // divisor = dividend / quotient
            else if (variable == Divisor)
            {
                result = Dividend.AllowableValues.First / Quotient.AllowableValues.First;
            }
            else
            {
                throw new InvalidOperationException("Should not be able to reach this point.");
            }

            variable.NarrowTo(result, out success);
        }
    }
}