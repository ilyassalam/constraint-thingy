using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CSharpUtils.Collections;

namespace ConstraintThingy
{
    /// <summary>
    /// Assigns a real-valued interval value to each element of a finite domain
    /// </summary>
    public class ScoreMapping<T> : IEnumerable<Pair<T, Interval>>
    {
        /// <summary>
        /// The finite domain this score mapping is assigned to
        /// </summary>
        public FiniteDomain<T> FiniteDomain { get; private set; }

        private readonly Interval[] _scores;

        /// <summary>
        /// Creates a new score mapping
        /// </summary>
        public ScoreMapping(FiniteDomain<T> finitedomain, params Interval[] scores)
        {
            FiniteDomain = finitedomain;

            _scores = new Interval[FiniteDomain.Count];

            Array.Copy(scores, _scores, scores.Length);
        }

        /// <summary>
        /// Creates a new score mapping
        /// </summary>
        public ScoreMapping(FiniteDomain<T> finiteDomain, params Pair<T, Interval>[] scores)
            : this(finiteDomain, (IEnumerable<Pair<T, Interval>>)scores)
        {
            
        }
        
        /// <summary>
        /// Creates a new score mapping
        /// </summary>
        public ScoreMapping(FiniteDomain<T> finiteDomain, IEnumerable<Pair<T, Interval>> scores)
        {
            FiniteDomain = finiteDomain;

            _scores = new Interval[FiniteDomain.Count];

            foreach (var score in scores)
            {
                _scores[FiniteDomain.IndexOf(score.First)] = score.Second;
            }
        }

        /// <summary>
        /// Gets the score associated with <paramref name="item"/>
        /// </summary>
        public Interval this[T item]
        {
            get
            {
                return _scores[FiniteDomain.IndexOf(item)];
            }
        }

        /// <summary>
        /// Gets the score of the <paramref name="index"/>'th item in the finite domain
        /// </summary>
        public Interval this[int index]
        {
            get
            {
                return _scores[index];
            }
        }

        public IEnumerator<Pair<T, Interval>> GetEnumerator()
        {
            for (int i = 0; i < FiniteDomain.Count; i++)
            {
                yield return new Pair<T, Interval>(FiniteDomain[i], _scores[i]);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}