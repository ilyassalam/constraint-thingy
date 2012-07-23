using System.Linq;
using CSharpUtils;
using ConstraintThingy;

namespace ConstraintThingyPerformanceTesting
{
    /// <summary>
    /// Selects room types along a path such that the level is solvable and the player's health is always above 0 along the path
    /// </summary>
    class PathPlayability : PerformanceTest
    {
        protected override void InitializeConstraintSystem(ConstraintThingySolver solver)
        {
            const int numRooms = 5;

            FiniteDomain<string> finiteDomain = new FiniteDomain<string>("enemy", "health-pack", "ammo", "boss", "puzzle");

            FiniteDomainVariable<string>[] roomTypes = new FiniteDomainVariable<string>[numRooms];

            for (int i = 0; i < numRooms; i++)
                roomTypes[i] = solver.CreateFiniteDomainVariable(finiteDomain, "enemy", "health-pack", "ammo", "boss", "puzzle");

            // exactly 1 boss in the level
            Constraint.RequireOccurences("boss", 1, roomTypes);

            // exactly 1 puzzle in the level
            Constraint.RequireOccurences("puzzle", 1, roomTypes);

            RealVariable[] roomScores = new RealVariable[numRooms];

            var scoreMapping = new ScoreMapping<string>(finiteDomain,
                                                        "enemy".PairedWith(new Interval(-3, -3)),
                                                        "health-pack".PairedWith(new Interval(2, 2)),
                                                        "ammo".PairedWith(new Interval(2, 2)),
                                                        "boss".PairedWith(new Interval(-6, -6)),
                                                        "puzzle".PairedWith(new Interval(0, 0)));

            for (int i = 0; i < numRooms; i++)
            {
                roomScores[i] = Constraint.ScoreVariable(roomTypes[i], scoreMapping);
            }

            var sum = roomScores.Aggregate((a, b) =>
                                               {
                                                   var result = Constraint.Add((RealVariable) a, b);
                                                   Constraint.GreaterThanOrEqual(result, 0);

                                                   return result;
                                               });

            Constraint.InRange(sum, 0, 10);
        }
    }
}