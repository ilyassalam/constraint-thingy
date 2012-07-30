using System;
using System.Diagnostics;
using System.Linq;

namespace ConstraintThingy
{
    /// <summary>
    /// Contains one variable to be the minimum of the others
    /// </summary>
    class RealMinConstraint : Constraint<RealVariable>
    {
        /// <summary>
        /// Creates a new interval min constraint
        /// </summary>
        public RealMinConstraint(RealVariable min, params RealVariable[] variables)
            : base(new[] { min }.Concat(variables).ToArray())
        {
            Debug.Assert(variables.Length >= 2);
        }

        private new RealVariable Min { get { return Variables[0]; } }

        protected internal override void UpdateVariable(RealVariable variable, out bool success)
        {
            MultiInterval result;

            if (variable == Min)
            {
                result = Variables[1].AllowableValues.First;
                for (int i = 2; i < Variables.Length; i++)
                {
                    result = MultiInterval.Min(result, Variables[i].AllowableValues.First);
                }
            }
            else
            {
                // we can't be less than the 'min', so we bound ourselves to the range    (min, +infinity]
                result = Min.AllowableValues.First.Extend(double.PositiveInfinity);
            }

            variable.NarrowTo(result, out success);

            return;
        }
    }
}