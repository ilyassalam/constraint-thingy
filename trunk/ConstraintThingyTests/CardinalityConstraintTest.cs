using ConstraintThingy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Tests
{
    
    
    /// <summary>
    ///This is a test class for CardinalityConstraintTest and is intended
    ///to contain all CardinalityConstraintTest Unit Tests
    ///</summary>
    [TestClass()]
    public class CardinalityConstraintTest
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
        ///A test for Narrowed
        ///</summary>
        [TestMethod()]
        public void NarrowedTest()
        {
            FiniteDomain d = new FiniteDomain("hub", "forest", "swamp", "cave", "other");
            FiniteDomainVariable[] vars = new FiniteDomainVariable[]
                                              {
                                                  new FiniteDomainVariable("l1", d),
                                                  new FiniteDomainVariable("l2", d),
                                                  new FiniteDomainVariable("l3", d),
                                                  new FiniteDomainVariable("l4", d),
                                                  new FiniteDomainVariable("l5", d),
                                                  new FiniteDomainVariable("l6", d)
                                              };

            new CardinalityConstraint("hub", 1, 1, vars);
            new CardinalityConstraint("forest", 1, 1, vars);
            new CardinalityConstraint("swamp", 1, 1, vars);
            new CardinalityConstraint("cave", 1, 1, vars);
            vars[0].UniqueValue = "hub";
            int solutions = 0;
            foreach (var ignore1 in vars[0].UniqueValues())
                foreach (var ignore2 in vars[1].UniqueValues())
                    foreach (var ignore3 in vars[2].UniqueValues())
                        foreach (var ignore4 in vars[3].UniqueValues())
                            foreach (var ignore5 in vars[4].UniqueValues())
                                foreach (var ignore6 in vars[5].UniqueValues())
                                {
                                    solutions++;
                                }
            Assert.AreEqual(5*4*3, solutions);
        }
    }
}
