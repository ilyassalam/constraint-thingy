using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSharpUtils;

namespace ConstraintThingy
{
    /// <summary>
    /// A variable whose value can take any value from a finite set of atomic values
    /// </summary>
    /// <typeparam name="T">The type of value in the finite domain</typeparam>
    public class FiniteDomainVariable<T> : Variable<UInt64>  
    {
        /// <summary>
        /// Creates a variable over a specified set of allowed values within a finite domain
        /// </summary>
        public FiniteDomainVariable(ConstraintThingySolver constraintThingySolver, string name, FiniteDomain<T> finiteDomain, IEnumerable<T> items) 
            : this(constraintThingySolver, name, finiteDomain, items.ToArray())
        {
            
        }

        /// <summary>
        /// Creates a variable over a specified set of allowed values within a finite domain
        /// </summary>
        public FiniteDomainVariable(ConstraintThingySolver constraintThingySolver, string name, FiniteDomain<T> finiteDomain, params T[] items) 
            : base(constraintThingySolver, name, finiteDomain.CreateBitSet(items))
        {
            FiniteDomain = finiteDomain;

            Priority = 1;
        }

        /// <summary>
        /// The finite domain associated with this variable
        /// </summary>
        public FiniteDomain<T> FiniteDomain { get; private set; }

        /// <summary>
        /// True if the variable has been narrowed to a single, unique value.
        /// </summary>
        public override bool IsUnique
        {
            get { return AllowableValues.IsSingleton(); }
        }

        /// <summary>
        /// True if the variable has been narrowed to the empty set.
        /// </summary>
        public override bool IsEmpty
        {
            get { return AllowableValues == 0UL; }
        }

        internal void NarrowTo(UInt64 value, out bool success)
        {
            TrySetValue(value & AllowableValues, out success);
        }

        /// <summary>
        /// Enumerates over all possible values of the variable
        /// </summary>
        public IEnumerable<T> PossibleValues
        {
            get
            {
                int[] setIndices = AllowableValues.GetSetIndices();

                for (int i = 0; i < setIndices.Length; i++)
                {
                    yield return FiniteDomain[setIndices[i]];
                }
            }
        }

        /// <summary>
        /// Returns the unique value of the variable
        /// </summary>
        public T UniqueValue
        {
            get
            {
                T[] values = PossibleValues.ToArray();

                if (values.Length == 1) return values[0];

                throw new InvalidOperationException("The variable was not narrowed to a unique value.");
            }
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            int[] setIndices = AllowableValues.GetSetIndices();

            builder.Append("{");

            builder.Append(FiniteDomain[setIndices[0]]);

            for (int i = 1; i < setIndices.Length; i++)
            {
                builder.Append("," + FiniteDomain[setIndices[i]]);
            }

            builder.Append("}");

            return builder.ToString();
        }

        internal override IEnumerable<UInt64> UniqueValues
        {
            get
            {
                int mark = ConstraintThingySolver.SaveValues();

                if (IsUnique)
                {
                    bool success;

                    // first try to narrow constraints
                    NarrowConstraints(out success);

                    // if failure, we're done
                    if (!success)
                    {
                        ConstraintThingySolver.Restore(mark);
                        yield break;
                    }

                    // then try to resolve constraints
                    ConstraintThingySolver.ResolveConstraints(out success);

                    if (!success)
                    {
                        ConstraintThingySolver.Restore(mark);
                        yield break;
                    }
                    
                    yield return AllowableValues;

                    ConstraintThingySolver.Restore(mark);
                }
                else
                {
                    // shuffle the set of indices in the finite domain
                    int count = FiniteDomain.Count;

                    int[] elementindices = new int[count];
                    for (int i = 0; i < count; i++)
                    {
                        elementindices[i] = i;
                    }

                    // if the options are set, we iterate over them in a random order
                    if (ConstraintThingySolver.ExpansionOrder == ExpansionOrder.Random)
                        elementindices.Shuffle(ConstraintThingySolver.Random);

                    foreach (var index in elementindices)
                    {
                        UInt64 candidate = BitHelper.GetMask(index);

                        if (AllowableValues.ContainsAny(candidate))
                        {
                            bool success;
                            TrySetAndResolveConstraints(candidate, out success);

                            if (success)
                                yield return AllowableValues;

                            ConstraintThingySolver.Restore(mark);
                        }
                    }
                }
            }
        }
    }
}
