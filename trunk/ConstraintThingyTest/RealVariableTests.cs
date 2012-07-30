using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using CSharpUtils;
using ConstraintThingy;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ConstraintThingyTest
{
    [TestClass]
    public partial class RealVariableTests
    {
        /// <summary>
        /// Because intervals only assert that a solution exists somewhere within the interval, we can only test if two intervals intersect
        /// </summary>
        private static void AssertIntersect(Interval a, Interval b)
        {
            bool intersects = Interval.Intersects(a, b);
            Assert.IsTrue(intersects, "Expected {0} to intersect {1}.", a, b);
        }

        [TestMethod]
        public void EqualityConstraint()
        {
            ConstraintThingySolver solver = new ConstraintThingySolver();

            RealVariable x = new RealVariable(solver, "x", new Interval(0, 10));
            RealVariable y = new RealVariable(solver, "y", new Interval(0, 15));

            Constraint.Equal(x, y);

            foreach (var solution in solver.Solutions.FirstElements(10))
            {
                Assert.AreEqual(x.UniqueValue, y.UniqueValue);
            }
        }

        [TestMethod]
        public void EqualityConstraint2()
        {
            ConstraintThingySolver solver = new ConstraintThingySolver();

            RealVariable x = new RealVariable(solver, "x", new Interval(0, 15));
            RealVariable y = new RealVariable(solver, "y", new Interval(10, 15));

            Constraint.Equal(x, y);

            foreach (var solution in solver.Solutions.FirstElements(10))
            {
                Assert.AreEqual(x.UniqueValue, y.UniqueValue);
            }
        }

        [TestMethod]
        public void EqualityConstraint3()
        {
            ConstraintThingySolver solver = new ConstraintThingySolver();

            RealVariable x = new RealVariable(solver, "x", new Interval(0, 9));
            RealVariable y = new RealVariable(solver, "y", new Interval(10, 15));

            Constraint.Equal(x, y);

            Assert.AreEqual(0, solver.Solutions.Count());
        }

        [TestMethod]
        public void EqualityConstraint4()
        {
            ConstraintThingySolver solver = new ConstraintThingySolver();

            RealVariable x = new RealVariable(solver, "x", new Interval(0, 11));
            RealVariable y = new RealVariable(solver, "y", new Interval(10, 15));
            RealVariable z = new RealVariable(solver, "z", new Interval(10, 11));

            Constraint.Equal(x, y);
            Constraint.Equal(y, z);

            foreach (var solution in solver.Solutions.FirstElements(10))
            {
                Assert.AreEqual(x.UniqueValue, y.UniqueValue);
                Assert.AreEqual(y.UniqueValue, z.UniqueValue);
            }
        }

        [TestMethod]
        public void SumConstraint()
        {
            ConstraintThingySolver solver = new ConstraintThingySolver();

            RealVariable x = new RealVariable(solver, "x", new Interval(0, 10));
            RealVariable y = new RealVariable(solver, "y", new Interval(10, 15));
            RealVariable sum = new RealVariable(solver, "sum", new Interval(0, 15));

            Constraint.Sum(sum, x, y);

            foreach (var solution in solver.Solutions.FirstElements(10))
            {
                AssertIntersect(sum.UniqueValue, x.UniqueValue + y.UniqueValue);
            }
        }

        [TestMethod]
        public void SumConstraint2()
        {
            ConstraintThingySolver solver = new ConstraintThingySolver();

            RealVariable x = new RealVariable(solver, "x", new Interval(0, 10));
            RealVariable y = new RealVariable(solver, "y", new Interval(10, 2000));
            RealVariable sum = new RealVariable(solver, "sum", new Interval(0, 20));

            Constraint.Sum(sum, x, y);

            foreach (var solution in solver.Solutions.FirstElements(10))
            {
                AssertIntersect(sum.UniqueValue, x.UniqueValue + y.UniqueValue);
            }
        }

        [TestMethod]
        public void SumConstraint3()
        {
            ConstraintThingySolver solver = new ConstraintThingySolver();

            RealVariable x = new RealVariable(solver, "x", new Interval(0, 1));
            RealVariable y = new RealVariable(solver, "y", new Interval(1, 5));
            RealVariable sum = new RealVariable(solver, "sum", new Interval(0, 20));

            Constraint.Sum(sum, x, y);

            foreach (var solution in solver.Solutions.FirstElements(10))
            {
                AssertIntersect(sum.UniqueValue, x.UniqueValue + y.UniqueValue);
            }
        }

        [TestMethod]
        public void SumConstraint4()
        {
            ConstraintThingySolver solver = new ConstraintThingySolver();

            RealVariable x = new RealVariable(solver, "x", new Interval(-10, 10));
            RealVariable y = new RealVariable(solver, "y", new Interval(10, 10));
            RealVariable sum = new RealVariable(solver, "sum", new Interval(0, 20));

            Constraint.Sum(sum, x, y);

            foreach (var solution in solver.Solutions.FirstElements(10))
            {
                AssertIntersect(sum.UniqueValue, x.UniqueValue + y.UniqueValue);
            }
        }

        [TestMethod]
        public void SumConstraint5()
        {
            ConstraintThingySolver solver = new ConstraintThingySolver();

            RealVariable x = new RealVariable(solver, "x", new Interval(-20, -10));
            RealVariable y = new RealVariable(solver, "y", new Interval(-10, 0));
            RealVariable sum = new RealVariable(solver, "sum", new Interval(-20, -15));

            Constraint.Sum(sum, x, y);

            foreach (var solution in solver.Solutions.FirstElements(10))
            {
                AssertIntersect(sum.UniqueValue, x.UniqueValue + y.UniqueValue);
            }
        }

        [TestMethod]
        public void SumConstraint6()
        {
            ConstraintThingySolver solver = new ConstraintThingySolver();

            RealVariable x = new RealVariable(solver, "x", new Interval(-20, -10));
            RealVariable y = new RealVariable(solver, "y", new Interval(-10, 0));

            RealVariable z = new RealVariable(solver, "z", new Interval(-100, 100));

            RealVariable sum = new RealVariable(solver, "sum", new Interval(-30, -10));

            RealVariable sum2 = new RealVariable(solver, "sum2", new Interval(-200, 0));

            Constraint.Sum(sum, x, y);

            Constraint.Sum(sum2, sum, z);

            foreach (var solution in solver.Solutions.FirstElements(10))
            {
                AssertIntersect(sum.UniqueValue, x.UniqueValue + y.UniqueValue);
                AssertIntersect(sum2.UniqueValue, sum.UniqueValue + z.UniqueValue);
            }
        }

        [TestMethod]
        public void ProductConstraint()
        {
            ConstraintThingySolver solver = new ConstraintThingySolver();

            RealVariable x = new RealVariable(solver, "x", new Interval(1, 5));
            RealVariable y = new RealVariable(solver, "y", new Interval(2, 6));

            RealVariable product = x * y;

            foreach (var solution in solver.Solutions.FirstElements(10))
            {
                AssertIntersect(product.UniqueValue, x.UniqueValue * y.UniqueValue);
            }
        }

        [TestMethod]
        public void ProductConstraint2()
        {
            ConstraintThingySolver solver = new ConstraintThingySolver();

            RealVariable x = new RealVariable(solver, "x", new Interval(1, 5));
            RealVariable y = new RealVariable(solver, "y", new Interval(2, 6));

            RealVariable product = x * y;

            Constraint.InRange(product, 28, 30);

            foreach (var solution in solver.Solutions.FirstElements(10))
            {
                AssertIntersect(product.UniqueValue, x.UniqueValue * y.UniqueValue);
            }
        }

        [TestMethod]
        public void ProductConstraint3()
        {
            ConstraintThingySolver solver = new ConstraintThingySolver();

            RealVariable x = new RealVariable(solver, "x", new Interval(-5, 5));
            RealVariable y = new RealVariable(solver, "y", new Interval(2, 6));

            RealVariable product = x * y;

            foreach (var solution in solver.Solutions.FirstElements(10))
            {
                AssertIntersect(product.UniqueValue, x.UniqueValue * y.UniqueValue);
            }
        }

        [TestMethod]
        public void RandomProductsContainingZero()
        {
            for(int i = 0; i < 1000; i++)
            {
                RandomProductsContainingZero(i);
            }
        }

        [TestMethod]
        public void RandomProductsIdentifiedFailureCase()
        {
            RandomProductsContainingZero(3);
        }

        public void RandomProductsContainingZero(int seed)
        {
            ConstraintThingySolver solver = new ConstraintThingySolver(seed);

            RealVariable x = new RealVariable(solver, "x", new Interval(-5, 5));
            RealVariable y = new RealVariable(solver, "y", new Interval(2, 6));

            RealVariable product = x * y;

            foreach (var solution in solver.Solutions.FirstElements(10))
            {
                AssertIntersect(product.UniqueValue, x.UniqueValue * y.UniqueValue);
            }
        }

        [TestMethod]
        public void ProductConstraint4()
        {
            ConstraintThingySolver solver = new ConstraintThingySolver();

            RealVariable x = new RealVariable(solver, "x", new Interval(-5, 5));
            RealVariable y = new RealVariable(solver, "y", new Interval(-6, 6));

            RealVariable product = x * y;

            Constraint.InRange(product, -30, -28);

            foreach (var solution in solver.Solutions.FirstElements(10))
            {
                AssertIntersect(product.UniqueValue, x.UniqueValue * y.UniqueValue);
            }
        }

        [TestMethod]
        public void ProductConstraint5()
        {
            ConstraintThingySolver solver = new ConstraintThingySolver();

            // example from the paper, each variable represents the probability of survival in each room

            RealVariable x = new RealVariable(solver, "x", new Interval(.1f, .95f));
            RealVariable x1 = new RealVariable(solver, "x1", new Interval(.1f, .95f));
            RealVariable x2 = new RealVariable(solver, "x2", new Interval(.1f, .95f));
            RealVariable x3 = new RealVariable(solver, "x3", new Interval(.1f, .95f));
            RealVariable x4 = new RealVariable(solver, "x4", new Interval(.1f, .95f));

            RealVariable product = x * x1 * x2 * x3 * x4;

            // probability of surviving the whole thing
            Constraint.InRange(product, .65f, 1f);

            foreach (var solution in solver.Solutions.FirstElements(10))
            {
                AssertIntersect(product.UniqueValue, 
                    x.UniqueValue * 
                    x1.UniqueValue * 
                    x2.UniqueValue *
                    x3.UniqueValue *
                    x4.UniqueValue);
            }
        }

        [TestMethod]
        public void ProductConstraint6()
        {
            ConstraintThingySolver solver = new ConstraintThingySolver();

            RealVariable x = new RealVariable(solver, "x", new Interval(-1, 1));
            RealVariable y = new RealVariable(solver, "y", new Interval(50, 50));

            RealVariable product = x * y;

            Constraint.InRange(product, -51, -49);

            foreach (var solution in solver.Solutions.FirstElements(10))
            {
                AssertIntersect(
                    product.UniqueValue,
                    x.UniqueValue *
                    y.UniqueValue);
            }
        }

        [TestMethod]
        public void ProductConstraint7()
        {
            ConstraintThingySolver solver = new ConstraintThingySolver();

            RealVariable x = new RealVariable(solver, "x", new Interval(-1, 1));
            RealVariable y = new RealVariable(solver, "y", new Interval(50, 50));

            RealVariable product = x * y;

            Constraint.InRange(product, 49, 51);

            foreach (var solution in solver.Solutions.FirstElements(10))
            {
                AssertIntersect(
                    product.UniqueValue,
                    x.UniqueValue *
                    y.UniqueValue);
            }
        }

        [TestMethod]
        public void ProductConstraint8()
        {
            ConstraintThingySolver solver = new ConstraintThingySolver();

            RealVariable x = new RealVariable(solver, "x", new Interval(-1, 1));
            RealVariable y = new RealVariable(solver, "y", new Interval(-50, 50));

            RealVariable product = x * y;

            Constraint.InRange(product, 49, 51);

            foreach (var solution in solver.Solutions.FirstElements(10))
            {
                AssertIntersect(
                    product.UniqueValue,
                    x.UniqueValue *
                    y.UniqueValue);
            }
        }


        [TestMethod]
        public void ProductConstraint9()
        {
            ConstraintThingySolver solver = new ConstraintThingySolver();

            List<RealVariable> variables = new List<RealVariable>();

            for (int i = 0; i < 5; i++)
            {
                variables.Add(solver.CreateRealVariable(-1, 1));
            }

            RealVariable product = Constraint.Multiply(variables);

            Constraint.InRange(product, -1, -.5);

            foreach (var solution in solver.Solutions.FirstElements(10))
            {
                AssertIntersect(
                    product.UniqueValue,
                    variables.Select(v => v.UniqueValue).Aggregate((a,b) => a * b));
            }
        }

        [TestMethod]
        public void ProductConstraint10()
        {
            ConstraintThingySolver solver = new ConstraintThingySolver();

            List<RealVariable> variables = new List<RealVariable>();

            for (int i = 0; i < 7; i++)
            {
                variables.Add(solver.CreateRealVariable(-1, 1));
            }

            RealVariable product = Constraint.Multiply(variables);

            Constraint.InRange(product, -1, -.5);

            foreach (var solution in solver.Solutions.FirstElements(10))
            {
                AssertIntersect(
                    product.UniqueValue,
                    variables.Select(v => v.UniqueValue).Aggregate((a, b) => a * b));
            }
        }

        [TestMethod]
        public void ProductConstraint11()
        {
            ConstraintThingySolver solver = new ConstraintThingySolver();

            List<RealVariable> variables = new List<RealVariable>();

            for (int i = 0; i < 8; i++)
            {
                variables.Add(solver.CreateRealVariable(-1, 1));
            }

            RealVariable product = Constraint.Multiply(variables);

            Constraint.InRange(product, -1, -.5);

            foreach (var solution in solver.Solutions.FirstElements(10))
            {
                AssertIntersect(
                    product.UniqueValue,
                    variables.Select(v => v.UniqueValue).Aggregate((a, b) => a * b));
            }
        }

        [TestMethod]
        public void ProductConstraint12()
        {
            ConstraintThingySolver solver = new ConstraintThingySolver();

            List<RealVariable> variables = new List<RealVariable>();

            for (int i = 0; i < 9; i++)
            {
                variables.Add(solver.CreateRealVariable(-1, 1));
            }

            RealVariable product = Constraint.Multiply(variables);

            Constraint.InRange(product, -1, .5);

            foreach (var solution in solver.Solutions.FirstElements(10))
            {
                AssertIntersect(
                    product.UniqueValue,
                    variables.Select(v => v.UniqueValue).Aggregate((a, b) => a * b));
            }
        }

        [TestMethod]
        public void ProductConstraint13()
        {
            ConstraintThingySolver solver = new ConstraintThingySolver();

            var a = solver.CreateRealVariable(-100, -50);

            var b = solver.CreateRealVariable(50, 100);

            // two negatives can't possibly make a positive
            var c = a*b;

            Constraint.InRange(c, 1, 342342);

            Assert.AreEqual(0, solver.Solutions.Count());
        }

        [TestMethod]
        public void ProductConstraint14()
        {
            ConstraintThingySolver solver = new ConstraintThingySolver();

            var a = solver.CreateRealVariable(-100, -50);

            var b = solver.CreateRealVariable(50, 100);

            // two negatives can't possibly make a positive
            var c = a * b;

            Constraint.InRange(c, -342342, 3);

            foreach (var solution in solver.Solutions.FirstElements(10))
            {
                AssertIntersect(
                    c.UniqueValue,
                    a.UniqueValue *
                    b.UniqueValue);
            }
        }


        [TestMethod]
        public void DifferenceConstraint()
        {
            ConstraintThingySolver solver = new ConstraintThingySolver();

            RealVariable x = new RealVariable(solver, "x", new Interval(1, 5));
            RealVariable y = new RealVariable(solver, "y", new Interval(2, 6));

            RealVariable difference = x - y;

            Constraint.InRange(difference, -1, 0);

            foreach (var solution in solver.Solutions.FirstElements(10))
            {
                AssertIntersect(difference.UniqueValue, x.UniqueValue - y.UniqueValue);
            }
        }

        [TestMethod]
        public void DifferenceConstraint2()
        {
            ConstraintThingySolver solver = new ConstraintThingySolver();

            RealVariable x = new RealVariable(solver, "x", new Interval(1, 5));
            RealVariable y = new RealVariable(solver, "y", new Interval(2, 6));
            RealVariable z = new RealVariable(solver, "z", new Interval(2, 6));

            RealVariable difference = x - y - z;

            Constraint.InRange(difference, -1, 0);

            foreach (var solution in solver.Solutions.FirstElements(10))
            {
                AssertIntersect(difference.UniqueValue, x.UniqueValue - y.UniqueValue - z.UniqueValue);
            }
        }

        [TestMethod]
        public void DifferenceConstraint3()
        {
            ConstraintThingySolver solver = new ConstraintThingySolver();

            RealVariable x = new RealVariable(solver, "x", new Interval(1, 5));
            RealVariable y = new RealVariable(solver, "y", new Interval(-500, -250));

            RealVariable difference = x - y;

            foreach (var solution in solver.Solutions.FirstElements(10))
            {
                AssertIntersect(difference.UniqueValue, x.UniqueValue - y.UniqueValue);
            }
        }

        [TestMethod]
        public void DifferenceConstraint4()
        {
            ConstraintThingySolver solver = new ConstraintThingySolver();

            RealVariable x = new RealVariable(solver, "x", new Interval(1, 5));
            RealVariable y = new RealVariable(solver, "y", new Interval(-500, -250));

            RealVariable difference = x - y;

            Constraint.InRange(difference, -1, 0);

            Assert.AreEqual(0, solver.Solutions.Count());
        }

        [TestMethod]
        public void QuotientConstraint()
        {
            ConstraintThingySolver solver = new ConstraintThingySolver();

            RealVariable x = new RealVariable(solver, "x", new Interval(-1, 1));
            RealVariable y = new RealVariable(solver, "y", new Interval(-50, 50));

            RealVariable product = x / y;

            foreach (var solution in solver.Solutions.FirstElements(10))
            {
                AssertIntersect(
                    product.UniqueValue,
                    x.UniqueValue /
                    y.UniqueValue);
            }
        }

        [TestMethod]
        public void QuotientConstraint2()
        {
            ConstraintThingySolver solver = new ConstraintThingySolver();

            RealVariable x = new RealVariable(solver, "x", new Interval(-1500, 1));
            RealVariable y = new RealVariable(solver, "y", new Interval(-50, 5000));

            RealVariable product = x / y;

            Constraint.InRange(product, -32, -30);

            foreach (var solution in solver.Solutions.FirstElements(10))
            {
                AssertIntersect(
                    product.UniqueValue,
                    x.UniqueValue /
                    y.UniqueValue);
            }
        }

        [TestMethod]
        public void RandomDistanceSquaredTests()
        {
            Random random = new Random();

            for (int i = 0; i < 100; i++)
            {
                double min = random.NextDouble(0, 1000);
                double max = random.NextDouble(min, min + 100);

                DistanceSquared(i, min, max);
            }
        }

        [TestMethod]
        public void RandomDistanceSquared1()
        {
            DistanceSquared(0, 0, 100);
        }

        [TestMethod]
        public void RandomDistanceSquared2()
        {
            DistanceSquared(0, 0, 100);
        }

        [TestMethod]
        public void RandomDistanceSquared3()
        {
            DistanceSquared(0, 0, 100);
        }

        [TestMethod]
        public void RandomDistanceSquared4()
        {
            DistanceSquared(0, 0, 100);
        }

        [TestMethod]
        public void RandomDistanceSquared5()
        {
            DistanceSquared(0, 0, 100);
        }

        [TestMethod]
        public void RandomDistanceSquared6()
        {
            DistanceSquared(0, 0, 100);
        }

        [TestMethod]
        public void RandomDistanceSquared7()
        {
            DistanceSquared(0, 0, 100);
        }

        [TestMethod]
        public void RandomDistanceSquared8()
        {
            DistanceSquared(0, 0, 100);
        }

        [TestMethod]
        public void RandomDistanceSquared9()
        {
            DistanceSquared(0, 0, 100);
        }

        [TestMethod]
        public void RandomDistanceSquared10()
        {
            DistanceSquared(0, 0, 100);
        }

        [TestMethod]
        public void RandomDistanceSquared11()
        {
            DistanceSquared(0, 0, 100);
        }

        [TestMethod]
        public void RandomDistanceSquared12()
        {
            DistanceSquared(0, 0, 100);
        }

        [TestMethod]
        public void RandomDistanceSquared13()
        {
            DistanceSquared(0, 0, 100);
        }

        [TestMethod]
        public void RandomDistanceSquared14()
        {
            DistanceSquared(0, 0, 100);
        }

        [TestMethod]
        public void RandomDistanceSquared15()
        {
            DistanceSquared(0, 0, 100);
        }

        [TestMethod]
        public void RandomDistanceSquared16()
        {
            DistanceSquared(0, 0, 100);
        }

        private static void DistanceSquared(int seed, double minDist, double maxDist)
        {
            ConstraintThingySolver solver = new ConstraintThingySolver(seed);

            RealVariable x1 = solver.CreateRealVariable();
            RealVariable y1 = solver.CreateRealVariable();

            RealVariable x2 = solver.CreateRealVariable();
            RealVariable y2 = solver.CreateRealVariable();

            RealVariable distanceSquared = Constraint.DistanceSquared(new Vector2Variable(x1, y1), new Vector2Variable(x2, y2));

            Constraint.InRange(distanceSquared, minDist, maxDist);

            foreach (var solution in solver.Solutions.FirstElements(10))
            {
                Interval x1Val = x1.UniqueValue;
                Interval x2Val = x2.UniqueValue;

                Interval y1Val = y1.UniqueValue;
                Interval y2Val = y2.UniqueValue;

                Interval x2MinusX1Val = x2Val - x1Val;

                Interval y2MinusY1Val = y2Val - y1Val;

                Interval distanceSquaredVal = x2MinusX1Val*x2MinusX1Val + y2MinusY1Val*y2MinusY1Val;

                AssertIntersect(
                    distanceSquared.UniqueValue,
                    distanceSquaredVal);
            }
        }

        [TestMethod]
        public void DistanceSquared2()
        {
            ConstraintThingySolver solver = new ConstraintThingySolver();

            RealVariable x1 = solver.CreateRealVariable();
            RealVariable y1 = solver.CreateRealVariable();

            RealVariable x2 = solver.CreateRealVariable();
            RealVariable y2 = solver.CreateRealVariable();

            RealVariable distanceSquared = Constraint.DistanceSquared(new Vector2Variable(x1, y1), new Vector2Variable(x2, y2));

            Constraint.InRange(distanceSquared, -150, -100);

            Assert.AreEqual(0, solver.Solutions.Count());
        }

        [TestMethod]
        public void DistanceSquared3()
        {
            ConstraintThingySolver solver = new ConstraintThingySolver();

            RealVariable x1 = solver.CreateRealVariable();
            RealVariable y1 = solver.CreateRealVariable();

            RealVariable x2 = solver.CreateRealVariable();
            RealVariable y2 = solver.CreateRealVariable();

            RealVariable distanceSquared = Constraint.DistanceSquared(new Vector2Variable(x1, y1), new Vector2Variable(x2, y2));

            Constraint.InRange(distanceSquared, -5000, -2000);

            int numSolutions = 0;

            foreach (var solution in solver.Solutions)
            {
                numSolutions++;
            }

            Assert.AreEqual(0, numSolutions);
        }

        [TestMethod]
        public void DistanceSquared4()
        {
            ConstraintThingySolver solver = new ConstraintThingySolver();

            RealVariable x1 = solver.CreateRealVariable();
            RealVariable y1 = solver.CreateRealVariable();

            RealVariable x2 = solver.CreateRealVariable();
            RealVariable y2 = solver.CreateRealVariable();


            RealVariable distanceSquared = Constraint.DistanceSquared(new Vector2Variable(x1, y1), new Vector2Variable(x2, y2));

            Constraint.InRange(distanceSquared, -2, 2);

            foreach (var solution in solver.Solutions.FirstElements(10))
            {
                Interval x1Val = x1.UniqueValue;
                Interval x2Val = x2.UniqueValue;

                Interval y1Val = y1.UniqueValue;
                Interval y2Val = y2.UniqueValue;

                Interval x2MinusX1Val = x2Val - x1Val;

                Interval y2MinusY1Val = y2Val - y1Val;

                Interval distanceSquaredVal = x2MinusX1Val * x2MinusX1Val + y2MinusY1Val * y2MinusY1Val;

                AssertIntersect(
                    distanceSquared.UniqueValue,
                    distanceSquaredVal);
            }
        }

        [TestMethod, Timeout(5000)]
        public void SolveQuadraticEquations()
        {
            for (int i = 0; i < 100; i++) QuadraticEquation(i);
        }

        [TestMethod]
        public void SolveQuadraticEquation1()
        {
            QuadraticEquation(1);
        }

        [TestMethod]
        public void SolveQuadraticEquation2()
        {
            QuadraticEquation(2);
        }

        [TestMethod]
        public void SolveQuadraticEquation3()
        {
            QuadraticEquation(3);
        }

        [TestMethod]
        public void SolveQuadraticEquation4()
        {
            QuadraticEquation(4);
        }

        [TestMethod]
        public void SolveQuadraticEquation5()
        {
            QuadraticEquation(5);
        }

        [TestMethod]
        public void SolveQuadraticEquation6()
        {
            QuadraticEquation(6);
        }

        [TestMethod]
        public void SolveQuadraticEquation7()
        {
            QuadraticEquation(7);
        }

        [TestMethod]
        public void SolveQuadraticEquation8()
        {
            QuadraticEquation(8);
        }

        [TestMethod]
        public void SolveQuadraticEquation9()
        {
            QuadraticEquation(9);
        }

        [TestMethod]
        public void SolveQuadraticEquation10()
        {
            QuadraticEquation(10);
        }

        [TestMethod]
        public void SolveQuadraticEquation11()
        {
            QuadraticEquation(11);
        }

        [TestMethod]
        public void SolveQuadraticEquation12()
        {
            QuadraticEquation(12);
        }

        [TestMethod]
        public void SolveQuadraticEquation13()
        {
            QuadraticEquation(13);
        }

        [TestMethod]
        public void SolveQuadraticEquation16()
        {
            QuadraticEquation(16);
        }

        private static void QuadraticEquation(int random)
        {
            ConstraintThingySolver solver = new ConstraintThingySolver(random);

            // find solutions to   y = x * x + 5x - 45

            RealVariable y = solver.CreateRealVariable();

            RealVariable x = solver.CreateRealVariable(10, 200);

            var xSquared = Constraint.Square(x);

            var fiveX = solver.CreateRealVariable(5) * x;

            var fourtyFive = solver.CreateRealVariable(45);

            var rightHandSide = xSquared + fiveX - fourtyFive;

            Constraint.Equal(y, rightHandSide);

            Constraint.InRange(y, -100, 500);

            foreach (var solution in solver.Solutions.FirstElements(10))
            {
                AssertIntersect(
                    y.UniqueValue,
                    (x.UniqueValue*x.UniqueValue) + (new Interval(5, 5) * x.UniqueValue) - new Interval(45, 45));
            }
        }

        [TestMethod]
        public void RandomSumTesting()
        {
            for (int i = 0; i < 100; i++)
            {
                RandomSumTest(i);
            }
        }

        private void RandomSumTest(int seed)
        {
            ConstraintThingySolver solver = new ConstraintThingySolver(seed);

            var x = solver.CreateRealVariable();
            var y = solver.CreateRealVariable();
            var z = solver.CreateRealVariable();
            var w = solver.CreateRealVariable(0);
            
            Constraint.Equal(x, y + z + w);

            Constraint.GreaterThanOrEqual(y, 0);
            Constraint.LessThanOrEqual(z, 0);

            Constraint.Equal(x, 0);

            foreach (var solution in solver.Solutions.FirstElements(1))
            {
                AssertIntersect(x.UniqueValue, y.UniqueValue + z.UniqueValue);
            }
        }

        [TestMethod]
        public void RandomLinearEquationTests()
        {
            for (int i = 0; i < 100; i++)
            {
                LinearEquation(1);
            }
        }

        [TestMethod]
        public void RandomLinearEquationTest1()
        {
            LinearEquation(1);
        }

        [TestMethod]
        public void RandomLinearEquationTest2()
        {
            LinearEquation(1);
        }

        [TestMethod]
        public void RandomLinearEquationTest3()
        {
            LinearEquation(1);
        }

        [TestMethod]
        public void RandomLinearEquationTest4()
        {
            LinearEquation(1);
        }

        [TestMethod]
        public void RandomLinearEquationTest5()
        {
            LinearEquation(1);
        }

        [TestMethod]
        public void RandomLinearEquationTest6()
        {
            LinearEquation(1);
        }

        [TestMethod]
        public void RandomLinearEquationTest7()
        {
            LinearEquation(1);
        }

        [TestMethod]
        public void RandomLinearEquationTest8()
        {
            LinearEquation(1);
        }

        [TestMethod]
        public void RandomLinearEquationTest9()
        {
            LinearEquation(1);
        }

        [TestMethod]
        public void RandomLinearEquationTest10()
        {
            LinearEquation(1);
        }

        private void LinearEquation(int seed)
        {
            Random random = new Random();

            // y = m * x + b

            ConstraintThingySolver solver = new ConstraintThingySolver(seed);

            var x = solver.CreateRealVariable();
            var y = solver.CreateRealVariable();
            var m = solver.CreateRealVariable(random.NextDouble(-100, 100));
            var b = solver.CreateRealVariable(random.NextDouble(-1000, 1000));

            Constraint.Equal(y, m * x + b);

            foreach (var solution in solver.Solutions.FirstElements(1))
            {
                AssertIntersect(y.UniqueValue, m.UniqueValue * x.UniqueValue + b.UniqueValue);
            }
        }

        [TestMethod]
        public void MaxConstraint()
        {
            ConstraintThingySolver solver = new ConstraintThingySolver();

            var x = solver.CreateRealVariable(10);

            var y = solver.CreateRealVariable(20);

            var z = solver.CreateRealVariable();
            
            Constraint.Max(z, x, y);

            foreach (var solution in solver.Solutions.FirstElements(1))
            {
                AssertIntersect(z.UniqueValue, Interval.Max(x.UniqueValue, y.UniqueValue));
            }
        }

        [TestMethod]
        public void MaxConstraint2()
        {
            ConstraintThingySolver solver = new ConstraintThingySolver();

            var x = solver.CreateRealVariable(10, 30);

            var y = solver.CreateRealVariable(10, 30);

            var z = solver.CreateRealVariable();

            Constraint.Max(z, x, y);

            foreach (var solution in solver.Solutions.FirstElements(1))
            {
                AssertIntersect(z.UniqueValue, Interval.Max(x.UniqueValue, y.UniqueValue));
            }
        }

        [TestMethod]
        public void MaxConstraint3()
        {
            ConstraintThingySolver solver = new ConstraintThingySolver();

            var x = solver.CreateRealVariable(10, 5000);

            var y = solver.CreateRealVariable(10, 30);

            var z = solver.CreateRealVariable();

            Constraint.Max(z, x, y);

            foreach (var solution in solver.Solutions.FirstElements(10))
            {
                AssertIntersect(z.UniqueValue, Interval.Max(x.UniqueValue, y.UniqueValue));
            }
        }

        [TestMethod]
        public void MaxConstraint4()
        {
            ConstraintThingySolver solver = new ConstraintThingySolver();

            var x = solver.CreateRealVariable(-500, 0);

            var y = solver.CreateRealVariable(10, 30);

            var z = solver.CreateRealVariable();

            Constraint.Max(z, x, y);

            foreach (var solution in solver.Solutions.FirstElements(10))
            {
                AssertIntersect(z.UniqueValue, Interval.Max(x.UniqueValue, y.UniqueValue));
                AssertIntersect(z.UniqueValue, y.UniqueValue);
            }
        }

        [TestMethod]
        public void MinConstraint()
        {
            ConstraintThingySolver solver = new ConstraintThingySolver();

            var x = solver.CreateRealVariable(10);

            var y = solver.CreateRealVariable(20);

            var z = solver.CreateRealVariable();

            Constraint.Min(z, x, y);

            foreach (var solution in solver.Solutions.FirstElements(1))
            {
                AssertIntersect(z.UniqueValue, Interval.Min(x.UniqueValue, y.UniqueValue));
            }
        }

        [TestMethod]
        public void MinConstraint2()
        {
            ConstraintThingySolver solver = new ConstraintThingySolver();

            var x = solver.CreateRealVariable(10, 30);

            var y = solver.CreateRealVariable(10, 30);

            var z = solver.CreateRealVariable();

            Constraint.Min(z, x, y);

            foreach (var solution in solver.Solutions.FirstElements(1))
            {
                AssertIntersect(z.UniqueValue, Interval.Min(x.UniqueValue, y.UniqueValue));
            }
        }

        [TestMethod]
        public void MinConstraint3()
        {
            ConstraintThingySolver solver = new ConstraintThingySolver();

            var x = solver.CreateRealVariable(10, 5000);

            var y = solver.CreateRealVariable(10, 30);

            var z = solver.CreateRealVariable();

            Constraint.Min(z, x, y);

            foreach (var solution in solver.Solutions.FirstElements(10))
            {
                AssertIntersect(z.UniqueValue, Interval.Min(x.UniqueValue, y.UniqueValue));
            }
        }

        [TestMethod]
        public void MinConstraint4()
        {
            ConstraintThingySolver solver = new ConstraintThingySolver();

            var x = solver.CreateRealVariable(-500, 0);

            var y = solver.CreateRealVariable(10, 30);

            var z = solver.CreateRealVariable();

            Constraint.Min(z, x, y);

            foreach (var solution in solver.Solutions.FirstElements(10))
            {
                AssertIntersect(z.UniqueValue, Interval.Min(x.UniqueValue, y.UniqueValue));
                AssertIntersect(z.UniqueValue, x.UniqueValue);
            }
        }
    }
}
