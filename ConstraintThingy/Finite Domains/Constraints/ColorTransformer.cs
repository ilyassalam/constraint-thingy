namespace ConstraintThingy
{
    /// <summary>
    /// Constraint that requires finite domain variables to have the same value.
    /// </summary>
    class EqualityConstraint<T> : FiniteDomainVariableConstraint<T>
    {
        /// <summary>
        /// Creates a new equality constraint
        /// </summary>
        public EqualityConstraint(FiniteDomainVariable<T> x, FiniteDomainVariable<T> y)
            : base(new[] { x, y })
        {
        }

        protected internal override void UpdateVariable(FiniteDomainVariable<T> variable, out bool success)
        {
            FiniteDomainVariable<T> otherVariable = (variable == Variables[0]) ? Variables[1] : Variables[0];

            variable.NarrowTo(otherVariable.AllowableValues, out success);
        }

    }
}