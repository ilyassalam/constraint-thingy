using System.Linq;
using ConstraintThingy;

namespace ConstraintThingyPerformanceTesting
{
    /// <summary>
    /// Enumerates through a bunch of unconstrainted variables forever
    /// </summary>
    class UnconstraintedEnumeration : PerformanceTest
    {
        protected override void InitializeConstraintSystem(ConstraintThingySolver solver)
        {
            var letters = "abcdefghijklmnopqrstuvwxyz".ToCharArray().Select(c => new string(c, 1));

            FiniteDomain<string> finiteDomain = new FiniteDomain<string>(letters.ToArray());

            for (int i = 0; i < 25; i++)
            {
                new FiniteDomainVariable<string>(solver, null, finiteDomain, letters.ToArray());
            }
        }
    }
}