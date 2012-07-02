using System.Collections;
using System.Collections.Generic;

namespace ConstraintThingy
{
    /// <summary>
    /// Iterates through possible solutions
    /// </summary>
    public sealed class SolutionSet : IEnumerable<Solution>
    {
        private readonly IEnumerator<Solution> _solutions; 

        internal SolutionSet(IEnumerable<Solution> solutions)
        {
            _solutions = solutions.GetEnumerator();
        }

        /// <summary>
        /// The current solution
        /// </summary>
        public Solution CurrentSolution { get; private set; }

        /// <summary>
        /// Returns the next solution to the constraint system
        /// </summary>
        public Solution NextSolution()
        {
            if (_solutions.MoveNext())
            {
                CurrentSolution = _solutions.Current;
                return CurrentSolution;
            }
            return null;
        }

        public IEnumerator<Solution> GetEnumerator()
        {
            Solution nextSolution = NextSolution();

            while (nextSolution != null)
            {
                yield return nextSolution;

                nextSolution = NextSolution();
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}