using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ConstraintThingy;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ConstraintThingyTest
{
    [TestClass]
    public class IntegerVariableTests
    {
        [TestMethod]
        public void IntegerSumConstraint1()
        {
            ConstraintThingySolver solver = new ConstraintThingySolver();

            IntegerVariable x = new IntegerVariable(solver, "x", new IntegerInterval(1, 1));
            IntegerVariable y = new IntegerVariable(solver, "y", new IntegerInterval(1, 1));
            IntegerVariable z = new IntegerVariable(solver, "z", new IntegerInterval(2, 2));

            Constraint.Sum(z, x, y);

            int solutionCount = 0;

            foreach (var solution in solver.Solutions)
            {
                solutionCount++;

                Assert.AreEqual(z.CurrentValue, x.CurrentValue + y.CurrentValue);
            }

            Assert.AreEqual(1, solutionCount);
        }

        [TestMethod]
        public void IntegerSumConstraint2()
        {
            ConstraintThingySolver solver = new ConstraintThingySolver();

            IntegerVariable x = new IntegerVariable(solver, "x", new IntegerInterval(1, 1));
            IntegerVariable y = new IntegerVariable(solver, "y", new IntegerInterval(1, 4));
            IntegerVariable z = new IntegerVariable(solver, "z", new IntegerInterval(2, 5));

            Constraint.Sum(z, x, y);

            int solutionCount = 0;

            foreach (var solution in solver.Solutions)
            {
                solutionCount++;

                Assert.AreEqual(z.CurrentValue, x.CurrentValue + y.CurrentValue);
            }

            Assert.AreEqual(4, solutionCount);
        }

        [TestMethod]
        public void IntegerSumConstraint3()
        {
            ConstraintThingySolver solver = new ConstraintThingySolver();

            IntegerVariable x = new IntegerVariable(solver, "x", new IntegerInterval(1, 1));
            IntegerVariable y = new IntegerVariable(solver, "y", new IntegerInterval(1, 1));
            IntegerVariable z = new IntegerVariable(solver, "z", new IntegerInterval(10, 10));

            Constraint.Sum(z, x, y);

            Assert.AreEqual(0, solver.Solutions.Count());
        }

        [TestMethod]
        public void IntegerSumConstraint4()
        {
            ConstraintThingySolver solver = new ConstraintThingySolver();

            IntegerVariable x = new IntegerVariable(solver, "x", new IntegerInterval(1, 1));
            IntegerVariable y = new IntegerVariable(solver, "y", new IntegerInterval(1, 2));
            IntegerVariable z = new IntegerVariable(solver, "z", new IntegerInterval(2, 3));

            Constraint.Sum(z, x, y);

            int solutionCount = 0;

            foreach (var solution in solver.Solutions)
            {
                solutionCount++;

                Assert.AreEqual(z.CurrentValue, x.CurrentValue + y.CurrentValue);
            }

            Assert.AreEqual(2, solutionCount);
        }

        [TestMethod]
        public void IntegerSumConstraint5()
        {
            ConstraintThingySolver solver = new ConstraintThingySolver();

            IntegerVariable x = new IntegerVariable(solver, "x", new IntegerInterval(1, 1));
            IntegerVariable y = new IntegerVariable(solver, "y", new IntegerInterval(1, 2));
            IntegerVariable z = new IntegerVariable(solver, "z", new IntegerInterval(3, 3));

            Constraint.Sum(z, x, y);

            int solutionCount = 0;

            foreach (var solution in solver.Solutions)
            {
                solutionCount++;

                Assert.AreEqual(z.CurrentValue, x.CurrentValue + y.CurrentValue);
            }

            Assert.AreEqual(1, solutionCount);
        }

        [TestMethod]
        public void IntegerSumConstraint6()
        {
            ConstraintThingySolver solver = new ConstraintThingySolver();

            IntegerVariable x = new IntegerVariable(solver, "x", new IntegerInterval(1, 5));
            IntegerVariable y = new IntegerVariable(solver, "y", new IntegerInterval(1, 5));
            IntegerVariable z = new IntegerVariable(solver, "z", new IntegerInterval(-5, 15));

            Constraint.Sum(z, x, y);

            int solutionCount = 0;

            foreach (var solution in solver.Solutions)
            {
                solutionCount++;

                Assert.AreEqual(z.CurrentValue, x.CurrentValue + y.CurrentValue);
            }

            Assert.AreEqual(25, solutionCount);
        }

        [TestMethod]
        public void IntegerSumConstraint7()
        {
            ConstraintThingySolver solver = new ConstraintThingySolver();

            IntegerVariable x = new IntegerVariable(solver, "x", new IntegerInterval(-10, 20));
            IntegerVariable y = new IntegerVariable(solver, "y", new IntegerInterval(1, 5));
            IntegerVariable z = new IntegerVariable(solver, "z", new IntegerInterval(-5, 15));

            Constraint.Sum(z, x, y);

            int solutionCount = 0;

            foreach (var solution in solver.Solutions)
            {
                solutionCount++;

                Assert.AreEqual(z.CurrentValue, x.CurrentValue + y.CurrentValue);
            }

            Assert.AreEqual(105, solutionCount);
        }

        [TestMethod]
        public void IntegerSumConstraint8()
        {
            ConstraintThingySolver solver = new ConstraintThingySolver();

            IntegerVariable x = new IntegerVariable(solver, "x", new IntegerInterval(-15, -10));
            IntegerVariable y = new IntegerVariable(solver, "y", new IntegerInterval(1, 5));
            IntegerVariable z = new IntegerVariable(solver, "z", new IntegerInterval(-5, 15));

            Constraint.Sum(z, x, y);

            int solutionCount = 0;

            foreach (var solution in solver.Solutions)
            {
                solutionCount++;

                Assert.AreEqual(z.CurrentValue, x.CurrentValue + y.CurrentValue);
            }

            Assert.AreEqual(1, solutionCount);
        }

        [TestMethod]
        public void EightQueens()
        {
            const int n = 8;

            ConstraintThingySolver solver = new ConstraintThingySolver();

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

                    IntegerVariable kVar = new IntegerVariable(solver, null, new IntegerInterval(k,k));

                    Constraint.NotEqual(queens[i], queens[j]);
                    Constraint.NotEqual(queens[i], Constraint.Add(queens[j], kVar));
                    Constraint.NotEqual(queens[i], Constraint.Subtract(queens[j], kVar));
                }
            }

            int answerCounter = 0;

            foreach (var solution in solver.Solutions)
            {
                answerCounter++;
            }

            Assert.AreEqual(92, answerCounter);   
        }

        [TestMethod]
        public void IntegerDifferenceConstraint1()
        {
            ConstraintThingySolver solver = new ConstraintThingySolver();

            IntegerVariable x = new IntegerVariable(solver, "x", new IntegerInterval(-15, -15));
            IntegerVariable y = new IntegerVariable(solver, "y", new IntegerInterval(1, 5));
            IntegerVariable z = new IntegerVariable(solver, "z", new IntegerInterval(-20, -16));

            Constraint.Difference(z, x, y);

            int solutionCount = 0;

            foreach (var solution in solver.Solutions)
            {
                solutionCount++;

                Assert.AreEqual(z.CurrentValue, x.CurrentValue - y.CurrentValue);
            }

            Assert.AreEqual(5, solutionCount);
        }

        [TestMethod]
        public void IntegerDifferenceConstraint2()
        {
            ConstraintThingySolver solver = new ConstraintThingySolver();

            IntegerVariable x = new IntegerVariable(solver, "x", new IntegerInterval(0, 0));
            IntegerVariable y = new IntegerVariable(solver, "y", new IntegerInterval(5, 5));
            IntegerVariable z = new IntegerVariable(solver, "z", new IntegerInterval(-50, -5));

            Constraint.Difference(z, x, y);

            int solutionCount = 0;

            foreach (var solution in solver.Solutions)
            {
                solutionCount++;

                Assert.AreEqual(z.CurrentValue, x.CurrentValue - y.CurrentValue);
            }

            Assert.AreEqual(1, solutionCount);
        }

        [TestMethod]
        public void IntegerDifferenceConstraint3()
        {
            ConstraintThingySolver solver = new ConstraintThingySolver();

            IntegerVariable x = new IntegerVariable(solver, "x", new IntegerInterval(0, 0));
            IntegerVariable y = new IntegerVariable(solver, "y", new IntegerInterval(5, 5));
            IntegerVariable z = new IntegerVariable(solver, "z", new IntegerInterval(-50, -6));

            Constraint.Difference(z, x, y);

            Assert.AreEqual(0, solver.Solutions.Count());
        }

        [TestMethod]
        public void IntegerDifferenceConstraint4()
        {
            ConstraintThingySolver solver = new ConstraintThingySolver();

            IntegerVariable x = new IntegerVariable(solver, "x", new IntegerInterval(1, 5));
            IntegerVariable y = new IntegerVariable(solver, "y", new IntegerInterval(1, 5));
            IntegerVariable z = new IntegerVariable(solver, "z", new IntegerInterval(-10, 10));

            Constraint.Difference(z, x, y);

            int solutionCount = 0;

            foreach (var solution in solver.Solutions)
            {
                solutionCount++;

                Assert.AreEqual(z.CurrentValue, x.CurrentValue - y.CurrentValue);
            }

            Assert.AreEqual(25, solutionCount);
        }

        [TestMethod]
        public void IntegerDifferenceConstraint5()
        {
            ConstraintThingySolver solver = new ConstraintThingySolver();

            IntegerVariable x = new IntegerVariable(solver, "x", new IntegerInterval(1, 5));
            IntegerVariable y = new IntegerVariable(solver, "y", new IntegerInterval(1, 5));
            IntegerVariable z = new IntegerVariable(solver, "z", new IntegerInterval(-3, 10));

            Constraint.Difference(z, x, y);

            int solutionCount = 0;

            foreach (var solution in solver.Solutions)
            {
                solutionCount++;

                Assert.AreEqual(z.CurrentValue, x.CurrentValue - y.CurrentValue);
            }

            Assert.AreEqual(24, solutionCount);
        }
    }
}
