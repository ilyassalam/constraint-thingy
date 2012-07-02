using ConstraintThingy;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    
    
    /// <summary>
    ///This is a test class for FiniteDomainVariableTest and is intended
    ///to contain all FiniteDomainVariableTest Unit Tests
    ///</summary>
    [TestClass]
    public class FiniteDomainVariableTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
// ReSharper disable ConvertToAutoProperty
        public TestContext TestContext
// ReSharper restore ConvertToAutoProperty
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
        ///A test for UniqueValues
        ///</summary>
        [TestMethod]
        public void UniqueValuesTest()
        {
            var d = new FiniteDomain("red", "green", "blue");
            var x = new FiniteDomainVariable("x", d);
            string[] answers = new[] { "red", "blue"};

            x.Value = 5;  // should be red + blue;
            int answerCounter = 0;
#pragma warning disable 168
            foreach (var ignore in x.UniqueValues())
#pragma warning restore 168
            {
                Assert.IsTrue(answerCounter<2, "Too many unique values enumerated");
                Assert.AreSame(answers[answerCounter++], x.UniqueValue);
            }
            Assert.AreEqual(2, answerCounter, "Insufficient unique values enumerated.");
        }

        /// <summary>
        ///A test for Candidates
        ///</summary>
        [TestMethod]
        public void CandidatesTest()
        {
            var d = new FiniteDomain("red", "green", "blue");
            var x = new FiniteDomainVariable("x", d);
            Assert.AreEqual(3, x.Candidates);
        }

        /// <summary>
        ///A test for DebugString
        ///</summary>
        [TestMethod]
        public void ToStringTest()
        {
            var d = new FiniteDomain("red", "green", "blue");
            var x = new FiniteDomainVariable("x", d);
            Assert.AreEqual("x = { red, green, blue }", x.ToString());
        }

        /// <summary>
        ///A test for IsEmpty
        ///</summary>
        [TestMethod]
        public void IsEmptyTest()
        {
            var d = new FiniteDomain("red", "green", "blue");
            var x = new FiniteDomainVariable("x", d);
            Assert.IsFalse(x.IsEmpty);
            x.Value = 0;
            Assert.IsTrue(x.IsEmpty);
        }

        /// <summary>
        ///A test for IsUnique
        ///</summary>
        [TestMethod]
        public void IsUniqueTest()
        {
            var d = new FiniteDomain("red", "green", "blue");
            var x = new FiniteDomainVariable("x", d);
            Assert.IsFalse(x.IsUnique);
            x.UniqueValue = "red";
            Assert.IsTrue(x.IsUnique);
            x.Value = 0;
            Assert.IsFalse(x.IsUnique);
        }

        /// <summary>
        ///A test for UniqueValue
        ///</summary>
        [TestMethod]
        public void UniqueValueTest()
        {
            var d = new FiniteDomain("red", "green", "blue");
            var x = new FiniteDomainVariable("x", d) {UniqueValue = "red"};
            Assert.AreEqual("red", x.UniqueValue);
        }
    }
}
