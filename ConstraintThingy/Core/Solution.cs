using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConstraintThingy
{
    /// <summary>
    /// A mapping of variables to values that represents a solution to a constraint system.
    /// </summary>
    public sealed class Solution : ConstraintComponent
    {
        /// <summary>
        /// The time it took the constraint solver to find the solution.
        /// </summary>
        public TimeSpan SolveTime { get; private set; }

        /// <summary>
        /// Represents the order in which the solutions were generated, starting at 0.
        /// </summary>
        public uint ID { get; private set; }

        internal Solution(ConstraintThingySolver constraintThingySolver, TimeSpan solveTime, uint solutionNumber) : base(constraintThingySolver)
        {
            SolveTime = solveTime;
            ID = solutionNumber;
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return String.Format("Solution #{0}, Solved in {1}", ID, SolveTime);
        }
    }
}