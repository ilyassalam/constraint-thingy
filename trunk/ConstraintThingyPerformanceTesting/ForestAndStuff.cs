using System;
using System.Linq;
using ConstraintThingy;

namespace ConstraintThingyPerformanceTesting
{
    class ForestAndStuff : PerformanceTest
    {
        protected override void InitializeConstraintSystem(ConstraintThingySolver solver)
        {
            FiniteDomain<String> finiteDomain = new FiniteDomain<String>("hub", "forest", "swamp", "cave", "other");

            FiniteDomainVariable<String> a = new FiniteDomainVariable<String>(solver, "a", finiteDomain, "hub");

            FiniteDomainVariable<String> b = new FiniteDomainVariable<String>(solver, "b", finiteDomain, "hub", "forest", "swamp", "cave", "other");

            FiniteDomainVariable<String> c = new FiniteDomainVariable<String>(solver, "c", finiteDomain, "hub", "forest", "swamp", "cave", "other");

            FiniteDomainVariable<String> d = new FiniteDomainVariable<String>(solver, "d", finiteDomain, "hub", "forest", "swamp", "cave", "other");

            FiniteDomainVariable<String> e = new FiniteDomainVariable<String>(solver, "e", finiteDomain, "hub", "forest", "swamp", "cave", "other");

            FiniteDomainVariable<String> f = new FiniteDomainVariable<String>(solver, "f", finiteDomain, "hub", "forest", "swamp", "cave", "other");


            FiniteDomainVariable<String>[] vars = new[] { a, b, c, d, e, f };

            Constraint.RequireOccurences("hub", 1, vars);
            Constraint.RequireOccurences("forest", 1, vars);
            Constraint.RequireOccurences("swamp", 1, vars);
            Constraint.RequireOccurences("cave", 1, vars);
        }
    }
}