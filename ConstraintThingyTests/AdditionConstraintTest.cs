using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using ConstraintThingy;
using Intervals;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    [TestClass]
    public class AdditionConstraintTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            var a = new IntervalVariable("a", new Interval(-10f, 10f));
            var b = new IntervalVariable("b", new Interval(-10f, 100f));
            var c = new IntervalVariable("c", new Interval(-20f, -20f));

            new IntervalSumConstraint(c, a, b);

            new IntervalEqualityConstraint(a, b);

            int numSolutions = 0;

            // the only solution is sum = (-20,-20) and a = b = (-10,-10)

            foreach (var solutionsAllVariable in Variable.Solutions(new [] {a,b,c}))
            {
                Assert.AreEqual(c.Value, a.Value + b.Value);
                numSolutions++;
            }

            Assert.AreEqual(1, numSolutions);
        }

        [TestMethod]
        public void TestMethod2()
        {
            var a = new IntervalVariable("a", new Interval(-30f, -10f));
            var b = new IntervalVariable("b", new Interval(-10f, 100f));
            var c = new IntervalVariable("c", new Interval(-20f, -20f));

            new IntervalSumConstraint(c, a, b);

            new IntervalEqualityConstraint(a, b);

            int numSolutions = 0;

            // the only solution is sum = (-20,20) and a = b = (-10,-10)

            foreach (var solutionsAllVariable in Variable.Solutions(new[] { a, b, c }))
            {
                Assert.AreEqual(c.Value, a.Value + b.Value);
                numSolutions++;
            }

            Assert.AreEqual(1, numSolutions);
        }

        [TestMethod]
        public void TestMethod3()
        {
            var variables = new IntervalVariable[]
                              {
                                  new IntervalVariable("1", new Interval(0, 10)), 
                                  new IntervalVariable("2", new Interval(0, 10)), 
                                  new IntervalVariable("3", new Interval(0, 10)), 
                                  new IntervalVariable("4", new Interval(0, 10)), 
                                  new IntervalVariable("5", new Interval(0, 10)), 
                                  new IntervalVariable("6", new Interval(0, 10)), 
                                  new IntervalVariable("7", new Interval(0, 10)), 
                                  new IntervalVariable("8", new Interval(0, 10)), 
                                  new IntervalVariable("9", new Interval(0, 10)), 
                                  new IntervalVariable("10", new Interval(0, 10)), 
                              };

            var sum = new IntervalVariable("sum", new Interval(100, 200));

            var constraint = new IntervalSumConstraint(sum, variables);

            int numSolutions = 0;

            // the only solution is sum = (100,100), and all variables = (10,10)

            foreach (var solutionsAllVariable in Variable.Solutions(constraint.Variables))
            {
                Assert.AreEqual(new Interval(100,100), sum.Value);

                foreach (var variable in variables)
                {
                    Assert.AreEqual(variable.Value, new Interval(10,10));
                }

                numSolutions++;
            }

            Assert.AreEqual(1, numSolutions);
        }
    }
}
