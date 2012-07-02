
using System;

namespace ConstraintThingy
{
    /// <summary>
    /// Constraint that requires finite domain variables to have different values.
    /// </summary>
    class InequalityConstraint<T> : FiniteDomainVariableConstraint<T>
    {
        public InequalityConstraint(FiniteDomainVariable<T> x, FiniteDomainVariable<T> y) : base(new [] { x, y })
        {
        }

        protected internal override void UpdateVariable(FiniteDomainVariable<T> variable, out bool success)
        {
            FiniteDomainVariable<T> otherVariable = (variable == Variables[0]) ? Variables[1] : Variables[0];

            if (otherVariable.IsUnique)
            {
                variable.NarrowTo(~otherVariable.AllowableValues, out success);

                if (!success) return;
            }

            success = true;
            return;
        }
    }
}