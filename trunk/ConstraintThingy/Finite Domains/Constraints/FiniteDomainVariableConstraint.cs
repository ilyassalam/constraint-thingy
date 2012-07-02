using System;

namespace ConstraintThingy
{
    abstract class FiniteDomainVariableConstraint<T> : Constraint<FiniteDomainVariable<T>>
    {
        protected FiniteDomainVariableConstraint(params FiniteDomainVariable<T>[] variables) : base(variables)
        {
            AssertSameFiniteDomains(variables);
        }

        private static void AssertSameFiniteDomains(FiniteDomainVariable<T>[] variables)
        {
            for (int i = 1; i < variables.Length; i++)
            {
                if (variables[0].FiniteDomain != variables[i].FiniteDomain)
                {
                    throw new InvalidOperationException("The finite domains of all involved variables must match");
                }
            }
        }
    }
}