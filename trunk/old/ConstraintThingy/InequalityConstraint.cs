using System;

namespace ConstraintThingy
{
    /// <summary>
    /// Constraint that requires variables to have different values.
    /// </summary>
    public class InequalityConstraint : Constraint<FiniteDomainVariable>
    {
        /// <summary>
        /// Establishes the constraint that X and Y must have different values
        /// </summary>
        public InequalityConstraint(FiniteDomainVariable x, FiniteDomainVariable y) : base(new [] { x, y })
        {
            if (x.Domain != y.Domain)
                throw new ArgumentException("Domains of variables in InequalityConstraint must match.");
        }

        /// <summary>
        /// Update this variable when the other has changed.
        /// We only update if the other variable has become unique. If so, we remove that value
        /// from this variable's potential values, and throw failure if that was its only possible value.
        /// </summary>
        /// <param name="var"></param>
        public override void UpdateVariable(FiniteDomainVariable var)
        {
            FiniteDomainVariable otherVariable = (var == Variables[0]) ? Variables[1] : Variables[0];
            if (otherVariable.IsUnique)
            {
                var.Value &= ~otherVariable.Value;
                if (var.IsEmpty)
                    throw new Failure("Inequality constraint failed.");
            }
        }
    }
}
