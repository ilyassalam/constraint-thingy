using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using ConstraintThingy;

namespace ConstraintThingyPerformanceTesting
{
    class Program
    {

        /*
         * Subclass the "PerformanceTest" class and implement the method to initialize a system of constraints
         * 
         * Then, instantiate the class and call the appropriate method based on what you want to test
         * 
         */

        static void Main(string[] args)
        {
            List<TimeSpan> solveTimes = new List<TimeSpan>();

            // solve it a couple times to try and JIT everything
            for (int i = 0; i < 10; i++) new AnActualLevel().SolveInitial();

            for (int i = 0; i < 1000; i++)
            {
                Solution solution = new AnActualLevel().SolveInitial();
                solveTimes.Add(solution.SolveTime);
            }

            StringBuilder data = new StringBuilder();

            foreach (var solveTime in solveTimes)
            {
                data.AppendLine(solveTime.TotalMilliseconds.ToString());
            }
            
            File.WriteAllText("data", data.ToString());

            Console.ReadLine();
        }
    }
}
