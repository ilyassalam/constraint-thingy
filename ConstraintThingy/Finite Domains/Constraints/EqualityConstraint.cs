using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using CSharpUtils.Collections;

namespace ConstraintThingy
{
    /// <summary>
    /// A surjective mapping from one finite domain set to another
    /// </summary>
    public sealed class Mapping<T1, T2> : IEnumerable<Pair<T1, T2>>
    {
        private readonly FiniteDomain<T1> _start;
        private readonly FiniteDomain<T2> _end;

        private readonly UInt64[] _mapping;

        private readonly UInt64[] _backwardMapping;

        internal UInt64 MapForward(int i)
        {
            return _mapping[i];
        }

        internal UInt64 MapBackward(int i)
        {
            return _backwardMapping[i];
        }
        
        public Mapping(FiniteDomain<T1> x, FiniteDomain<T2> y)
        {
            _start = x;
            _end = y;

            _mapping = new UInt64[x.Count];
            _backwardMapping = new UInt64[y.Count];
        }

        public void Add(T1 first, T2 second)
        {
            int forwardIndex = _start.IndexOf(first);
            int backwardIndex = _end.IndexOf(second);

            _mapping[forwardIndex] = _mapping[forwardIndex] | BitHelper.GetMask(backwardIndex);

            _backwardMapping[backwardIndex] = _backwardMapping[backwardIndex] | BitHelper.GetMask(forwardIndex);
        }

        public IEnumerator<Pair<T1, T2>> GetEnumerator()
        {
            for (int i = 0; i < _mapping.Length; i++)
            {
                int[] setIndices = _mapping[i].GetSetIndices();

                foreach (var index in setIndices)
                {
                    yield return new Pair<T1, T2>(_start[i], _end[index]);
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

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