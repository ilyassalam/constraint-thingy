using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using ConstraintThingy;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ConstraintThingyTest
{
    [TestClass]
    public class FiniteDomainVariableTests
    {
        [TestMethod]
        public void CorrectlyEnumeratesPossibleValues()
        {
            ConstraintThingySolver constraintThingySolver = new ConstraintThingySolver();

            FiniteDomain<String> finiteDomain = new FiniteDomain<string>("red", "green", "blue")
                                                    {
                                                        
                                                    }; 

            new FiniteDomainVariable<string>(constraintThingySolver, "x", finiteDomain, "red", "blue");

            Assert.AreEqual(2, constraintThingySolver.Solutions.Count());
        }
        
        [TestMethod]
        public void CardinalityConstraint()
        {
            ConstraintThingySolver constraintThingySolver = new ConstraintThingySolver();

            FiniteDomain<String> finiteDomain = new FiniteDomain<String>("hub", "forest", "swamp", "cave", "other")
                                                    {
                                                        
                                                    };

            FiniteDomainVariable<String> a = new FiniteDomainVariable<String>(constraintThingySolver, "a", finiteDomain, "hub");

            FiniteDomainVariable<String> b = new FiniteDomainVariable<String>(constraintThingySolver, "b", finiteDomain, "hub", "forest", "swamp", "cave", "other");

            FiniteDomainVariable<String> c = new FiniteDomainVariable<String>(constraintThingySolver, "c", finiteDomain, "hub", "forest", "swamp", "cave", "other");

            FiniteDomainVariable<String> d = new FiniteDomainVariable<String>(constraintThingySolver, "d", finiteDomain, "hub", "forest", "swamp", "cave", "other");

            FiniteDomainVariable<String> e = new FiniteDomainVariable<String>(constraintThingySolver, "e", finiteDomain, "hub", "forest", "swamp", "cave", "other");

            FiniteDomainVariable<String> f = new FiniteDomainVariable<String>(constraintThingySolver, "f", finiteDomain, "hub", "forest", "swamp", "cave", "other");


            FiniteDomainVariable<String>[] vars = new[] { a, b, c, d, e, f };

            Constraint.RequireOccurences("hub", 1, vars);
            Constraint.RequireOccurences("forest", 1, vars);
            Constraint.RequireOccurences("swamp", 1, vars);
            Constraint.RequireOccurences("cave", 1, vars);

            Assert.AreEqual(60, constraintThingySolver.Solutions.Count());
        }

        [TestMethod]
        public void InequalityConstraint()
        {
            var constraintSolver = new ConstraintThingySolver();

            var finiteDomain = new FiniteDomain<String>("red", "green", "blue")
                                   {
                                       
                                   };

            var x = new FiniteDomainVariable<String>(constraintSolver, "x", finiteDomain, "red", "green", "blue");

            var y = new FiniteDomainVariable<String>(constraintSolver, "y", finiteDomain, "red", "green", "blue");

            Constraint.NotEqual(x, y);

            int solutions = 0;

            foreach (Solution solution in constraintSolver.Solutions)
            {
                solutions++;
                Assert.AreNotEqual(x.UniqueValue, y.UniqueValue);
            }

            Assert.AreEqual(6, solutions);
        }

        [TestMethod]
        public void EqualityConstraint()
        {
            var constraintSolver = new ConstraintThingySolver();

            var finiteDomain = new FiniteDomain<String>("red", "green", "blue")
                                   {
                                      
                                   };

            var x = new FiniteDomainVariable<String>(constraintSolver, "x", finiteDomain, "red", "green", "blue");

            var y = new FiniteDomainVariable<String>(constraintSolver, "y", finiteDomain, "red", "green", "blue");

            Constraint.Equal(x, y);

            int solutions = 0;

            foreach (Solution solution in constraintSolver.Solutions)
            {
                solutions++;
                Assert.AreEqual(x.UniqueValue, y.UniqueValue);
            }

            Assert.AreEqual(3, solutions);
        }

        [TestMethod]
        public void EqualityAndInequalityInteraction()
        {
            var constraintSolver = new ConstraintThingySolver();

            var finiteDomain = new FiniteDomain<String>("red", "green", "blue")
                                   {
                                      
                                   };

            var x = new FiniteDomainVariable<String>(constraintSolver, "x", finiteDomain, "red", "green", "blue");

            var y = new FiniteDomainVariable<String>(constraintSolver, "y", finiteDomain, "red", "green", "blue");

            var z = new FiniteDomainVariable<String>(constraintSolver, "z", finiteDomain, "red", "green", "blue");

            Constraint.Equal(x, y);

            Constraint.NotEqual(y, z);

            int solutions = 0;

            foreach (Solution solution in constraintSolver.Solutions)
            {
                solutions++;
                Assert.AreEqual(x.UniqueValue, y.UniqueValue);
                Assert.AreNotEqual(y.UniqueValue, z.UniqueValue);
                Assert.AreNotEqual(x.UniqueValue, z.UniqueValue);
            }

            Assert.AreEqual(6, solutions);
        }

        [TestMethod]
        public void CorrectlyEnumeratesLotsOfPossibleValues()
        {
            ConstraintThingySolver constraintThingySolver = new ConstraintThingySolver();

            FiniteDomain<int> finiteDomain = new FiniteDomain<int>(Enumerable.Range(0, 64))
                                                    {
                                                        
                                                    };

            FiniteDomainVariable<int> x = new FiniteDomainVariable<int>(constraintThingySolver, "x", finiteDomain, Enumerable.Range(0, 64));

            FiniteDomainVariable<int> y = new FiniteDomainVariable<int>(constraintThingySolver, "y", finiteDomain, Enumerable.Range(0, 64));

            FiniteDomainVariable<int> z = new FiniteDomainVariable<int>(constraintThingySolver, "z", finiteDomain, Enumerable.Range(0, 64));

            Assert.AreEqual(64 * 64 * 64, constraintThingySolver.Solutions.Count());
        }
    }
}
