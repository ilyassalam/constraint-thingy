using System;
using System.Linq;
using ConstraintThingy;

namespace ConstraintThingyPerformanceTesting
{
    abstract class PerformanceTest
    {
        protected abstract void InitializeConstraintSystem(ConstraintThingySolver solver);

        /// <summary>
        /// Solves for all solutions
        /// </summary>
        /// <returns>The number of solutions found</returns>
        public int SolveAll()
        {
            ConstraintThingySolver solver = new ConstraintThingySolver();
            InitializeConstraintSystem(solver);

            return solver.Solutions.Count();
        }

        /// <summary>
        /// Solves for all solutions, before repeating and restarting the process
        /// </summary>
        public void SolveForever()
        {
            while (true) SolveAll();
        }

        /// <summary>
        /// Solves for an initial solution, before repeating and restarting. If a solution cannot be found, false will be returned.
        /// </summary>
        public bool SolveInitialForever()
        {
            while (true)
            {
                var solution = SolveInitial();
                if (solution == null) 
                    return false;

                Console.WriteLine(solution);
            }
        }

        /// <summary>
        /// Solves for the first solution, returning true if one was found.
        /// </summary>
        /// <returns></returns>
        public Solution SolveInitial()
        {
            ConstraintThingySolver solver = new ConstraintThingySolver();
            InitializeConstraintSystem(solver);

            foreach (var solution in solver.Solutions)
            {
                return solution;
            }
            return null;
        }
    }
}