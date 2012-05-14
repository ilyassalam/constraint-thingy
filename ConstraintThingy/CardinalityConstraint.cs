using System;

namespace ConstraintThingy
{
    /// <summary>
    /// Restricts the number of times a specific value may occur in a set of finite domain variables
    /// </summary>
    public class CardinalityConstraint : Constraint<FiniteDomainVariable>
    {
        /// <summary>
        /// Restricts the number of times a specific value may occur in a set of finite domain variables
        /// </summary>
        public CardinalityConstraint(string value, int min, int max, params FiniteDomainVariable[] vars)
            : base(vars)
        {
            for (int i=1; i<vars.Length; i++)
                if (vars[i].Domain != vars[0].Domain)
                    throw new ArgumentException("Domains of constrained variables must be the same.");
            Value = value;
            Min = min;
            Max = max;
            valueBit = vars[0].Domain.Bitmask(Value);
        }

        /// <summary>
        /// Value of domain that whose frequency is restricted
        /// </summary>
        public readonly string Value;

        /// <summary>
        /// The value as a bitfield.
        /// </summary>
        private readonly UInt64 valueBit;

        /// <summary>
        /// Minimum number of times Value may occur
        /// </summary>
        public readonly int Min;
        /// <summary>
        /// Maximum number of times Value may occur
        /// </summary>
        public readonly int Max;

        /// <summary>
        /// Called when narrowedVariable is narrowed.
        /// Determine whether contraint is still satisfiable given the values of all participating variables.
        /// </summary>
        /// <param name="narrowedVariable"></param>
        public override void Narrowed(Variable narrowedVariable)
        {
            int possible = 0;
            int definite = 0;
            // Count up the number of variables that can/do have Value
            foreach (var v in Variables)
            {
                if (v.ContainsAny(valueBit))
                {
                    possible++;
                    if (v.IsUnique)
                        definite++;
                }
            }
            if (possible == Min)
            {
                // Force all variables that can have value to be that value.
                foreach (var v in Variables)

                    if (v.ContainsAny(valueBit))
                        v.Value = valueBit;
            }
            else if (possible < Min)
                throw new Failure("Too few possible occurances of Value in CardinalityConstraint");
            if (definite == Max)
            {
                // Rule out any remaining variables that are possible but not definite
                foreach (var v in Variables)

                    if (v.ContainsAny(valueBit) && !v.IsUnique)
                        v.Value &= ~valueBit;
            }
            else if (definite > Max)
                throw new Failure("Too many occurances of Value in CardinalityConstraint");
        }

        /// <summary>
        /// Not used for this type of constraint.
        /// </summary>
        public override void UpdateVariable(FiniteDomainVariable var)
        {
            // Should never reach this point.
            throw new NotImplementedException();
        }
    }
}
