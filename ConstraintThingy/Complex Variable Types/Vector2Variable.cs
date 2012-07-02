using System.Collections.Generic;

namespace ConstraintThingy
{
    /// <summary>
    /// Represents a pair of real-valued variables
    /// </summary>
    public class Vector2Variable
    {
        /// <summary>
        /// Creates a new Vector2 variable
        /// </summary>
        public Vector2Variable(ConstraintThingySolver constraintThingySolver)
        {
            X = constraintThingySolver.CreateRealVariable();
            Y = constraintThingySolver.CreateRealVariable();
        }

        /// <summary>
        /// Creates a new Vector2 variable
        /// </summary>
        public Vector2Variable(RealVariable x, RealVariable y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// The X-coordinate of the vector
        /// </summary>
        public readonly RealVariable X;

        /// <summary>
        /// The Y-coordinate of the vector
        /// </summary>
        public readonly RealVariable Y;
    }
}