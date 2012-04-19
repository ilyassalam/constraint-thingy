using System;
using System.Collections.Generic;

namespace ConstraintThingy
{
    /// <summary>
    /// Base class for variables that participate in constraints.
    /// </summary>
    public abstract class Variable
    {
        static readonly List<Variable> AllVariables = new List<Variable>(); 

        /// <summary>
        /// Base initializer for variables; just sets the debugging name.
        /// </summary>
        protected Variable(string name)
        {
            Name = name;
            lastSaveFramePointer = -1;
            AllVariables.Add(this);
        }

        /// <summary>
        /// Name of variable for purposes of debugging
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// True if the variable has been narrowed to a single, unique value.
        /// </summary>
        public virtual bool IsUnique
        {
            get { return true; }
        }

        /// <summary>
        /// True if variable has been narrowed to the empty set.
        /// </summary>
        public virtual bool IsEmpty
        {
            get { return false;  }
        }

        #region Solution finding
        /// <summary>
        /// Narrows the variable until it has a unique value, or throws failure if there's no possible consistent value.
        /// </summary>
        public virtual IEnumerable<bool> UniqueValues()
        {
            throw new NotImplementedException("Class does not define a method for narrowing to a unique value.");
        }

        /// <summary>
        /// Find sets of unique values that satisfy all constraints for all variables.
        /// </summary>
        public static IEnumerable<bool> Solutions()
        {
            return Solutions(AllVariables.ToArray());
        }

        /// <summary>
        /// Find sets of unique values for VARS that satisfy all constraints.
        /// </summary>
        public static IEnumerable<bool> Solutions(params Variable[] vars)
        {
            //
            // This is super-painful becausee C# doesn't allow recursion in iterators.
            // Understandable, but it means we have to manually implement a recursion stack.
            //

            if (vars.Length == 0)
                yield break;
            // Allocate stack of enumerators
            var enumerators = new IEnumerator<bool>[vars.Length];
            int tos = 0;
            enumerators[tos] = vars[tos].UniqueValues().GetEnumerator();
            while (true)
            {
                // Find the next solution.
                while (true)
                {
                    // Try to advance the variable at the top of stack
                    if (enumerators[tos].MoveNext())
                    {
                        // It worked.
                        if (tos == enumerators.Length - 1)
                            // This was the last variable, so we've found a solution.
                            break;
                        else
                        {
                            // Need to "recurse"
                            tos++;
                            enumerators[tos] = vars[tos].UniqueValues().GetEnumerator();
                        }
                    }
                    else
                    {
                        // MoveNext failed.  Pop the stack.
                        tos--;
                        if (tos < 0)
                            // Popped off the top of the stack: we're done.
                            yield break;
                    }
                }
                yield return false;
            }
        }
        #endregion

        #region Undostack management
        /// <summary>
        /// Holds a stack of variable's that have been modified.
        /// To prevent boxing of variable values, the actual saved values are stored
        /// in separate stacks, one per type (see Variable(T).TypedUndoStack).
        /// </summary>
        protected static readonly Stack<Variable> UndoStack = new Stack<Variable>();
        /// <summary>
        /// Holds the saved frame pointers of spilled variables
        /// </summary>
        protected static readonly Stack<int> FrameStack = new Stack<int>();

        /// <summary>
        /// List of all constraints the variable bound by
        /// </summary>
        protected readonly List<Constraint> constraints = new List<Constraint>();

        /// <summary>
        /// Mark that this variable is constrained by the specified constraint.
        /// </summary>
        public void AddConstraint(Constraint c)
        {
            constraints.Add(c);
        }

        /// <summary>
        /// Total number of values that have been saved tot the undo stack
        /// </summary>
        public static int StackDepth
        {
            get { return UndoStack.Count; }
        }

        /// <summary>
        /// Holds the stack address of the last frame within which this variable has been saved.
        /// </summary>
        protected int lastSaveFramePointer;

        /// <summary>
        /// Stack address of the current frame.
        /// </summary>
        protected static int currentFramePointer = -1;

        /// <summary>
        /// Restores variable's value from the top of its TypedUndoStack.
        /// </summary>
        protected abstract void Restore();

        /// <summary>
        /// Marks a location on the UndoStack so that all subsequent variable assignments can be undone using RestoreValues(framepointer).
        /// </summary>
        /// <returns>Stack address of new frame</returns>
        public static int SaveValues()
        {
            currentFramePointer = UndoStack.Count;
            return currentFramePointer;
        }

        /// <summary>
        /// Undoes all assignments to all variables since the specified mark on the undostack.
        /// </summary>
        /// <param name="framePointer">Base address of the previous frame.</param>
        public static void RestoreValues(int framePointer)
        {
            for (int c = StackDepth; c > framePointer; c--)
                UndoStack.Pop().Restore();
            currentFramePointer = framePointer;
            Constraint.ClearWorklist();
        }

        /// <summary>
        /// Used only in the test rig.
        /// </summary>
        public static void ResetVariableSystemForTesting()
        {
            UndoStack.Clear();
            currentFramePointer = -1;
        }
        #endregion
    }

    /// <summary>
    /// A variable whose value is of type T.
    /// </summary>
    /// <typeparam name="T">Datatype for value of variable</typeparam>
    public class Variable<T> : Variable
    {
        /// <summary>
        /// Creates a new variable
        /// </summary>
        public Variable(string name, T initialValue) : base(name)
        {
            mValue = initialValue;
        }

        #region Value tracking
        /// <summary>
        /// 
        /// </summary>
        public T Value
        {
            get { return mValue; }
            set
            {
                if (!EqualityComparer<T>.Default.Equals(mValue, value))
                {
                    if (lastSaveFramePointer != currentFramePointer)
                        SaveValue();
                    mValue = value;
                    foreach (var c in constraints)
                        c.Narrowed(this);
                    Constraint.ResolveCurrentConstraints();
                }
            }
        }
        private T mValue;
        #endregion
        
        #region Undostack management
        private void SaveValue()
        {
            UndoStack.Push(this);
            FrameStack.Push(lastSaveFramePointer);
            TypedUndoStack.Push(mValue);
            lastSaveFramePointer = currentFramePointer;
        }

        /// <summary>
        /// Holds the saved values of variables to be undone.
        /// </summary>
        static readonly Stack<T> TypedUndoStack = new Stack<T>();

        /// <summary>
        /// Restores the variable's value from the TypedUndoStack.
        /// </summary>
        protected override void Restore()
        {
            mValue = TypedUndoStack.Pop();
            lastSaveFramePointer = FrameStack.Pop();
        }
        #endregion
    }
}
