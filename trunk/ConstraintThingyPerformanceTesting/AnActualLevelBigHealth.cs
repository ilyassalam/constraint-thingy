using System;
using CSharpUtils;
using ConstraintThingy;

namespace ConstraintThingyPerformanceTesting
{
    class AnActualLevelBigKeysAndLocks : PerformanceTest
    {
        protected override void InitializeConstraintSystem(ConstraintThingySolver solver)
        {
            // topology: https://docs.google.com/a/u.northwestern.edu/drawings/d/1mpZ2gPq7D8mab7PI2PtV4bKYCrrtb7ZS3hg5gwgMa_E/
            //

            // declare all of the room types

            FiniteDomain<String> roomType = new FiniteDomain<string>("key", "locked-door", "empty");

            FiniteDomainVariable<String>[] roomTypes = new FiniteDomainVariable<string>[41];

            for (int i = 0; i <= 40; i++)
            {
                roomTypes[i] = solver.CreateFiniteDomainVariable(roomType, "key", "locked-door", "empty");
            }

            /*
            for (int i = 0; i <= 40; i++)
            {
                if (i != 1 && i != 8 && i != 9 && i != 10 && i != 13)
                {
                    Constraint.NotEqual(roomTypes[i], "key");

                    Constraint.NotEqual(roomTypes[i], "locked-door");
                }
            }*/

            ScoreMapping<String> keyLockScoring = new ScoreMapping<String>(roomType,
                    "key".PairedWith(new Interval(1)),
                    "locked-door".PairedWith(new Interval(-1)),
                    "empty".PairedWith(new Interval(0)));

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

            RealVariable[] lockKeyDelta = new RealVariable[41];

            for (int i = 0; i <= 40; i++)
            {
                lockKeyDelta[i] = Constraint.ScoreVariable(roomTypes[i], keyLockScoring);
                lockKeyDelta[i].RequireUnique = false;
            }

            RealVariable[] lockKeyRunningSum = new RealVariable[41];

            // require the running sum to stay positive
            for (int i = 0; i <= 40; i++)
            {
                lockKeyRunningSum[i] = solver.CreateRealVariable(0, 100);
                lockKeyRunningSum[i].RequireUnique = false;
            }

            // start at 0.
            Constraint.Equal(lockKeyRunningSum[0], 0);
            // end at 0.
            Constraint.InRange(lockKeyRunningSum[14], 0, 0);


            Constraint.Sum(lockKeyRunningSum[1], lockKeyDelta[1], lockKeyRunningSum[0]);
            Constraint.Sum(lockKeyRunningSum[2], lockKeyDelta[2], lockKeyRunningSum[1]);

            Constraint.Equal(lockKeyRunningSum[2], lockKeyRunningSum[16]);

            Constraint.Sum(lockKeyRunningSum[3], lockKeyDelta[3], lockKeyRunningSum[2]);

            Constraint.Equal(lockKeyRunningSum[3], lockKeyRunningSum[23], lockKeyRunningSum[16], lockKeyRunningSum[18]);
            Constraint.Sum(lockKeyRunningSum[4], lockKeyDelta[4], lockKeyRunningSum[3]);

            Constraint.Sum(lockKeyRunningSum[5], lockKeyDelta[5], lockKeyRunningSum[1]);

            Constraint.Equal(lockKeyRunningSum[5], lockKeyRunningSum[25]);
            Constraint.Sum(lockKeyRunningSum[6], lockKeyDelta[6], lockKeyRunningSum[5]);

            Constraint.Equal(lockKeyRunningSum[6], lockKeyRunningSum[27]);
            Constraint.Sum(lockKeyRunningSum[7], lockKeyDelta[7], lockKeyRunningSum[6]);

            Constraint.Equal(lockKeyRunningSum[4], lockKeyRunningSum[7]);
            Constraint.Sum(lockKeyRunningSum[8], lockKeyDelta[8], lockKeyRunningSum[4]);

            Constraint.Equal(lockKeyRunningSum[8], lockKeyRunningSum[35], lockKeyRunningSum[39]);
            Constraint.Sum(lockKeyRunningSum[9], lockKeyDelta[9], lockKeyRunningSum[8]);
            Constraint.Sum(lockKeyRunningSum[10], lockKeyDelta[10], lockKeyRunningSum[9]);

            Constraint.Sum(lockKeyRunningSum[11], lockKeyDelta[11], lockKeyRunningSum[10]);
            Constraint.Sum(lockKeyRunningSum[12], lockKeyDelta[12], lockKeyRunningSum[10]);

            Constraint.Equal(lockKeyRunningSum[11], lockKeyRunningSum[12], lockKeyRunningSum[10], lockKeyRunningSum[22], lockKeyRunningSum[32]);
            Constraint.Sum(lockKeyRunningSum[13], lockKeyDelta[13], lockKeyRunningSum[11]);

            Constraint.Sum(lockKeyRunningSum[14], lockKeyDelta[14], lockKeyRunningSum[13]);

            Constraint.Sum(lockKeyRunningSum[15], lockKeyDelta[15], lockKeyRunningSum[2]);
            Constraint.Sum(lockKeyRunningSum[16], lockKeyDelta[16], lockKeyRunningSum[15]);
            Constraint.Sum(lockKeyRunningSum[17], lockKeyDelta[17], lockKeyRunningSum[16]);

            Constraint.Equal(lockKeyRunningSum[15], lockKeyRunningSum[17]);
            Constraint.Sum(lockKeyRunningSum[18], lockKeyDelta[18], lockKeyRunningSum[15]);
            Constraint.Sum(lockKeyRunningSum[19], lockKeyDelta[19], lockKeyRunningSum[10]);

            Constraint.Equal(lockKeyRunningSum[19], lockKeyRunningSum[11]);
            Constraint.Sum(lockKeyRunningSum[20], lockKeyDelta[20], lockKeyRunningSum[19]);
            Constraint.Sum(lockKeyRunningSum[21], lockKeyDelta[21], lockKeyRunningSum[11]);
            Constraint.Sum(lockKeyRunningSum[22], lockKeyDelta[22], lockKeyRunningSum[21]);

            Constraint.Equal(lockKeyRunningSum[2], lockKeyRunningSum[5]);
            Constraint.Sum(lockKeyRunningSum[23], lockKeyDelta[23], lockKeyRunningSum[2]);

            Constraint.Sum(lockKeyRunningSum[24], lockKeyDelta[24], lockKeyRunningSum[1]);
            Constraint.Sum(lockKeyRunningSum[25], lockKeyDelta[25], lockKeyRunningSum[24]);
            Constraint.Equal(lockKeyRunningSum[25], lockKeyRunningSum[6]);
            Constraint.Sum(lockKeyRunningSum[26], lockKeyDelta[26], lockKeyRunningSum[25]);
            Constraint.Sum(lockKeyRunningSum[27], lockKeyDelta[27], lockKeyRunningSum[26]);

            Constraint.Sum(lockKeyRunningSum[28], lockKeyDelta[28], lockKeyRunningSum[10]);

            Constraint.Equal(lockKeyRunningSum[28], lockKeyRunningSum[12]);
            Constraint.Sum(lockKeyRunningSum[29], lockKeyDelta[29], lockKeyRunningSum[28]);

            Constraint.Equal(lockKeyRunningSum[29], lockKeyRunningSum[12]);
            Constraint.Sum(lockKeyRunningSum[30], lockKeyDelta[30], lockKeyRunningSum[29]);
            Constraint.Equal(lockKeyRunningSum[30], lockKeyRunningSum[12]);
            Constraint.Sum(lockKeyRunningSum[31], lockKeyDelta[31], lockKeyRunningSum[30]);
            Constraint.Sum(lockKeyRunningSum[32], lockKeyDelta[32], lockKeyRunningSum[31]);

            Constraint.Sum(lockKeyRunningSum[33], lockKeyDelta[33], lockKeyRunningSum[8]);
            Constraint.Sum(lockKeyRunningSum[34], lockKeyDelta[34], lockKeyRunningSum[33]);

            Constraint.Equal(lockKeyRunningSum[33], lockKeyRunningSum[34]);
            Constraint.Sum(lockKeyRunningSum[35], lockKeyDelta[35], lockKeyRunningSum[33]);

            Constraint.Minimize(lockKeyRunningSum[8], lockKeyRunningSum[33]);
            Constraint.Sum(lockKeyRunningSum[36], lockKeyDelta[36], lockKeyRunningSum[8]);
            Constraint.Sum(lockKeyRunningSum[37], lockKeyDelta[37], lockKeyRunningSum[36]);
            Constraint.Sum(lockKeyRunningSum[38], lockKeyDelta[38], lockKeyRunningSum[37]);

            Constraint.Equal(lockKeyRunningSum[40], lockKeyRunningSum[37], lockKeyRunningSum[38]);
            Constraint.Sum(lockKeyRunningSum[39], lockKeyDelta[39], lockKeyRunningSum[40]);

            Constraint.Equal(lockKeyRunningSum[33], lockKeyRunningSum[36], lockKeyRunningSum[37]);
            Constraint.Sum(lockKeyRunningSum[40], lockKeyDelta[40], lockKeyRunningSum[33]);
            
            //    Constraint.LimitOccurences("trap", 2, 4, roomTypes);

            //            Constraint.MaximumOccurences("small-health-pack", 5, roomTypes);
            //Constraint.MaximumOccurences("big-health-pack", 6, roomTypes);

            //     Constraint.RequireOccurences("boss", 1, roomTypes);

            //Constraint.MaximumOccurences("key", 5, roomTypes);
            //Constraint.MaximumOccurences("locked-door", 5, roomTypes);
        }
    }

