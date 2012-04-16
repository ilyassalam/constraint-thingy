using System.Collections.Generic;

namespace ConstraintThingy
{
    /// <summary>
    /// Represents a constraint, i.e. a relation that has to hold between a set of variables.
    /// </summary>
    public abstract class Constraint
    {
        static readonly Queue<ConstraintArc> worklist = new Queue<ConstraintArc>();

        internal static void ClearWorklist()
        {
            worklist.Clear();
        }

        internal static void QueueForUpdate(ConstraintArc constraintArc)
        {
            worklist.Enqueue(constraintArc);
        }

        /// <summary>
        /// Update variables that could potentially be narrowed by narrowings that have happened so far.
        /// </summary>
        public static void ResolveCurrentConstraints()
        {
            while (worklist.Count > 0)
                worklist.Dequeue().Update();
        }

        /// <summary>
        /// Called when one of the variables participating in the constraint is narrowed.
        /// </summary>
        public abstract void Narrowed(Variable narrowedVariable);
    }

    /// <summary>
    /// Represents a constraint, i.e. a relation that has to hold between a set of variables.
    /// </summary>
    public abstract class Constraint<VType> : Constraint where VType : Variable
    {
        /// <summary>
        /// Represents a constraint, i.e. a relation that has to hold between a set of variables.
        /// </summary>
        protected Constraint(VType[] vars)
        {
            Variables = vars;
            arcs = new ConstraintArc<VType>[vars.Length];
            for (int i = 0; i < arcs.Length; i++)
            {
                arcs[i] = new ConstraintArc<VType>(this, vars[i]);
                vars[i].AddConstraint(this);
            }
        }

        /// <summary>
        /// Variables constrained by the constraint.
        /// </summary>
        public VType[] Variables { get; private set; }

        private readonly ConstraintArc<VType>[] arcs;

        /// <summary>
        /// Called when one of the variables participating in the constraint is narrowed.
        /// </summary>
        public override void Narrowed(Variable narrowedVariable)
        {
            foreach (var a in arcs)
                if (a.Variable != narrowedVariable)
                    a.MarkForUpdate();
        }

        /// <summary>
        /// Tries to narrow the variable based on the constraint.  Returns false if variable cannot be narrowed.
        /// </summary>
        public abstract void UpdateVariable(VType var);
    }
}
