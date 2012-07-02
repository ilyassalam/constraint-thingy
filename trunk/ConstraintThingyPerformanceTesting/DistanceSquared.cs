using ConstraintThingy;

namespace ConstraintThingyPerformanceTesting
{
    /// <summary>
    /// Selects two points that are between 5 and 10 units away
    /// </summary>
    class DistanceSquared : PerformanceTest
    {
        protected override void InitializeConstraintSystem(ConstraintThingySolver solver)
        {
            RealVariable x1 = solver.CreateRealVariable();
            RealVariable y1 = solver.CreateRealVariable();

            RealVariable x2 = solver.CreateRealVariable();
            RealVariable y2 = solver.CreateRealVariable();

            RealVariable distanceSquared = Constraint.DistanceSquared(new Vector2Variable(x1, y1), new Vector2Variable(x2, y2));

            Constraint.InRange(distanceSquared, 25, 100);
        }
    }
}