using System.Collections.Generic;
using System.Linq;
using ConstraintThingy;

namespace ConstraintThingyPerformanceTesting
{
    /// <summary>
    /// Lots of multiplication!
    /// </summary>
    class LotsOfMultiplication : PerformanceTest
    {
        protected override void InitializeConstraintSystem(ConstraintThingySolver solver)
        {
            List<RealVariable> variables = new List<RealVariable>();
 
            // solves 2 <= a*b*c*d*...z <= 10   where -10 <= a...z <= 10
            for (int i = 0; i < 25; i++)
            {
                variables.Add(solver.CreateRealVariable(-10, 10));
            }

            RealVariable multiplication = Constraint.Multiply(variables);

            Constraint.InRange(multiplication, 2, 10);
        }
    }
}