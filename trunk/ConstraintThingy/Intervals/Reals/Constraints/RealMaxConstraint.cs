using System;
using System.Diagnostics;
using System.Linq;

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
        public RealMaxConstraint(RealVariable max, params RealVariable[] variables)
            : base(new[] { max }.Concat(variables).ToArray())
        {
            Debug.Assert(variables.Length >= 2);
        }

        private new RealVariable Max { get { return Variables[0]; } }

        protected internal override void UpdateVariable(RealVariable variable, out bool success)
        {
            MultiInterval result;

            if (variable == Max)
            {
                result = Variables[1].AllowableValues.First;
                for (int i = 2; i < Variables.Length; i++)
                {
                    result = MultiInterval.Max(result, Variables[i].AllowableValues.First);
                }
            }
            else
            {
                // we can't be greater than the 'max', so we bound ourselves to the range    (-infinity, max]
                result = Max.AllowableValues.First.Extend(double.NegativeInfinity);
            }

            variable.NarrowTo(result, out success);

            return;
        }
    }
}