using System.Diagnostics;

namespace ConstraintThingy
{
    /// <summary>
    /// Represents one of the variables constrained by a constraint
    /// </summary>
    abstract class ConstraintArc
    {
        internal abstract void Update(ref bool succeeded);

        protected bool queuedForUpdate;

        public void UnmarkForUpdate()
        {
            queuedForUpdate = false;
        }
    }

    /// <summary>
    /// Represents one of the variables constrained by a constraint
    /// </summary>
    [DebuggerDisplay("{Variable.Name}")]
    class ConstraintArc<VType> : ConstraintArc where VType : Variable
    {
        public ConstraintArc(Constraint<VType> c, VType v)
        {
            Constraint = c;
            Variable = v;
        }
        public Constraint<VType> Constraint { get; private set; }
        public VType Variable { get; private set; }

        public void MarkForUpdate()
        {
            if (!queuedForUpdate)
            {
                queuedForUpdate = true;
                ConstraintThingy.Constraint.QueueForUpdate(this);
            }
        }

        internal override void Update(ref bool succeeded)
        {
            queuedForUpdate = false;
            Constraint.UpdateVariable(Variable, ref succeeded);
        }
    }
}