    class AnActualLevelBigHealth : PerformanceTest
    {
        protected override void InitializeConstraintSystem(ConstraintThingySolver solver)
        {
            // topology: https://docs.google.com/a/u.northwestern.edu/drawings/d/1mpZ2gPq7D8mab7PI2PtV4bKYCrrtb7ZS3hg5gwgMa_E/
            //

            // declare all of the room types

            FiniteDomain<String> roomType = new FiniteDomain<string>("start", "small-ammo", "big-ammo", "small-health-pack", "big-health-pack", "zombie", "two zombies", "dog", "trap", "boss", "end");

            FiniteDomainVariable<String>[] roomTypes = new FiniteDomainVariable<string>[41];
            roomTypes[0] = solver.CreateFiniteDomainVariable(roomType, "start");

            for (int i = 1; i <= 13; i++)
            {
                roomTypes[i] = solver.CreateFiniteDomainVariable(roomType, "small-ammo", "big-ammo", "small-health-pack", "big-health-pack", "zombie", "two zombies", "dog", "trap", "boss");
            }

            roomTypes[14] = solver.CreateFiniteDomainVariable(roomType, "end");

            for (int i = 15; i <= 40; i++)
            {
                roomTypes[i] = solver.CreateFiniteDomainVariable(roomType, "small-ammo", "big-ammo", "small-health-pack", "big-health-pack", "zombie", "two zombies", "dog", "trap", "boss");
            }

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

            Constraint.NotEqual(roomTypes[2], roomTypes[15]);
            Constraint.NotEqual(roomTypes[15], roomTypes[16]);
            Constraint.NotEqual(roomTypes[16], roomTypes[3]);
            Constraint.NotEqual(roomTypes[16], roomTypes[4]);
            Constraint.NotEqual(roomTypes[16], roomTypes[17]);
            Constraint.NotEqual(roomTypes[15], roomTypes[18]);
            Constraint.NotEqual(roomTypes[17], roomTypes[18]);
            Constraint.NotEqual(roomTypes[18], roomTypes[4]);

            Constraint.NotEqual(roomTypes[2], roomTypes[23]);
            Constraint.NotEqual(roomTypes[5], roomTypes[23]);
            Constraint.NotEqual(roomTypes[23], roomTypes[7]);
            Constraint.NotEqual(roomTypes[23], roomTypes[4]);

            Constraint.NotEqual(roomTypes[1], roomTypes[24]);
            Constraint.NotEqual(roomTypes[24], roomTypes[25]);
            Constraint.NotEqual(roomTypes[25], roomTypes[6]);
            Constraint.NotEqual(roomTypes[25], roomTypes[26]);
            Constraint.NotEqual(roomTypes[6], roomTypes[26]);
            Constraint.NotEqual(roomTypes[26], roomTypes[27]);
            Constraint.NotEqual(roomTypes[27], roomTypes[7]);

            Constraint.NotEqual(roomTypes[8], roomTypes[33]);
            Constraint.NotEqual(roomTypes[8], roomTypes[36]);
            Constraint.NotEqual(roomTypes[33], roomTypes[36]);
            Constraint.NotEqual(roomTypes[33], roomTypes[40]);
            Constraint.NotEqual(roomTypes[33], roomTypes[34]);
            Constraint.NotEqual(roomTypes[33], roomTypes[35]);
            Constraint.NotEqual(roomTypes[34], roomTypes[35]);
            Constraint.NotEqual(roomTypes[35], roomTypes[9]);
            Constraint.NotEqual(roomTypes[36], roomTypes[40]);
            Constraint.NotEqual(roomTypes[36], roomTypes[37]);
            Constraint.NotEqual(roomTypes[37], roomTypes[40]);
            Constraint.NotEqual(roomTypes[37], roomTypes[39]);
            Constraint.NotEqual(roomTypes[37], roomTypes[38]);
            Constraint.NotEqual(roomTypes[40], roomTypes[39]);
            Constraint.NotEqual(roomTypes[38], roomTypes[39]);
            Constraint.NotEqual(roomTypes[39], roomTypes[9]);


            Constraint.NotEqual(roomTypes[10], roomTypes[19]);
            Constraint.NotEqual(roomTypes[19], roomTypes[20]);
            Constraint.NotEqual(roomTypes[20], roomTypes[21]);
            Constraint.NotEqual(roomTypes[21], roomTypes[22]);
            Constraint.NotEqual(roomTypes[22], roomTypes[13]);
            Constraint.NotEqual(roomTypes[11], roomTypes[20]);

            Constraint.NotEqual(roomTypes[10], roomTypes[13]);

            Constraint.NotEqual(roomTypes[10], roomTypes[28]);
            Constraint.NotEqual(roomTypes[28], roomTypes[29]);
            Constraint.NotEqual(roomTypes[12], roomTypes[29]);
            Constraint.NotEqual(roomTypes[29], roomTypes[30]);
            Constraint.NotEqual(roomTypes[12], roomTypes[30]);
            Constraint.NotEqual(roomTypes[30], roomTypes[31]);
            Constraint.NotEqual(roomTypes[12], roomTypes[31]);
            Constraint.NotEqual(roomTypes[31], roomTypes[32]);
            Constraint.NotEqual(roomTypes[32], roomTypes[13]);

            ScoreMapping<String> damages = new ScoreMapping<String>(roomType,
                "small-health-pack".PairedWith(new Interval(35)),
                "big-health-pack".PairedWith(new Interval(70)),
                "small-ammo".PairedWith(new Interval(0)),
                "big-ammo".PairedWith(new Interval(0)),
                "start".PairedWith(new Interval(0)),
                "zombie".PairedWith(new Interval(-6)),
                "two zombies".PairedWith(new Interval(-15)),
                "dog".PairedWith(new Interval(-8)),
                "trap".PairedWith(new Interval(-5)),
                "boss".PairedWith(new Interval(-30)),
                "end".PairedWith(new Interval(0)));

            // create variables to contain how much damage is dealt in each room

            RealVariable[] roomDelta = new RealVariable[41];

            roomDelta[0] = solver.CreateRealVariable(0);
            roomDelta[0].RequireUnique = false;

            for (int i = 1; i <= 13; i++)
            {
                roomDelta[i] = Constraint.ScoreVariable(roomTypes[i], damages);
                roomDelta[i].RequireUnique = false;
            }

            roomDelta[14] = solver.CreateRealVariable(0);
            roomDelta[14].RequireUnique = false;

            for (int i = 15; i <= 40; i++)
            {
                roomDelta[i] = Constraint.ScoreVariable(roomTypes[i], damages);
                roomDelta[i].RequireUnique = false;
            }

            // variables for how much health the player has in each room
            // under the assumptions of forward progress in the paper

            RealVariable[] playerHealth = new RealVariable[41];

            // start at 100
            playerHealth[0] = solver.CreateRealVariable(100);
            playerHealth[0].RequireUnique = false;

            // player must always have above 0 health
            for (int i = 1; i <= 40; i++)
            {
                playerHealth[i] = solver.CreateRealVariable(0, 10000);
                playerHealth[i].RequireUnique = false;
            }

            Constraint.Sum(playerHealth[1], roomDelta[1], playerHealth[0]);
            Constraint.Sum(playerHealth[2], roomDelta[2], playerHealth[1]);
            Constraint.Sum(playerHealth[3], roomDelta[3], Constraint.Minimize(playerHealth[2], playerHealth[16]).With(r => r.RequireUnique = false));
            Constraint.Sum(playerHealth[4], roomDelta[4], Constraint.Minimize(playerHealth[3], playerHealth[23], playerHealth[16], playerHealth[18]).With(r => r.RequireUnique = false));

            Constraint.Sum(playerHealth[5], roomDelta[5], playerHealth[1]);
            Constraint.Sum(playerHealth[6], roomDelta[6], Constraint.Minimize(playerHealth[5], playerHealth[25]).With(r => r.RequireUnique = false));
            Constraint.Sum(playerHealth[7], roomDelta[7], Constraint.Minimize(playerHealth[6], playerHealth[27]).With(r => r.RequireUnique = false));

            Constraint.Sum(playerHealth[8], roomDelta[8], Constraint.Minimize(playerHealth[4], playerHealth[7]).With(r => r.RequireUnique = false));

            Constraint.Sum(playerHealth[9], roomDelta[9], Constraint.Minimize(playerHealth[8], playerHealth[35], playerHealth[39]).With(r => r.RequireUnique = false));
            Constraint.Sum(playerHealth[10], roomDelta[10], playerHealth[9]);

            Constraint.Sum(playerHealth[11], roomDelta[11], playerHealth[10]);
            Constraint.Sum(playerHealth[12], roomDelta[12], playerHealth[10]);

            Constraint.Sum(playerHealth[13], roomDelta[13], Constraint.Minimize(playerHealth[11], playerHealth[12], playerHealth[10], playerHealth[22], playerHealth[32]).With(r => r.RequireUnique = false));

            Constraint.Sum(playerHealth[14], roomDelta[14], playerHealth[13]);

            Constraint.Sum(playerHealth[15], roomDelta[15], playerHealth[2]);
            Constraint.Sum(playerHealth[16], roomDelta[16], playerHealth[15]);
            Constraint.Sum(playerHealth[17], roomDelta[17], playerHealth[16]);
            Constraint.Sum(playerHealth[18], roomDelta[18], Constraint.Minimize(playerHealth[15], playerHealth[17]).With(r => r.RequireUnique = false));
            Constraint.Sum(playerHealth[19], roomDelta[19], playerHealth[10]);
            Constraint.Sum(playerHealth[20], roomDelta[20], Constraint.Minimize(playerHealth[19], playerHealth[11]).With(r => r.RequireUnique = false));
            Constraint.Sum(playerHealth[21], roomDelta[21], playerHealth[11]);
            Constraint.Sum(playerHealth[22], roomDelta[22], playerHealth[21]);

            Constraint.Sum(playerHealth[23], roomDelta[23], Constraint.Minimize(playerHealth[2], playerHealth[5]).With(r => r.RequireUnique = false));

            Constraint.Sum(playerHealth[24], roomDelta[24], playerHealth[1]);
            Constraint.Sum(playerHealth[25], roomDelta[25], playerHealth[24]);
            Constraint.Sum(playerHealth[26], roomDelta[26], Constraint.Minimize(playerHealth[25], playerHealth[6]).With(r => r.RequireUnique = false));
            Constraint.Sum(playerHealth[27], roomDelta[27], playerHealth[26]);

            Constraint.Sum(playerHealth[28], roomDelta[28], playerHealth[10]);
            Constraint.Sum(playerHealth[29], roomDelta[29], Constraint.Minimize(playerHealth[28], playerHealth[12]).With(r => r.RequireUnique = false));
            Constraint.Sum(playerHealth[30], roomDelta[30], Constraint.Minimize(playerHealth[29], playerHealth[12]).With(r => r.RequireUnique = false));
            Constraint.Sum(playerHealth[31], roomDelta[31], Constraint.Minimize(playerHealth[30], playerHealth[12]).With(r => r.RequireUnique = false));
            Constraint.Sum(playerHealth[32], roomDelta[32], playerHealth[31]);

            Constraint.Sum(playerHealth[33], roomDelta[33], playerHealth[8]);
            Constraint.Sum(playerHealth[34], roomDelta[34], playerHealth[33]);
            Constraint.Sum(playerHealth[35], roomDelta[35], Constraint.Minimize(playerHealth[33], playerHealth[34]).With(r => r.RequireUnique = false));

            Constraint.Sum(playerHealth[36], roomDelta[36], Constraint.Minimize(playerHealth[8], playerHealth[33]).With(r => r.RequireUnique = false));
            Constraint.Sum(playerHealth[37], roomDelta[37], playerHealth[36]);
            Constraint.Sum(playerHealth[38], roomDelta[38], playerHealth[37]);
            Constraint.Sum(playerHealth[39], roomDelta[39], Constraint.Minimize(playerHealth[40], playerHealth[37], playerHealth[38]).With(r => r.RequireUnique = false));
            Constraint.Sum(playerHealth[40], roomDelta[40], Constraint.Minimize(playerHealth[33], playerHealth[36], playerHealth[37]).With(r => r.RequireUnique = false));

            ScoreMapping<String> ammoChanges = new ScoreMapping<String>(roomType,
                "small-health-pack".PairedWith(new Interval(0)),
                "big-health-pack".PairedWith(new Interval(0)),
                "small-ammo".PairedWith(new Interval(6)),
                "big-ammo".PairedWith(new Interval(12)),
                "start".PairedWith(new Interval(0)),
                "zombie".PairedWith(new Interval(-1)),
                "two zombies".PairedWith(new Interval(-2)),
                "dog".PairedWith(new Interval(-2)),
                "trap".PairedWith(new Interval(0)),
                "boss".PairedWith(new Interval(-8)),
                "end".PairedWith(new Interval(0)));

            // create variables to contain how much damage is dealt in each room

            RealVariable[] ammoRoomDelta = new RealVariable[41];

            ammoRoomDelta[0] = solver.CreateRealVariable(0);
            ammoRoomDelta[0].RequireUnique = false;

            for (int i = 1; i <= 13; i++)
            {
                ammoRoomDelta[i] = Constraint.ScoreVariable(roomTypes[i], ammoChanges);
                ammoRoomDelta[i].RequireUnique = false;
            }

            ammoRoomDelta[14] = solver.CreateRealVariable(0);
            ammoRoomDelta[14].RequireUnique = false;

            for (int i = 15; i <= 40; i++)
            {
                ammoRoomDelta[i] = Constraint.ScoreVariable(roomTypes[i], ammoChanges);
                ammoRoomDelta[i].RequireUnique = false;
            }

            // variables for how much health the player has in each room
            // under the assumptions of forward progress in the paper

            RealVariable[] playerAmmo = new RealVariable[41];

            // start at 10
            playerAmmo[0] = solver.CreateRealVariable(10);
            playerAmmo[0].RequireUnique = false;

            // player must always have above 0 ammo, but can be unlimited
            for (int i = 1; i <= 40; i++)
            {
                playerAmmo[i] = solver.CreateRealVariable(0, 500000);
                playerAmmo[i].RequireUnique = false;
            }

            Constraint.Sum(playerAmmo[1], ammoRoomDelta[1], playerAmmo[0]);
            Constraint.Sum(playerAmmo[2], ammoRoomDelta[2], playerAmmo[1]);
            Constraint.Sum(playerAmmo[3], ammoRoomDelta[3], Constraint.Minimize(playerAmmo[2], playerAmmo[16]).With(r => r.RequireUnique = false));
            Constraint.Sum(playerAmmo[4], ammoRoomDelta[4], Constraint.Minimize(playerAmmo[3], playerAmmo[23], playerAmmo[16], playerAmmo[18]).With(r => r.RequireUnique = false));

            Constraint.Sum(playerAmmo[5], ammoRoomDelta[5], playerAmmo[1]);
            Constraint.Sum(playerAmmo[6], ammoRoomDelta[6], Constraint.Minimize(playerAmmo[5], playerAmmo[25]).With(r => r.RequireUnique = false));
            Constraint.Sum(playerAmmo[7], ammoRoomDelta[7], Constraint.Minimize(playerAmmo[6], playerAmmo[27]).With(r => r.RequireUnique = false));

            Constraint.Sum(playerAmmo[8], ammoRoomDelta[8], Constraint.Minimize(playerAmmo[4], playerAmmo[7]).With(r => r.RequireUnique = false));

            Constraint.Sum(playerAmmo[9], ammoRoomDelta[9], Constraint.Minimize(playerAmmo[8], playerAmmo[35], playerAmmo[39]).With(r => r.RequireUnique = false));
            Constraint.Sum(playerAmmo[10], ammoRoomDelta[10], playerAmmo[9]);

            Constraint.Sum(playerAmmo[11], ammoRoomDelta[11], playerAmmo[10]);
            Constraint.Sum(playerAmmo[12], ammoRoomDelta[12], playerAmmo[10]);

            Constraint.Sum(playerAmmo[13], ammoRoomDelta[13], Constraint.Minimize(playerAmmo[11], playerAmmo[12], playerAmmo[10], playerAmmo[22], playerAmmo[32]).With(r => r.RequireUnique = false));

            Constraint.Sum(playerAmmo[14], ammoRoomDelta[14], playerAmmo[13]);

            Constraint.Sum(playerAmmo[15], ammoRoomDelta[15], playerAmmo[2]);
            Constraint.Sum(playerAmmo[16], ammoRoomDelta[16], playerAmmo[15]);
            Constraint.Sum(playerAmmo[17], ammoRoomDelta[17], playerAmmo[16]);
            Constraint.Sum(playerAmmo[18], ammoRoomDelta[18], Constraint.Minimize(playerAmmo[15], playerAmmo[17]).With(r => r.RequireUnique = false));
            Constraint.Sum(playerAmmo[19], ammoRoomDelta[19], playerAmmo[10]);
            Constraint.Sum(playerAmmo[20], ammoRoomDelta[20], Constraint.Minimize(playerAmmo[19], playerAmmo[11]).With(r => r.RequireUnique = false));
            Constraint.Sum(playerAmmo[21], ammoRoomDelta[21], playerAmmo[11]);
            Constraint.Sum(playerAmmo[22], ammoRoomDelta[22], playerAmmo[21]);

            Constraint.Sum(playerAmmo[23], ammoRoomDelta[23], Constraint.Minimize(playerAmmo[2], playerAmmo[5]).With(r => r.RequireUnique = false));

            Constraint.Sum(playerAmmo[24], ammoRoomDelta[24], playerAmmo[1]);
            Constraint.Sum(playerAmmo[25], ammoRoomDelta[25], playerAmmo[24]);
            Constraint.Sum(playerAmmo[26], ammoRoomDelta[26], Constraint.Minimize(playerAmmo[25], playerAmmo[6]).With(r => r.RequireUnique = false));
            Constraint.Sum(playerAmmo[27], ammoRoomDelta[27], playerAmmo[26]);

            Constraint.Sum(playerAmmo[28], ammoRoomDelta[28], playerAmmo[10]);
            Constraint.Sum(playerAmmo[29], ammoRoomDelta[29], Constraint.Minimize(playerAmmo[28], playerAmmo[12]).With(r => r.RequireUnique = false));
            Constraint.Sum(playerAmmo[30], ammoRoomDelta[30], Constraint.Minimize(playerAmmo[29], playerAmmo[12]).With(r => r.RequireUnique = false));
            Constraint.Sum(playerAmmo[31], ammoRoomDelta[31], Constraint.Minimize(playerAmmo[30], playerAmmo[12]).With(r => r.RequireUnique = false));
            Constraint.Sum(playerAmmo[32], ammoRoomDelta[32], playerAmmo[31]);

            Constraint.Sum(playerAmmo[33], ammoRoomDelta[33], playerAmmo[8]);
            Constraint.Sum(playerAmmo[34], ammoRoomDelta[34], playerAmmo[33]);
            Constraint.Sum(playerAmmo[35], ammoRoomDelta[35], Constraint.Minimize(playerAmmo[33], playerAmmo[34]).With(r => r.RequireUnique = false));

            Constraint.Sum(playerAmmo[36], ammoRoomDelta[36], Constraint.Minimize(playerAmmo[8], playerAmmo[33]).With(r => r.RequireUnique = false));
            Constraint.Sum(playerAmmo[37], ammoRoomDelta[37], playerAmmo[36]);
            Constraint.Sum(playerAmmo[38], ammoRoomDelta[38], playerAmmo[37]);
            Constraint.Sum(playerAmmo[39], ammoRoomDelta[39], Constraint.Minimize(playerAmmo[40], playerAmmo[37], playerAmmo[38]).With(r => r.RequireUnique = false));
            Constraint.Sum(playerAmmo[40], ammoRoomDelta[40], Constraint.Minimize(playerAmmo[33], playerAmmo[36], playerAmmo[37]).With(r => r.RequireUnique = false));



            Constraint.LimitOccurences("trap", 2, 4, roomTypes);

            Constraint.MaximumOccurences("small-health-pack", 5, roomTypes);
            Constraint.MaximumOccurences("big-health-pack", 5, roomTypes);

            Constraint.MaximumOccurences("small-ammo", 5, roomTypes);
            Constraint.MaximumOccurences("big-ammo", 5, roomTypes);
            
            Constraint.RequireOccurences("boss", 1, roomTypes);
        }
    }
}