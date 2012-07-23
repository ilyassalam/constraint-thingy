using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace ConstraintThingy
{
    /// <summary>
    /// A variable whose value is of type <typeparamref name="T"/>
    /// </summary>
    public abstract class Variable<T> : Variable where T : IEquatable<T>
    {
        /// <summary>
        /// Creates a new variable
        /// </summary>
        internal Variable(ConstraintThingySolver constraintThingySolver, String name, T allowableValues) : base(constraintThingySolver, name)
        {
            AllowableValues = allowableValues;
        }
        

        /// <summary>
        /// Narrows the variable until it has a unique value.
        /// </summary>
        internal abstract IEnumerable<T> UniqueValues { get; } 

        /// <summary>
        /// The allowable values of a variable
        /// </summary>
        internal T AllowableValues { get; private set; }

        internal void TrySetAndResolveConstraints(T value, out bool success)
        {
            TrySetValue(value, out success);

            if (!success) return;

            ConstraintThingySolver.ResolveConstraints(out success);
        }

        internal void TrySetValue(T value, out bool success)
        {
            // this resolves to IEquatable, which is much faster than the regular Equals(obj)
            if (!AllowableValues.Equals(value))
            {
                if (LastSaveFramePointer != ConstraintThingySolver.CurrentFramePointer)
                {
                    SaveValue();   
                }

                AllowableValues = value;

                NarrowConstraints(out success);

                if (!success) 
                    return;
            }

            if (IsEmpty)
            {
                success = false;
                return;
            }

            success = true;
            return;
        }

        protected void NarrowConstraints(out bool success)
        {
            foreach (var constraint in Constraints)
            {
                constraint.Narrowed(this, out success);

                // bow out early if there's a failure
                if (!success) return;
            }

            success = true;
            return;
        }

        /// <summary>
        /// Saves this variable's current value in the solver's undo stack
        /// </summary>
        protected void SaveValue()
        {
            ConstraintThingySolver.SaveValue(this, AllowableValues, LastSaveFramePointer);

            LastSaveFramePointer = ConstraintThingySolver.CurrentFramePointer;
        }

        internal void BackdoorSet(T value)
        {
            AllowableValues = value;
        }

        internal override void BackdoorSet(object value)
        {
            BackdoorSet((T) value);
        }

        internal sealed override object BoxedValue
        {
            get { return AllowableValues; }
        }

        internal override IEnumerable<object> BoxedUniqueValues
        {
            get
            {
                foreach (T uniqueValue in UniqueValues)
                    yield return uniqueValue;
            }
        }
    }

    /// <summary>
    /// Base class for variables that participate in constraints
    /// </summary>
    public abstract class Variable : ConstraintComponent
    {
        /// <summary>
        /// True if the variable is required to be narrowed to a unique value
        /// </summary>
        public bool RequireUnique { get; set; }

        /// <summary>
        /// Helps guide the order of expansion
        /// </summary>
        public int Priority { get; set; }

        /// <summary>
        /// The name of the variable
        /// </summary>
        public String Name { get; internal set; }

        /// <summary>
        /// Creates a new variable
        /// </summary>
        internal Variable(ConstraintThingySolver constraintThingySolver, String name) : base(constraintThingySolver)
        {
            RequireUnique = true;

            Name = name;

            if (constraintThingySolver != null) constraintThingySolver.AddVariable(this);
        }

        /// <summary>
        /// True if the variable has been narrowed to a single, unique value.
        /// </summary>
        public abstract bool IsUnique { get; }

        /// <summary>
        /// True if the variable has been narrowed to the empty set.
        /// </summary>
        public abstract bool IsEmpty { get; }


        internal void AddConstraint(Constraint constraint)
        {
            Constraints.Add(constraint);
        }

        // used to auto save variables when changed
        internal int LastSaveFramePointer = -1;

        internal abstract void BackdoorSet(object value);

        /// <summary>
        /// The list of constraints this variable is involved in
        /// </summary>
        protected internal readonly List<Constraint> Constraints = new List<Constraint>();

        internal abstract object BoxedValue { get; }

        internal abstract IEnumerable<object> BoxedUniqueValues { get; }

        /// <summary>
        /// The variable's name and value
        /// </summary>
        public override string ToString()
        {
            return String.Format("{0}:{1}", Name, BoxedValue);
        }
    }
}