using System;
using System.Diagnostics;

namespace ConstraintThingy
{
    /// <summary>
    /// Constraint that requires finite domain variables to have the same value.
    /// </summary>
    class MappingConstraint<T1, T2> : Constraint<Variable>
    {
        private readonly Mapping<T1, T2> _mapping; 

        /// <summary>
        /// Creates a new equality constraint
        /// </summary>
        public MappingConstraint(FiniteDomainVariable<T1> x, FiniteDomainVariable<T2> y, Mapping<T1, T2> mapping)
            : base(new Variable[] { x, y })
        {
            _mapping = mapping;

            if (x.FiniteDomain != mapping.Start || y.FiniteDomain != mapping.End) throw new InvalidOperationException("The finite domains on the variables are not the same as the provided mapping.");
        }

        protected internal override void UpdateVariable(Variable variable, out bool success)
        {
            if (variable == Variables[0])
            {
                int[] setIndices = (Variables[1] as FiniteDomainVariable<T2>).AllowableValues.GetSetIndices();

                UInt64 allowableValues = 0UL;

                foreach (var index in setIndices)
                {
                    allowableValues = allowableValues | _mapping.MapBackward(index);
                }

                (Variables[1] as FiniteDomainVariable<T2>).NarrowTo(allowableValues, out success);
            }
            else
            {
                Debug.Assert(variable == Variables[1]);

                int[] setIndices = (Variables[0] as FiniteDomainVariable<T1>).AllowableValues.GetSetIndices();

                UInt64 allowableValues = 0UL;

                foreach (var index in setIndices)
                {
                    allowableValues = allowableValues | _mapping.MapForward(index);
                }

                (Variables[1] as FiniteDomainVariable<T2>).NarrowTo(allowableValues, out success);
            }
        }
    }
}