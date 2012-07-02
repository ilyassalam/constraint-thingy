using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using CSharpUtils;
using ConstraintThingy;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ConstraintThingyTest
{
    public partial class RealVariableTests
    {
        [TestMethod]
        public void EnemyHealthPack1()
        {
            ConstraintThingySolver solver = new ConstraintThingySolver();

            FiniteDomain<String> finiteDomain = new FiniteDomain<string>("enemy", "health-pack");

            FiniteDomainVariable<String> variable = solver.CreateFiniteDomainVariable(finiteDomain, "enemy", "health-pack");

            RealVariable score = solver.CreateRealVariable(0, 5);

            new ScoreContraint<String>(score, variable, new ScoreMapping<string>(finiteDomain, "enemy".PairedWith(new Interval(-3, -3)), "health-pack".PairedWith(new Interval(3, 3))));
            int solutionCount = 0;

            foreach (var solution in solver.Solutions)
            {
                solutionCount++;
                Assert.AreEqual(new Interval(3, 3), score.UniqueValue);
                Assert.AreEqual("health-pack", variable.UniqueValue);
            }

            Assert.AreEqual(1, solutionCount);
        }

        [TestMethod]
        public void EnemyHealthPack2()
        {
            ConstraintThingySolver solver = new ConstraintThingySolver();

            FiniteDomain<String> finiteDomain = new FiniteDomain<string>("enemy", "health-pack")
                                                    {
                                                        
                                                    };

            FiniteDomainVariable<String> roomType = solver.CreateFiniteDomainVariable(finiteDomain, "enemy", "health-pack");

            RealVariable score = solver.CreateRealVariable(0, 5);

            // How this reads:
            //     Enemies will always do 3 damage to the player
            //     Health packs arbitarily restore between 3 and 4 health

            // Once solved, "roomType" will hold which kind of room was selected
            // and "score" will hold the strength of the item or enemy

            // in this case, only health packs can be assigned to the interval (0,5), so "score" will 
            // contain the strength of the health pack, which is some arbitrary number between 3 and 4.

            new ScoreContraint<String>(score, roomType, new ScoreMapping<string>(finiteDomain, "enemy".PairedWith(new Interval(-3, -3)), "health-pack".PairedWith(new Interval(3, 4))));

            foreach (var solution in solver.Solutions.FirstElements(10))
            {
                AssertIntersect(new Interval(3, 4), score.UniqueValue);
                Assert.AreEqual("health-pack", roomType.UniqueValue);
            }
        }

        [TestMethod]
        public void EnemyHealthPack3()
        {
            ConstraintThingySolver solver = new ConstraintThingySolver();

            FiniteDomain<String> finiteDomain = new FiniteDomain<string>("enemy", "health-pack")
                                                    {
                                                        
                                                    };

            FiniteDomainVariable<String> roomType = solver.CreateFiniteDomainVariable(finiteDomain, "enemy", "health-pack");

            RealVariable score = solver.CreateRealVariable(0, 5);

            // How this reads:
            //     Enemies will always do 3 damage to the player
            //     Health packs arbitarily restore between 3 and 4 health

            // Once solved, "roomType" will hold which kind of room was selected
            // and "score" will hold the strength of the item or enemy

            // in this case, only health packs can be assigned to the interval (0,5), so "score" will 
            // contain the strength of the health pack, which is some arbitrary number between 3 and 5.

            new ScoreContraint<String>(score, roomType, new ScoreMapping<string>(finiteDomain, "enemy".PairedWith(new Interval(-3, -3)), "health-pack".PairedWith(new Interval(3, 100))));

            foreach (var solution in solver.Solutions.FirstElements(10))
            {
                AssertIntersect(new Interval(3, 5), score.UniqueValue);
                Assert.AreEqual("health-pack", roomType.UniqueValue);
            }
        }

        [TestMethod]
        public void EnemyHealthPack4()
        {
            const int numRooms = 2;

            ConstraintThingySolver solver = new ConstraintThingySolver();

            FiniteDomain<String> finiteDomain = new FiniteDomain<string>("enemy", "health-pack");

            FiniteDomainVariable<String>[] roomTypes = new FiniteDomainVariable<string>[numRooms];

            for (int i = 0; i < numRooms; i++) roomTypes[i] = solver.CreateFiniteDomainVariable(finiteDomain, "enemy", "health-pack");

            RealVariable[] roomScores = new RealVariable[numRooms];

            var scoreMapping = new ScoreMapping<String>(finiteDomain, new Interval(-3, -3), new Interval(4, 4));

            for (int i = 0; i < numRooms; i++)
            {
                roomScores[i] = solver.CreateEnumeratedReal(roomTypes[i], scoreMapping);
            }

            var sum = Constraint.Add(roomScores);

            Constraint.InRange(sum, -2, 2);

            foreach (var solution in solver.Solutions.FirstElements(1))
            {
                Assert.IsTrue(roomTypes.Count(variable => variable.UniqueValue == "enemy") >= 1);
                Assert.IsTrue(roomTypes.Count(variable => variable.UniqueValue == "health-pack") >= 1);

                AssertIntersect(new Interval(-2, 2), sum.UniqueValue);
            }
        }

        [TestMethod, Timeout(1000)]
        public void EnemyHealthPack5()
        {
            const int numRooms = 3;

            ConstraintThingySolver solver = new ConstraintThingySolver();

            FiniteDomain<String> finiteDomain = new FiniteDomain<string>("enemy", "health-pack");

            FiniteDomainVariable<String>[] roomTypes = new FiniteDomainVariable<string>[numRooms];

            for (int i = 0; i < numRooms; i++) roomTypes[i] = solver.CreateFiniteDomainVariable(finiteDomain, "enemy", "health-pack");

            RealVariable[] roomScores = new RealVariable[numRooms];

            var scoreMapping = new ScoreMapping<String>(finiteDomain, new Interval(-3, -3), new Interval(3, 3));

            for (int i = 0; i < numRooms; i++)
            {
                roomScores[i] = solver.CreateEnumeratedReal(roomTypes[i], scoreMapping);
            }

            var sum = Constraint.Add(roomScores);

            Constraint.InRange(sum, 0, 4);

            foreach (var solution in solver.Solutions.FirstElements(1))
            {
                AssertIntersect(new Interval(0, 4), sum.UniqueValue);
            }
        }


        [TestMethod]
        public void PopulationWithOccurences()
        {
            const int numRooms = 5;

            ConstraintThingySolver solver = new ConstraintThingySolver();

            // this the finite domain variables to be expanded first -- we test without this below
            solver.ExpansionOrder = ExpansionOrder.Deterministic;

            FiniteDomain<String> finiteDomain = new FiniteDomain<string>("enemy", "health-pack", "ammo", "boss", "puzzle");

            FiniteDomainVariable<String>[] roomTypes = new FiniteDomainVariable<string>[numRooms];

            for (int i = 0; i < numRooms; i++) roomTypes[i] = solver.CreateFiniteDomainVariable(finiteDomain, "enemy", "health-pack", "ammo", "boss", "puzzle");

            // exactly 1 boss in the level
            Constraint.RequireOccurences("boss", 1, roomTypes);

            // at most 1 puzzle in the level
            Constraint.MaximumOccurences("puzzle", 1, roomTypes);

            RealVariable[] roomScores = new RealVariable[numRooms];

            var scoreMapping = new ScoreMapping<String>(finiteDomain, 
                "enemy".PairedWith(new Interval(-3, -3)),
                "health-pack".PairedWith(new Interval(2, 2)), 
                "ammo".PairedWith(new Interval(2, 2)),
                "boss".PairedWith(new Interval(-6, -6)),
                "puzzle".PairedWith(new Interval(0, 0)));

            for (int i = 0; i < numRooms; i++)
            {
                roomScores[i] = solver.CreateEnumeratedReal(roomTypes[i], scoreMapping);
            }

            var sum = roomScores.Aggregate((a,b) =>
                                               {
                                                   var result = Constraint.Add(a, b);

                                                   Constraint.GreaterThanOrEqual(result, 0);

                                                   return result;
                                               });

            Constraint.InRange(sum, 0, 10);

            foreach (var solution in solver.Solutions.FirstElements(10))
            {
                AssertIntersect(new Interval(0, 10), sum.UniqueValue);
            }
        }

        [TestMethod, Timeout(2000)]
        public void PlayabilityTermination1()
        {
            int randomSeed = 4;
            const int numRooms = 5;

            PlayabilityExperiment(numRooms, randomSeed);
        }

        [TestMethod, Timeout(2000)]
        public void PlayabilityTermination2()
        {
            int randomSeed = 122;
            const int numRooms = 5;

            PlayabilityExperiment(numRooms, randomSeed);
        }

        [TestMethod, Timeout(2000)]
        public void PlayabilityTermination3()
        {
            int randomSeed = 864;
            const int numRooms = 5;

            PlayabilityExperiment(numRooms, randomSeed);
        }

        [TestMethod, Timeout(2000)]
        public void PlayabilityTermination4()
        {
            int randomSeed = 222;
            const int numRooms = 5;

            PlayabilityExperiment(numRooms, randomSeed);
        }

        [TestMethod, Timeout(2000)]
        public void PlayabilityTermination5()
        {
            int randomSeed = 343;
            const int numRooms = 5;

            PlayabilityExperiment(numRooms, randomSeed);
        }

        [TestMethod, Timeout(2000)]
        public void PlayabilityTermination6()
        {
            int randomSeed = 652113;
            const int numRooms = 5;

            PlayabilityExperiment(numRooms, randomSeed);
        }

        [TestMethod, Timeout(2000)]
        public void PlayabilityTermination7()
        {
            int randomSeed = 6527223;
            const int numRooms = 5;

            PlayabilityExperiment(numRooms, randomSeed);
        }

        [TestMethod, Timeout(2000)]
        public void PlayabilityTermination8()
        {
            int randomSeed = 699523;
            const int numRooms = 5;

            PlayabilityExperiment(numRooms, randomSeed);
        }

        [TestMethod, Timeout(2000)]
        public void PlayabilityTermination9()
        {
            int randomSeed = 656923;
            const int numRooms = 5;

            PlayabilityExperiment(numRooms, randomSeed);
        }

        [TestMethod, Timeout(2000)]
        public void PlayabilityTermination10()
        {
            int randomSeed = 2126523;
            const int numRooms = 5;

            PlayabilityExperiment(numRooms, randomSeed);
        }

        [TestMethod, Timeout(2000)]
        public void PlayabilityTermination11()
        {
            int randomSeed = 622223;
            const int numRooms = 5;

            PlayabilityExperiment(numRooms, randomSeed);
        }

        [TestMethod, Timeout(2000)]
        public void PlayabilityTermination12()
        {
            int randomSeed = 4232;
            const int numRooms = 5;

            PlayabilityExperiment(numRooms, randomSeed);
        }

        [TestMethod, Timeout(2000)]
        public void PlayabilityTermination13()
        {
            int randomSeed = 95092;
            const int numRooms = 5;

            PlayabilityExperiment(numRooms, randomSeed);
        }

        private static void PlayabilityExperiment(int numRooms, int randomSeed)
        {
            ConstraintThingySolver solver = new ConstraintThingySolver(randomSeed);

            FiniteDomain<String> finiteDomain = new FiniteDomain<string>("enemy", "health-pack", "ammo", "boss", "puzzle");

            FiniteDomainVariable<String>[] roomTypes = new FiniteDomainVariable<string>[numRooms];

            for (int i = 0; i < numRooms; i++)
                roomTypes[i] = solver.CreateFiniteDomainVariable(finiteDomain, "enemy", "health-pack", "ammo", "boss", "puzzle");

            // exactly 1 boss in the level
            Constraint.RequireOccurences("boss", 1, roomTypes);

            // exactly 1 puzzle in the level
            Constraint.RequireOccurences("puzzle", 1, roomTypes);

            RealVariable[] roomScores = new RealVariable[numRooms];

            var scoreMapping = new ScoreMapping<String>(finiteDomain,
                                                        "enemy".PairedWith(new Interval(-3, -3)),
                                                        "health-pack".PairedWith(new Interval(2, 2)),
                                                        "ammo".PairedWith(new Interval(2, 2)),
                                                        "boss".PairedWith(new Interval(-6, -6)),
                                                        "puzzle".PairedWith(new Interval(0, 0)));

            for (int i = 0; i < numRooms; i++)
            {
                roomScores[i] = solver.CreateEnumeratedReal(roomTypes[i], scoreMapping);
            }

            var sum = roomScores.Aggregate((a, b) =>
                                               {
                                                   var result = Constraint.Add(a, b);
                                                   Constraint.GreaterThanOrEqual(result, 0);

                                                   return result;
                                               });

            Constraint.InRange(sum, 0, 10);

            foreach (var solution in solver.Solutions.FirstElements(10))
            {
                AssertIntersect(new Interval(0, 10), sum.UniqueValue);
                Assert.AreEqual(1, roomTypes.Count(room => room.UniqueValue == "puzzle"));
                Assert.AreEqual(1, roomTypes.Count(room => room.UniqueValue == "boss"));
            }
        }

        [TestMethod]
        public void ImpossiblePlayability()
        {
            const int numRooms = 5;

            ConstraintThingySolver solver = new ConstraintThingySolver();

            FiniteDomain<String> finiteDomain = new FiniteDomain<string>("enemy", "health-pack", "ammo", "boss", "puzzle");

            FiniteDomainVariable<String>[] roomTypes = new FiniteDomainVariable<string>[numRooms];

            for (int i = 0; i < numRooms; i++) roomTypes[i] = solver.CreateFiniteDomainVariable(finiteDomain, "enemy", "health-pack", "ammo", "boss", "puzzle");

            // exactly 1 boss in the level
            Constraint.RequireOccurences("boss", 1, roomTypes);

            // at most 1 puzzle in the level
            Constraint.MaximumOccurences("puzzle", 1, roomTypes);

            RealVariable[] roomScores = new RealVariable[numRooms];

            var scoreMapping = new ScoreMapping<String>(finiteDomain,
                "enemy".PairedWith(new Interval(-3, -3)),
                "health-pack".PairedWith(new Interval(2, 2)),
                "ammo".PairedWith(new Interval(2, 2)),
                // make placing a boss super expensive so there's no valid solution, yet still constrained to appear
                "boss".PairedWith(new Interval(-600, -600)),
                "puzzle".PairedWith(new Interval(0, 0)));

            for (int i = 0; i < numRooms; i++)
            {
                roomScores[i] = solver.CreateEnumeratedReal(roomTypes[i], scoreMapping);
            }

            var sum = Constraint.Add(roomScores);

            Constraint.InRange(sum, 0, 10);

            Assert.AreEqual(0, solver.Solutions.Count());
        }

    }
}
