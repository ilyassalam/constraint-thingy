using System;
using System.Diagnostics;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using ConstraintThingy;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ConstraintThingyTest
{
    [TestClass]
    public class MultiIntervalTests
    {
        private T AssertCommutative<T>(Func<MultiInterval, MultiInterval, T> func, MultiInterval a, MultiInterval b)
        {
            Assert.AreEqual(func(a, b), func(b, a));
            return func(a, b);
        }

        [TestMethod]
        public void Creation()
        {
            var interval = new MultiInterval(new Interval(0, 5), new Interval(6, 10));

            Assert.IsTrue(interval.Contains(0));
            Assert.IsTrue(interval.Contains(1));
            Assert.IsTrue(interval.Contains(2));
            Assert.IsTrue(interval.Contains(2.22));
            Assert.IsTrue(interval.Contains(5));
            Assert.IsTrue(interval.Contains(6));
            Assert.IsTrue(interval.Contains(8));
            Assert.IsTrue(interval.Contains(9));
            Assert.IsTrue(interval.Contains(10));

            Assert.IsFalse(interval.Contains(-1));
            Assert.IsFalse(interval.Contains(5.5));
            Assert.IsFalse(interval.Contains(11));

            Assert.IsFalse(interval.ContainsWithin(0));
            Assert.IsFalse(interval.ContainsWithin(5));
            Assert.IsFalse(interval.ContainsWithin(6));
            Assert.IsFalse(interval.ContainsWithin(10));
        }

        [TestMethod]
        public void Creation2()
        {
            var interval = new MultiInterval(new Interval(0, 5), new Interval(5, 10));

            Assert.IsTrue(interval.Contains(0));
            Assert.IsTrue(interval.Contains(1));
            Assert.IsTrue(interval.Contains(2));
            Assert.IsTrue(interval.Contains(2.22));
            Assert.IsTrue(interval.Contains(5));
            Assert.IsTrue(interval.Contains(5.5));
            Assert.IsTrue(interval.Contains(6));
            Assert.IsTrue(interval.Contains(8));
            Assert.IsTrue(interval.Contains(9));
            Assert.IsTrue(interval.Contains(10));

            Assert.IsFalse(interval.Contains(-1));
            Assert.IsFalse(interval.Contains(11));

            Assert.IsFalse(interval.ContainsWithin(0));
            Assert.IsFalse(interval.ContainsWithin(10));
        }

        [TestMethod]
        public void Equality()
        {
            Assert.AreEqual(new MultiInterval(new Interval(1, 15)), new MultiInterval(new Interval(1, 15)));
            Assert.AreNotEqual(new MultiInterval(new Interval(2, 15)), new MultiInterval(new Interval(1, 15)));
            Assert.AreEqual(new MultiInterval(new Interval(1, 15), new Interval(20, 25)), new MultiInterval(new Interval(1, 15), new Interval(20, 25)));
            Assert.AreNotEqual(new MultiInterval(new Interval(14, 15), new Interval(20, 25)), new MultiInterval(new Interval(1, 15), new Interval(20, 25)));
        }

        [TestMethod]
        public void Addition()
        {
            var a = new MultiInterval(new Interval(0, 5), new Interval(6, 10));
            var b = new MultiInterval(new Interval(1, 1), new Interval(3, 5));

            var c = MultiInterval.Add(a, b);

            Assert.AreEqual(new MultiInterval(new Interval(1, 15)), c);
        }

        [TestMethod]
        public void Subtraction()
        {
            var a = new MultiInterval(new Interval(0, 5), new Interval(6, 10));
            var b = new MultiInterval(new Interval(1, 1), new Interval(3, 5));

            var c = MultiInterval.Subtract(a, b);

            Assert.AreEqual(new MultiInterval(new Interval(-5, 9)), c);
        }

        [TestMethod]
        public void Multiplication()
        {
            var a = new MultiInterval(new Interval(0, 5), new Interval(6, 10));
            var b = new MultiInterval(new Interval(1, 1), new Interval(3, 5));

            var c = MultiInterval.Multiply(a, b);

            Assert.AreEqual(new MultiInterval(new Interval(0, 50)), c);
        }

        [TestMethod]
        public void Division()
        {
            var a = new MultiInterval(new Interval(10, 20));
            var b = new MultiInterval(new Interval(-10, 10));

            var c = MultiInterval.Divide(a, b);

            Assert.AreEqual(new MultiInterval(new Interval(1, double.PositiveInfinity), new Interval(double.NegativeInfinity, -1)), c);
        }
    }

    [TestClass]
    public class IntervalTests
    {
        private T AssertCommutative<T>(Func<Interval, Interval, T> func, Interval a, Interval b)
        {
            Assert.AreEqual(func(a, b), func(b, a));
            return func(a, b);
        }

        [TestMethod]
        public void Creation()
        {
            Interval interval = new Interval();

            Assert.AreEqual(0f, interval.LowerBound);
            Assert.AreEqual(0f, interval.UpperBound);
            Assert.AreEqual(0f, interval.Range);
            Assert.AreEqual(0f, interval.Center);
        }

        [TestMethod]
        public void Creation2()
        {
            Interval interval = new Interval(5f, 11f);

            Assert.AreEqual(5f, interval.LowerBound);
            Assert.AreEqual(11f, interval.UpperBound);
            Assert.AreEqual(6f, interval.Range);
            Assert.AreEqual(8f, interval.Center);
        }

        [TestMethod]
        public void Creation3()
        {
            Interval interval = new Interval(-5f, 11f);

            Assert.AreEqual(-5f, interval.LowerBound);
            Assert.AreEqual(11f, interval.UpperBound);
            Assert.AreEqual(16f, interval.Range);
            Assert.AreEqual(3f, interval.Center);
        }

        [TestMethod]
        [ExpectedException(typeof (ArgumentOutOfRangeException))]
        public void Creation4()
        {
            new Interval(-5f, -10f);
        }


        [TestMethod]
        public void Equality()
        {
            Assert.AreEqual(new Interval(), new Interval());
            Assert.AreEqual(new Interval(0, 0), new Interval());
            Assert.AreEqual(new Interval(1, 2), new Interval(1, 2));
            Assert.AreEqual(new Interval(-5, 5), new Interval(-5, 5));
            Assert.AreEqual(Interval.Empty, Interval.Empty);

            Assert.AreNotEqual(Interval.Empty, new Interval(0, 1));
            Assert.AreNotEqual(new Interval(), new Interval(0, 1));
            Assert.AreNotEqual(new Interval(0, 1), new Interval(0, 2));
            Assert.AreNotEqual(new Interval(-1, -1), new Interval(1, 1));
        }

        [TestMethod]
        public void Add()
        {
            Assert.AreEqual(new Interval(3, 5), AssertCommutative(Interval.Add, new Interval(1, 2), new Interval(2, 3)));
            Assert.AreEqual(new Interval(2, 3), AssertCommutative(Interval.Add, new Interval(0, 0), new Interval(2, 3)));
            Assert.AreEqual(new Interval(0, 0), AssertCommutative(Interval.Add, new Interval(0, 0), new Interval(0, 0)));
            Assert.AreEqual(new Interval(-2, 0),
                            AssertCommutative(Interval.Add, new Interval(1, 2), new Interval(-3, -2)));
            Assert.AreEqual(Interval.Empty, AssertCommutative(Interval.Add, Interval.Empty, new Interval(-3, -2)));
        }

        [TestMethod]
        public void Subtract()
        {
            Assert.AreEqual(new Interval(-2, 0), Interval.Subtract(new Interval(1, 2), new Interval(2, 3)));
            Assert.AreEqual(new Interval(-17, -8), Interval.Subtract(new Interval(-5, 2), new Interval(10, 12)));
            Assert.AreEqual(new Interval(5, 5), Interval.Subtract(new Interval(0, 0), new Interval(-5, -5)));
            Assert.AreEqual(new Interval(-5, -5), Interval.Subtract(new Interval(0, 0), new Interval(5, 5)));
            Assert.AreEqual(new Interval(5, 5), Interval.Subtract(new Interval(5, 5), new Interval(0, 0)));
            Assert.AreEqual(new Interval(-5, 5), Interval.Subtract(new Interval(-5, 5), new Interval(0, 0)));
            Assert.AreEqual(new Interval(0, 0), Interval.Subtract(new Interval(0, 0), new Interval(0, 0)));
            Assert.AreEqual(Interval.Empty, Interval.Subtract(Interval.Empty, new Interval(-3, -2)));
        }

        [TestMethod]
        public void Multiply()
        {
            Assert.AreEqual(new Interval(0, 0),
                            AssertCommutative(Interval.Multiply, new Interval(2, 3), new Interval(0, 0)));
            Assert.AreEqual(new Interval(2, 3),
                            AssertCommutative(Interval.Multiply, new Interval(1, 1), new Interval(2, 3)));
            Assert.AreEqual(new Interval(0, 0),
                            AssertCommutative(Interval.Multiply, new Interval(-2, 4), new Interval(0, 0)));
            Assert.AreEqual(new Interval(-2, 4),
                            AssertCommutative(Interval.Multiply, new Interval(-2, 4), new Interval(1, 1)));


            Assert.AreEqual(new Interval(1, 24),
                            AssertCommutative(Interval.Multiply, new Interval(1, 4), new Interval(1, 6)));
            Assert.AreEqual(new Interval(-4, 24),
                            AssertCommutative(Interval.Multiply, new Interval(1, 4), new Interval(-1, 6)));
            Assert.AreEqual(new Interval(-6, 24),
                            AssertCommutative(Interval.Multiply, new Interval(-1, 4), new Interval(-1, 6)));
            Assert.AreEqual(new Interval(-24, 6),
                            AssertCommutative(Interval.Multiply, new Interval(-1, 4), new Interval(-6, -1)));
            Assert.AreEqual(new Interval(-25, 25),
                            AssertCommutative(Interval.Multiply, new Interval(-5, 5), new Interval(-5, 5)));

            Assert.AreEqual(new Interval(-25, 0),
                            AssertCommutative(Interval.Multiply, new Interval(0, 5), new Interval(-5, 0)));
            Assert.AreEqual(new Interval(-25, 5),
                            AssertCommutative(Interval.Multiply, new Interval(0.5f, 5), new Interval(-5, 1)));
        }

        [TestMethod]
        public void Divide()
        {
            Assert.AreEqual(new Interval(1/3f, 1/2f), Interval.Divide(new Interval(1, 1), new Interval(2, 3)));
            Assert.AreEqual(new Interval(1/0.99f, 1/0.01f),
                            Interval.Divide(new Interval(1, 1), new Interval(0.01f, 0.99f)));
            Assert.AreEqual(new Interval(-1/2f, -1/3f), Interval.Divide(new Interval(-1, -1), new Interval(2, 3)));
            Assert.AreEqual(new Interval(-5f, -4/3f), Interval.Divide(new Interval(-10, -4), new Interval(2, 3)));
        }

        [TestMethod]
        [ExpectedException(typeof (ArgumentOutOfRangeException))]
        public void DivideFailureCase1()
        {
            Interval.Divide(new Interval(1, 1), new Interval(-2, 3));
        }

        [TestMethod]
        [ExpectedException(typeof (ArgumentOutOfRangeException))]
        public void DivideFailureCase2()
        {
            Interval.Divide(new Interval(1, 1), new Interval(-0.01f, 0.99f));
        }

        [TestMethod]
        [ExpectedException(typeof (ArgumentOutOfRangeException))]
        public void DivideFailureCase3()
        {
            Interval.Divide(new Interval(-1, -1), new Interval(-2, 3));
        }

        [TestMethod]
        public void DivideFailureCase4()
        {
            Assert.AreEqual(new Interval(double.NegativeInfinity, double.PositiveInfinity), Interval.Divide(new Interval(-10, -4), new Interval(0, 0)));
        }

        [TestMethod]
        public void DivideFailureCase5()
        {
            Assert.AreEqual(new Interval(0.8, double.PositiveInfinity), Interval.Divide(new Interval(-10, -4), new Interval(-5, 0)));
        }

        [TestMethod]
        public void DivideFailureCase6()
        {
            Assert.AreEqual(new Interval(double.NegativeInfinity, -0.8), Interval.Divide(new Interval(-10, -4), new Interval(0, 5)));
        }

        [TestMethod]
        public void Intersects()
        {
            Assert.IsTrue(AssertCommutative(Interval.Intersects, new Interval(0, 1), new Interval(0, 0.5f)));
            Assert.IsTrue(AssertCommutative(Interval.Intersects, new Interval(0, 0), new Interval(0, 0)));
            Assert.IsTrue(AssertCommutative(Interval.Intersects, new Interval(1, 1), new Interval(1, 1)));
            Assert.IsTrue(AssertCommutative(Interval.Intersects, new Interval(0, 1), new Interval(-2, 3)));
            Assert.IsTrue(AssertCommutative(Interval.Intersects, new Interval(-3, 1), new Interval(0, 0.01f)));
            Assert.IsTrue(AssertCommutative(Interval.Intersects, new Interval(0, 1), new Interval(0, 1.5f)));
            Assert.IsTrue(AssertCommutative(Interval.Intersects, new Interval(0, 1), new Interval(0, 2.5f)));

            Assert.IsFalse(AssertCommutative(Interval.Intersects, new Interval(0, 1), new Interval(2, 3)));
            Assert.IsFalse(AssertCommutative(Interval.Intersects, new Interval(-3, 1.99f), new Interval(2, 3)));
            Assert.IsFalse(AssertCommutative(Interval.Intersects, new Interval(-3, 2.01f), new Interval(2.02f, 3)));

            Assert.IsFalse(AssertCommutative(Interval.Intersects, new Interval(-3, 2.01f), Interval.Empty));
        }

        [TestMethod]
        public void Intersection()
        {
            Assert.AreEqual(new Interval(0, 0),
                            AssertCommutative(Interval.Intersection, new Interval(0, 0), new Interval(0, 0)));
            Assert.AreEqual(new Interval(1, 1),
                            AssertCommutative(Interval.Intersection, new Interval(1, 1), new Interval(1, 1)));
            Assert.AreEqual(new Interval(1, 2),
                            AssertCommutative(Interval.Intersection, new Interval(1, 2), new Interval(1, 2)));
            Assert.AreEqual(new Interval(-1, 1),
                            AssertCommutative(Interval.Intersection, new Interval(-1, 1), new Interval(-2, 2)));
            Assert.AreEqual(new Interval(-5, 5),
                            AssertCommutative(Interval.Intersection, new Interval(-10, 5), new Interval(-5, 10)));
            Assert.AreEqual(Interval.Empty,
                            AssertCommutative(Interval.Intersection, new Interval(-1, 0), new Interval(1, 2)));
            Assert.AreEqual(new Interval(1, 1),
                            AssertCommutative(Interval.Intersection, new Interval(0, 1), new Interval(1, 2)));
            Assert.AreEqual(Interval.Empty,
                            AssertCommutative(Interval.Intersection, Interval.Empty, new Interval(-50, 50)));
        }

        [TestMethod]
        public void Union()
        {
            Assert.AreEqual(new Interval(0, 0),
                            AssertCommutative(Interval.Union, new Interval(0, 0), new Interval(0, 0)));
            Assert.AreEqual(new Interval(0, 2),
                            AssertCommutative(Interval.Union, new Interval(0, 1), new Interval(1, 2)));
            Assert.AreEqual(new Interval(-1, 1),
                            AssertCommutative(Interval.Union, new Interval(-1, 0), new Interval(0, 1)));
            Assert.AreEqual(new Interval(0, 5),
                            AssertCommutative(Interval.Union, new Interval(0, 2.5f), new Interval(2, 5)));
            Assert.AreEqual(new Interval(0, 5),
                            AssertCommutative(Interval.Union, new Interval(0, 3), new Interval(3, 5)));
            Assert.AreEqual(new Interval(-10, 10),
                            AssertCommutative(Interval.Union, new Interval(-10, 10), new Interval(10, 10)));
            Assert.AreEqual(new Interval(0, 5), AssertCommutative(Interval.Union, new Interval(0, 5), Interval.Empty));
            Assert.AreEqual(new Interval(-10, 10),
                            AssertCommutative(Interval.Union, new Interval(-10, 2), new Interval(9, 10)));
        }

        [TestMethod]
        public void Max()
        {
            Assert.AreEqual(new Interval(0, 0), AssertCommutative(Interval.Max, new Interval(0, 0), new Interval(0, 0)));
            Assert.AreEqual(new Interval(2, 3), AssertCommutative(Interval.Max, new Interval(0, 0), new Interval(2, 3)));
            Assert.AreEqual(new Interval(5, 10),
                            AssertCommutative(Interval.Max, new Interval(0, 0), new Interval(5, 10)));
            Assert.AreEqual(new Interval(0, 0), AssertCommutative(Interval.Max, new Interval(0, 0), new Interval(-5, 0)));
            Assert.AreEqual(new Interval(5, 10),
                            AssertCommutative(Interval.Max, new Interval(0, 0), new Interval(5, 10)));
            Assert.AreEqual(new Interval(9, 20),
                            AssertCommutative(Interval.Max, new Interval(2, 10), new Interval(9, 20)));
        }

        [TestMethod]
        public void Min()
        {
            Assert.AreEqual(new Interval(0, 0), AssertCommutative(Interval.Min, new Interval(0, 0), new Interval(0, 0)));
            Assert.AreEqual(new Interval(0, 0), AssertCommutative(Interval.Min, new Interval(0, 0), new Interval(2, 3)));
            Assert.AreEqual(new Interval(-5, 0),
                            AssertCommutative(Interval.Min, new Interval(-5, 0), new Interval(5, 10)));
            Assert.AreEqual(new Interval(-5, 0),
                            AssertCommutative(Interval.Min, new Interval(0, 0), new Interval(-5, 0)));
            Assert.AreEqual(new Interval(0, 0), AssertCommutative(Interval.Min, new Interval(0, 0), new Interval(5, 10)));
            Assert.AreEqual(new Interval(2, 10),
                            AssertCommutative(Interval.Min, new Interval(2, 10), new Interval(9, 20)));
        }


        [TestMethod]
        public void Range()
        {
            Assert.AreEqual(0, new Interval(0, 0).Range);
            Assert.AreEqual(1, new Interval(0, 1).Range);
            Assert.AreEqual(2, new Interval(0, 2).Range);
            Assert.AreEqual(3, new Interval(0, 3).Range);
            Assert.AreEqual(3, new Interval(-3, 0).Range);
            Assert.AreEqual(5, new Interval(-5, 0).Range);
            Assert.AreEqual(5, new Interval(-2, 3).Range);
        }

        [TestMethod]
        public void Center()
        {
            Assert.AreEqual(0, new Interval(0, 0).Center);
            Assert.AreEqual(0.5f, new Interval(0, 1).Center);
            Assert.AreEqual(1, new Interval(0, 2).Center);
            Assert.AreEqual(1.5f, new Interval(0, 3).Center);
            Assert.AreEqual(-1.5f, new Interval(-3, 0).Center);
            Assert.AreEqual(-2.5f, new Interval(-5, 0).Center);
            Assert.AreEqual(0.5f, new Interval(-2, 3).Center);
        }

        [TestMethod]
        public void Empty()
        {
            Assert.IsTrue(Interval.Empty.IsEmpty);

            Assert.IsFalse(new Interval(0, 0).IsEmpty);
            Assert.IsFalse(new Interval(0, 1).IsEmpty);
            Assert.IsFalse(new Interval(0, 2).IsEmpty);
            Assert.IsFalse(new Interval(0, 3).IsEmpty);
            Assert.IsFalse(new Interval(-1, 1).IsEmpty);
            Assert.IsFalse(new Interval(-2, 2).IsEmpty);
            Assert.IsFalse(new Interval(-3, 0).IsEmpty);
        }

        [TestMethod]
        public void Contains()
        {
            var interval = new Interval(0, 10);

            Assert.IsTrue(interval.Contains(new Interval(1, 5)));

            Assert.IsTrue(interval.Contains(new Interval(1, 1)));

            Assert.IsTrue(interval.Contains(new Interval(0, 10)));

            Assert.IsFalse(interval.Contains(new Interval(-1, 10)));

            Assert.IsFalse(interval.Contains(new Interval(9, 12)));

            Assert.IsFalse(interval.Contains(new Interval(-5, -2)));
        }
    }
}

