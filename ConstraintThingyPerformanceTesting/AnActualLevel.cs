using System;
using System.Linq;
using CSharpUtils;
using ConstraintThingy;

namespace ConstraintThingyPerformanceTesting
{
    class AnActualLevel : PerformanceTest
    {
        protected override void InitializeConstraintSystem(ConstraintThingySolver solver)
        {
            // topology: https://docs.google.com/a/u.northwestern.edu/drawings/d/1mpZ2gPq7D8mab7PI2PtV4bKYCrrtb7ZS3hg5gwgMa_E/
            //

            // declare all of the room types

            FiniteDomain<String> roomType = new FiniteDomain<string>("start", "empty", "small-health-pack", "big-health-pack", "zombie", "two zombies", "dog", "trap", "boss", "locked-door", "end");

            FiniteDomainVariable<String>[] roomTypes = new FiniteDomainVariable<string>[15];
            roomTypes[0] = solver.CreateFiniteDomainVariable(roomType, "start");

            for (int i = 1; i <= 13; i++)
            {
                roomTypes[i] = solver.CreateFiniteDomainVariable(roomType, "empty", "small-health-pack", "big-health-pack", "zombie", "two zombies", "dog", "trap", "boss", "locked-door");
            }

            roomTypes[14] = solver.CreateFiniteDomainVariable(roomType, "end");


            // assert that adjacent rooms cannot have the same content
            Constraint.NotEqual(roomTypes[0], roomTypes[1]);
            Constraint.NotEqual(roomTypes[1], roomTypes[2]);
            Constraint.NotEqual(roomTypes[2], roomTypes[3]);
            Constraint.NotEqual(roomTypes[3], roomTypes[4]);

            Constraint.NotEqual(roomTypes[1], roomTypes[5]);
            Constraint.NotEqual(roomTypes[5], roomTypes[6]);
            Constraint.NotEqual(roomTypes[6], roomTypes[7]);

            Constraint.NotEqual(roomTypes[7], roomTypes[8]);
            Constraint.NotEqual(roomTypes[4], roomTypes[8]);

            Constraint.NotEqual(roomTypes[8], roomTypes[9]);
            Constraint.NotEqual(roomTypes[9], roomTypes[10]);

            Constraint.NotEqual(roomTypes[10], roomTypes[11]);
            Constraint.NotEqual(roomTypes[10], roomTypes[12]);
            Constraint.NotEqual(roomTypes[11], roomTypes[13]);
            Constraint.NotEqual(roomTypes[12], roomTypes[13]);
            Constraint.NotEqual(roomTypes[13], roomTypes[14]);

            ScoreMapping<String> damages = new ScoreMapping<String>(roomType,
                "small-health-pack".PairedWith(new Interval(10)),
                "big-health-pack".PairedWith(new Interval(20)),
                "start".PairedWith(new Interval(0)),
                "empty".PairedWith(new Interval(0)),
                "zombie".PairedWith(new Interval(-10)),
                "two zombies".PairedWith(new Interval(-25)),
                "dog".PairedWith(new Interval(-13)),
                "trap".PairedWith(new Interval(-10)),
                "boss".PairedWith(new Interval(-30)),
                "locked-door".PairedWith(new Interval(0)),
                "end".PairedWith(new Interval(0)));

            // create variables to contain how much damage is dealt in each room

            RealVariable[] roomDelta = new RealVariable[15];

            roomDelta[0] = solver.CreateRealVariable(0);
            roomDelta[0].RequireUnique = false;

            for (int i = 1; i <= 13; i++)
            {
                roomDelta[i] = Constraint.ScoreVariable(roomTypes[i], damages);
                roomDelta[i].RequireUnique = false;
            }

            roomDelta[14] = solver.CreateRealVariable(0);
            roomDelta[14].RequireUnique = false;


            // variables for how much health the player has in each room
            // under the assumptions of forward progress in the paper

            RealVariable[] playerHealth = new RealVariable[15];

            // start at 100
            playerHealth[0] = solver.CreateRealVariable(100);
            playerHealth[0].RequireUnique = false;

            // player must stay between 0 and 100 health
            for (int i = 1; i <= 14; i++)
            {
                playerHealth[i] = solver.CreateRealVariable(10, 100);
                playerHealth[i].RequireUnique = false;
            }

            Constraint.Sum(playerHealth[1], roomDelta[1], playerHealth[0]);
            Constraint.Sum(playerHealth[2], roomDelta[2], playerHealth[1]);
            Constraint.Sum(playerHealth[3], roomDelta[3], playerHealth[2]);
            Constraint.Sum(playerHealth[4], roomDelta[4], playerHealth[3]);


            Constraint.Sum(playerHealth[5], roomDelta[5], playerHealth[1]);
            Constraint.Sum(playerHealth[6], roomDelta[6], playerHealth[5]);
            Constraint.Sum(playerHealth[7], roomDelta[7], playerHealth[6]);

            Constraint.Sum(playerHealth[8], roomDelta[8], Constraint.Minimize(playerHealth[4], playerHealth[7]).With(r => r.RequireUnique = false));

            Constraint.Sum(playerHealth[9], roomDelta[9], playerHealth[8]);
            Constraint.Sum(playerHealth[10], roomDelta[10], playerHealth[9]);

            Constraint.Sum(playerHealth[11], roomDelta[11], playerHealth[10]);
            Constraint.Sum(playerHealth[12], roomDelta[12], playerHealth[10]);

            Constraint.Sum(playerHealth[13], roomDelta[13], Constraint.Minimize(playerHealth[11], playerHealth[12]).With(r => r.RequireUnique = false));

            Constraint.Sum(playerHealth[14], roomDelta[14], playerHealth[13]);

            ScoreMapping<String> keyLockScoring = new ScoreMapping<String>(roomType,
                    "small-health-pack".PairedWith(new Interval(0)),
                    "big-health-pack".PairedWith(new Interval(0)),
                    "start".PairedWith(new Interval(0)),
                    "empty".PairedWith(new Interval(0)),
                    "zombie".PairedWith(new Interval(0)),
                    "two zombies".PairedWith(new Interval(0)),
                    "dog".PairedWith(new Interval(0)),
                    "trap".PairedWith(new Interval(0)),
                    "boss".PairedWith(new Interval(2)),
                    "locked-door".PairedWith(new Interval(-1)),
                    "end".PairedWith(new Interval(0)));

            // 3 things need to happen for a key and lock puzzle.
            //          1) player needs to first encounter the boss
            //          2) player needs to then encounter the locked door
            //          3) player needs to finally encounter the exit

            // we say a boss provides a value of +2
            // we say the locked door provides a value of -1
            // and we say that we need to have a value of +1 at the exit
            // with the restriction that we never dip negative along the path integral
            // so that placing a locked door before a guaranteed boss would be dissallowed

            // we use 'min' as our aggregation function

            RealVariable[] lockKeyDelta = new RealVariable[15];

            lockKeyDelta[0] = solver.CreateRealVariable(0);
            lockKeyDelta[0].RequireUnique = false;

            for (int i = 1; i <= 14; i++)
            {
                lockKeyDelta[i] = Constraint.ScoreVariable(roomTypes[i], keyLockScoring);
                lockKeyDelta[i].RequireUnique = true;
                lockKeyDelta[i].Precision = .2;
                lockKeyDelta[i].Priority = 2;
            }

            RealVariable[] lockKeyRunningSum = new RealVariable[15];

            // start at 0
            lockKeyRunningSum[0] = solver.CreateRealVariable(0);
            lockKeyRunningSum[0].RequireUnique = true;

            // require the running sum to stay positive
            for (int i = 1; i <= 14; i++)
            {
                lockKeyRunningSum[i] = solver.CreateRealVariable(0, 2.5);
                lockKeyRunningSum[i].RequireUnique = false;
            }


            // require that the sum at the end be basically equal to 1
            Constraint.InRange(lockKeyRunningSum[14], .9, 1.1);

            Constraint.Sum(lockKeyRunningSum[1], lockKeyDelta[1], lockKeyRunningSum[0]);
            Constraint.Sum(lockKeyRunningSum[2], lockKeyDelta[2], lockKeyRunningSum[1]);
            Constraint.Sum(lockKeyRunningSum[3], lockKeyDelta[3], lockKeyRunningSum[2]);
            Constraint.Sum(lockKeyRunningSum[4], lockKeyDelta[4], lockKeyRunningSum[3]);

            Constraint.Sum(lockKeyRunningSum[5], lockKeyDelta[5], lockKeyRunningSum[1]);
            Constraint.Sum(lockKeyRunningSum[6], lockKeyDelta[6], lockKeyRunningSum[5]);
            Constraint.Sum(lockKeyRunningSum[7], lockKeyDelta[7], lockKeyRunningSum[6]);

            Constraint.Equal(lockKeyRunningSum[4], lockKeyRunningSum[7]);

            Constraint.Sum(lockKeyRunningSum[8], lockKeyDelta[8], lockKeyRunningSum[4]);

            Constraint.Sum(lockKeyRunningSum[9], lockKeyDelta[9], lockKeyRunningSum[8]);
            Constraint.Sum(lockKeyRunningSum[10], lockKeyDelta[10], lockKeyRunningSum[9]);

            Constraint.Sum(lockKeyRunningSum[11], lockKeyDelta[11], lockKeyRunningSum[10]);
            Constraint.Sum(lockKeyRunningSum[12], lockKeyDelta[12], lockKeyRunningSum[10]);

            Constraint.Equal(lockKeyRunningSum[11], lockKeyRunningSum[12]);

            Constraint.Sum(lockKeyRunningSum[13], lockKeyDelta[13], lockKeyRunningSum[11]);

            Constraint.Sum(lockKeyRunningSum[14], lockKeyDelta[14], lockKeyRunningSum[13]);

            Constraint.LimitOccurences("trap", 1, 2, roomTypes);
            Constraint.MaximumOccurences("small-health-pack", 1, roomTypes);
            Constraint.MaximumOccurences("big-health-pack", 1, roomTypes);
            Constraint.MaximumOccurences("empty", 2, roomTypes);

            Constraint.RequireOccurences("boss", 1, roomTypes);
            Constraint.RequireOccurences("locked-door", 1, roomTypes);
        }
    }
}