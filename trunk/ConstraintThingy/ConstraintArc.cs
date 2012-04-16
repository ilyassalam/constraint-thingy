namespace ConstraintThingy
{
    /// <summary>
    /// Represents one of the variables constrained by a constraint
    /// </summary>
    abstract class ConstraintArc
    {
        internal abstract void Update();
    }

    /// <summary>
    /// Represents one of the variables constrained by a constraint
    /// </summary>
    class ConstraintArc<VType> : ConstraintArc where VType : Variable
    {
        public ConstraintArc(Constraint<VType> c, VType v)
        {
            Constraint = c;
            Variable = v;
        }
        public Constraint<VType> Constraint { get; private set; }
        public VType Variable { get; private set; }
        private bool queuedForUpdate;

        public void MarkForUpdate()
        {
            if (!queuedForUpdate)
            {
                queuedForUpdate = true;
                ConstraintThingy.Constraint.QueueForUpdate(this);
            }
        }

        internal override void Update()
        {
            queuedForUpdate = false;
            Constraint.UpdateVariable(Variable);
        }
    }
}
