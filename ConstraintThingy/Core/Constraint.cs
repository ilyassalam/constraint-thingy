using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace ConstraintThingy
{
    /// <summary>
    /// Represents a constraint between variables
    /// </summary>
    public abstract partial class Constraint : ConstraintComponent
    {
        internal Constraint(ConstraintThingySolver constraintThingySolver)
            : base(constraintThingySolver)
        {
            constraintThingySolver.AddConstraint(this);
        }

        internal abstract void Narrowed(Variable narrowedVariable, out bool success);

        internal abstract void QueueAllArcs();

        internal abstract IEnumerable<ConstraintArc> Arcs { get; }
    }

    /// <summary>
    /// Represents a constraint between variables
    /// </summary>
    /// <typeparam name="T">The type of variable this constraint applies to</typeparam>
    public abstract class Constraint<T> : Constraint where T : Variable
    {
        private readonly T[] _variables;
        private readonly ConstraintArc<T>[] _arcs;

        internal override IEnumerable<ConstraintArc> Arcs
        {
            get { return _arcs; }
        }

        protected T[] Variables { get { return _variables; } }

        internal Constraint(params T[] variables) : base(variables[0].ConstraintThingySolver)
        {
            AssertVariablesHaveSameConstraintSystem(variables);

            _variables = new T[variables.Length];
            Array.Copy(variables, _variables, variables.Length);

            _arcs = new ConstraintArc<T>[_variables.Length];

            for(int i = 0; i < _arcs.Length; i++)
            {
                _arcs[i] = new ConstraintArc<T>(ConstraintThingySolver, this, _variables[i]);
                _variables[i].AddConstraint(this);
            }
        }

        private void AssertVariablesHaveSameConstraintSystem(T[] variables)
        {
            for (int i = 0; i < variables.Length; i++)
            {
                if (ConstraintThingySolver != variables[i].ConstraintThingySolver)
                {
                    throw new InvalidOperationException("The constraint systems of all involved variables must match");
                }
            }
        }

        internal override void Narrowed(Variable narrowedVariable, out bool success)
        {
            // default behavior simply marks each one of the variables 
            // involved in this constraint for an update.
            foreach (var arc in _arcs)
            {
                if (arc.Variable.IsEmpty)
                {
                    success = false;
                    return;
                }

                if (arc.Variable != narrowedVariable)
                {
                    arc.MarkForUpdate();
                }
            }

            success = true;
            return;
        }

        protected internal abstract void UpdateVariable(T variable, out bool success);

        internal override void QueueAllArcs()
        {
            foreach (var constraintArc in _arcs)
            {
                constraintArc.MarkForUpdate();
            }
        }

        public override string ToString()
        {
            return String.Format("{0}, Variables: {1}", GetType().Name, Variables.Select(v => v.ToString()).Aggregate((a, b) => a + " " + b));
        }
    }
}