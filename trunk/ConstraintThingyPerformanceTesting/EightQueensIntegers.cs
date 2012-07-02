using ConstraintThingy;

namespace ConstraintThingyPerformanceTesting
{
    /// <summary>
    /// Solves the eight queens problem via integers
    /// </summary>
    class EightQueensIntegers : PerformanceTest
    {
        protected override void InitializeConstraintSystem(ConstraintThingySolver solver)
        {
            const int n = 8;

            // i stole this formulation of N-queens from the Choco documentation

            IntegerVariable[] queens = new IntegerVariable[n];

            for (int i = 0; i < n; i++)
            {
                queens[i] = new IntegerVariable(solver, null, new IntegerInterval(1, n));
            }

            for (int i = 0; i < n; i++)
            {
                for (int j = i + 1; j < n; j++)
                {
                    int k = j - i;

                    IntegerVariable kVar = new IntegerVariable(solver, null, new IntegerInterval(k, k));

                    Constraint.NotEqual(queens[i], queens[j]);
                    Constraint.NotEqual(queens[i], Constraint.Add(queens[j], kVar));
                    Constraint.NotEqual(queens[i], Constraint.Subtract(queens[j], kVar));
                }
            }
        }
    }
}