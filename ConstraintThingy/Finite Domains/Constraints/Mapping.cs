using System;
using System.Collections;
using System.Collections.Generic;
using CSharpUtils.Collections;

namespace ConstraintThingy
{
    /// <summary>
    /// A surjective mapping from one finite domain set to another
    /// </summary>
    public sealed class Mapping<T1, T2> : IEnumerable<Pair<T1, T2>>
    {
        internal FiniteDomain<T1> Start { get; private set; }
        internal FiniteDomain<T2> End { get; private set; }

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
        
        /// <summary>
        /// Creates a new mapping between values in <paramref name="x"/> to values in <paramref name="y"/>
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public Mapping(FiniteDomain<T1> x, FiniteDomain<T2> y)
        {
            Start = x;
            End = y;

            _mapping = new UInt64[x.Count];
            _backwardMapping = new UInt64[y.Count];
        }

        /// <summary>
        /// Adds a new mapping between a value in the first finite domain to a value in the second finite domain
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        public void Add(T1 first, T2 second)
        {
            int forwardIndex = Start.IndexOf(first);
            int backwardIndex = End.IndexOf(second);

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
                    yield return new Pair<T1, T2>(Start[i], End[index]);
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}