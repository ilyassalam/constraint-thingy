using System.Diagnostics;

namespace ConstraintThingy
{
    /// <summary>
    /// Constrains two variables to be equal to one another
    /// </summary>
    class IntegerInequalityConstraint : Constraint<IntegerVariable>
    {
        /// <summary>
        /// Creates a interval equality constraint
        /// </summary>
        public IntegerInequalityConstraint(IntegerVariable a, IntegerVariable b)
            : base(new[] { a, b })
        {
        }

        private IntegerVariable Left { get { return Variables[0]; } }

        private IntegerVariable Right { get { return Variables[1]; } }

        protected internal override void UpdateVariable(IntegerVariable variable, out bool success)
        {
#warning this can probably be sped up with the ability to split intervals, but has the downside of we can't guarantee how many times the interval will be subdivided

            if (Left.IsUnique && Right.IsUnique)
            {
                if (Left.AllowableValues == Right.AllowableValues)
                {
                    success = false;
                    return;
                }
            }

            success = true;
        }
    }
}