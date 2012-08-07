using System;
using System.Diagnostics;

namespace ConstraintThingy
{
    /// <summary>
    /// Restricts the number of times a specific value may occur in a set of finite domain variables
    /// </summary>
    [DebuggerDisplay("{Value}")]
    public class CardinalityConstraint : Constraint<FiniteDomainVariable>
    {
        /// <summary>
        /// Restricts the number of times a specific value may occur in a set of finite domain variables
        /// </summary>
        public CardinalityConstraint(string value, int min, int max, params FiniteDomainVariable[] vars)
            : base(vars)
        {
            for (int i = 1; i < vars.Length; i++)
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
        public override void Narrowed(Variable narrowedVariable, ref bool succeeded)
        {
            //if ((((FiniteDomainVariable)narrowedVariable).NarrowedElements & valueBit) == 0)
            //    return;
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
                    {
                        v.TrySetValue(valueBit, ref succeeded);
                        if (!succeeded)
                        {
                            return;
                        }
                    }
            }
            else if (possible < Min)
            {
                succeeded = false;
                return;
            }
            if (definite == Max)
            {
                // Rule out any remaining variables that are possible but not definite
                foreach (var v in Variables)

                    if (v.ContainsAny(valueBit) && !v.IsUnique)
                    {
                        v.TrySetValue(v.Value & ~valueBit, ref succeeded);
                        if (!succeeded)
                        {
                            return;
                        }
                    }
            }
            else if (definite > Max)
            {
                succeeded = false;
                return;
            }
        }

        /// <summary>
        /// Not used for this type of constraint.
        /// </summary>
        public override void UpdateVariable(FiniteDomainVariable var, ref bool succeeded)
        {
            // Should never reach this point.
            throw new NotImplementedException();
        }
    }
}
