using System;
using System.Collections.Generic;
using ConstraintThingy;

namespace ConstraintThingyPerformanceTesting
{
    /// <summary>
    /// Solves the eight queens problem via finite domains
    /// </summary>
    class EightQueens : PerformanceTest
    {
        protected override void InitializeConstraintSystem(ConstraintThingySolver solver)
        {
            FiniteDomainVariable<bool>[,] grid = new FiniteDomainVariable<bool>[8, 8];

            FiniteDomain<bool> finiteDomain = new FiniteDomain<bool>(true, false);

            List<FiniteDomainVariable<bool>> allCells = new List<FiniteDomainVariable<bool>>();

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    grid[i, j] = new FiniteDomainVariable<bool>(solver, String.Format("{0},{1}", i, j), finiteDomain, true, false);
                    allCells.Add(grid[i, j]);
                }
            }

            Constraint.RequireOccurences(true, 8, allCells.ToArray());

            // column / row constraints
            for (int i = 0; i < 8; i++)
            {
                List<FiniteDomainVariable<bool>> row = new List<FiniteDomainVariable<bool>>();
                List<FiniteDomainVariable<bool>> col = new List<FiniteDomainVariable<bool>>();

                for (int j = 0; j < 8; j++)
                {
                    row.Add(grid[i, j]);
                    col.Add(grid[j, i]);
                }

                Constraint.MaximumOccurences(true, 1, row.ToArray());
                Constraint.MaximumOccurences(true, 1, col.ToArray());
            }

            int n = 8;
            for (int slice = 0; slice < 2 * 8 - 1; ++slice)
            {
                List<FiniteDomainVariable<bool>> diagonal1 = new List<FiniteDomainVariable<bool>>();
                List<FiniteDomainVariable<bool>> diagonal2 = new List<FiniteDomainVariable<bool>>();

                int z = slice < n ? 0 : slice - n + 1;
                for (int j = z; j <= slice - z; ++j)
                {
                    diagonal1.Add(grid[j, slice - j]);
                    diagonal2.Add(grid[j, 7 - (slice - j)]);
                }

                Constraint.MaximumOccurences(true, 1, diagonal1.ToArray());
                Constraint.MaximumOccurences(true, 1, diagonal2.ToArray());
            }
        }
    }
}