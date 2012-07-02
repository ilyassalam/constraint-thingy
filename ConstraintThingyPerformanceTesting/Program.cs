using System;
using System.Diagnostics;
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
            new PathPlayability().SolveInitialForever();
        }
    }
}
