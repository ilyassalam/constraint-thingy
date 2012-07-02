using ConstraintThingy;

namespace ConstraintThingyPerformanceTesting
{
    class InequalityPropogation : PerformanceTest
    {
        protected override void InitializeConstraintSystem(ConstraintThingySolver solver)
        {
            var letters = "abcde".ToCharArray();

            FiniteDomain<char> finiteDomain = new FiniteDomain<char>(letters);

            var a = new FiniteDomainVariable<char>(solver, "a", finiteDomain, letters);
            var b = new FiniteDomainVariable<char>(solver, "b", finiteDomain, letters);
            var c = new FiniteDomainVariable<char>(solver, "c", finiteDomain, letters);
            var d = new FiniteDomainVariable<char>(solver, "d", finiteDomain, letters);
            var e = new FiniteDomainVariable<char>(solver, "e", finiteDomain, letters);

            Constraint.NotEqual(a, b, c, d, e);
        }
    }
}