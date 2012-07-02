using System;
using System.Diagnostics.Contracts;

namespace ConstraintThingy
{
    /// <summary>
    /// A mapping between variables and constraints
    /// </summary>
    abstract class ConstraintArc : ConstraintComponent
    {
        protected ConstraintArc(ConstraintThingySolver constraintThingySolver)
            : base(constraintThingySolver)
        {
        }

        internal abstract void Update(out bool success);

        internal bool QueuedForUpdate = false;

        internal void MarkForUpdate()
        {
            if (!QueuedForUpdate)
            {
                ConstraintThingySolver.QueueForUpdate(this);
            }
        }
    }

    /// <summary>
    /// A mapping between variables and constraints
    /// </summary>
    sealed class ConstraintArc<T> : ConstraintArc where T : Variable
    {
        public ConstraintArc(ConstraintThingySolver constraintThingySolver, Constraint<T> constraint, T variable) : base(constraintThingySolver)
        {
            _constraint = constraint;
            Variable = variable;
        }

        private readonly Constraint<T> _constraint;

        internal T Variable { get; private set; }

        internal override void Update(out bool success)
        {
            _constraint.UpdateVariable(Variable, out success);
        }

        public override string ToString()
        {
            return String.Format("Variable: {0}, Constraint: {1}", Variable, _constraint);
        }
    }
}