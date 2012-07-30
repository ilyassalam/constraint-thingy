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
            // solve it a couple times to try and JIT everything
            for (int i = 0; i < 5; i++) new AnActualLevelBig().SolveInitial();

            int numSolutions = 0;
            while(true)
            {
                Solution solution = new AnActualLevelBig().SolveInitial();

                File.AppendAllText("data", solution.SolveTime.TotalMilliseconds + "\n");

                numSolutions++;
                Console.WriteLine(numSolutions);
            }
        }
    }
}
