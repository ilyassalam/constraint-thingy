using System;

namespace ConstraintThingy
{
    /// <summary>
    /// An abstract class of all components that have a reference to the constraint solver.
    /// </summary>
    public abstract class ConstraintComponent
    {
        /// <summary>
        /// Creates a new constraint component
        /// </summary>
        protected ConstraintComponent(ConstraintThingySolver constraintThingySolver)
        {
            ConstraintThingySolver = constraintThingySolver;
        }

        /// <summary>
        /// The constraint solver this component is associated with.
        /// </summary>
        internal ConstraintThingySolver ConstraintThingySolver { get; private set; }
    }
}