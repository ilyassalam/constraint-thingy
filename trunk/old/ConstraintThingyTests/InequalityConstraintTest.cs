using ConstraintThingy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Tests
{
    
    
    /// <summary>
    ///This is a test class for InequalityConstraintTest and is intended
    ///to contain all InequalityConstraintTest Unit Tests
    ///</summary>
    [TestClass()]
    public class InequalityConstraintTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for UpdateVariable
        ///</summary>
        [TestMethod()]
        public void UpdateVariableTest()
        {
            var d = new FiniteDomain("red", "green", "blue");
            var x = new FiniteDomainVariable("x", d);
            var y = new FiniteDomainVariable("y", d);
            new InequalityConstraint(x, y);
            int solutions = 0;
            foreach (var ignore1 in x.UniqueValues())
                foreach (var ignore2 in y.UniqueValues())
                {
                    Assert.AreNotEqual(x.UniqueValue, y.UniqueValue);
                    solutions++;
                }
            Assert.AreEqual(6, solutions);
        }
    }
}
