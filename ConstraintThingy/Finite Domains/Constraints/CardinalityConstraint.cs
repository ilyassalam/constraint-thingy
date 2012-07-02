using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConstraintThingy
{
    /// <summary>
    /// Constraints the allowed number of occurences among a set of variables
    /// </summary>
    /// <typeparam name="T"></typeparam>
    class CardinalityConstraint<T> : FiniteDomainVariableConstraint<T>
    {
        /// <summary>
        /// Creates a new cardinality constraint
        /// </summary>
        public CardinalityConstraint(T value, int minOccurences, int maxOccurences, params FiniteDomainVariable<T>[] variables) 
            : base(variables)
        {
            Value = value;

            MinOccurences = minOccurences;

            MaxOccurences = maxOccurences;

            _set = variables[0].FiniteDomain.CreateBitSet(value);
        }

        /// <summary>
        /// The constrainted value
        /// </summary>
        public readonly T Value;

        private readonly UInt64 _set;

        /// <summary>
        /// The minimum number of occurences
        /// </summary>
        public readonly int MinOccurences;

        /// <summary>
        /// The maximum number of occurences
        /// </summary>
        public readonly int MaxOccurences;

        // we override 'narrowed' here because there is no narrowing to be done in 'update variable'. This is a boolean test.
        internal override void Narrowed(Variable narrowedVariable, out bool success)
        {
            int possible = 0;
            int definite = 0;

            for (int i = 0; i < Variables.Length; i++)
            {
                var variable = Variables[i];
                if ((_set & variable.AllowableValues) != 0UL)
                {
                    possible++;
                    if (variable.IsUnique)
                        definite++;
                }
            }

            if (possible < MinOccurences || definite > MaxOccurences)
            {
                success = false;
                return;
            }

            success = true;
            return;
        }

        protected internal override void UpdateVariable(FiniteDomainVariable<T> variable, out bool success)
        {
            throw new NotImplementedException();
        }
    }
}
