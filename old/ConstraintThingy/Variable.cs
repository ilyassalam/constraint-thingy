using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ConstraintThingy
{
    /// <summary>
    /// Base class for variables that participate in constraints.
    /// </summary>
    public abstract class Variable
    {
        static readonly List<Variable> AllVariables = new List<Variable>();
        /// <summary>
        /// Total number of variables created.
        /// Used only for performance measurement.
        /// </summary>
        public static int TotalVariables { get; private set; }
        /// <summary>
        /// Maximum depth the undostack has reached.
        /// Used only for performance measurement.
        /// </summary>
        public static int MaxUndoStackDepth { get; private set; }
        /// <summary>
        /// Total number of times the system has backtracked.
        /// Used only for performance measurement.
        /// </summary>
        public static int TotalBacktracks { get; private set; }
        /// <summary>
        /// Resets the performance statistics.
        /// </summary>
        public static void ResetStatistics()
        {
            TotalBacktracks = TotalVariables = MaxUndoStackDepth = 0;
        }

        private static int RemainingBacktracks { get; set; }


        /// <summary>
        /// Base initializer for variables; just sets the debugging name.
        /// </summary>
        protected Variable(string name)
        {
            TotalVariables++;
            Name = name;
            lastSaveFramePointer = -1;
            if (this is FiniteDomainVariable)
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
        public static IEnumerable<bool> SolutionsAllVariables()
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

            Debug.Assert(StackDepth == 0);
            RestoreValues(0);
            if (vars.Length == 0)
                yield break;
            // Allocate stack of enumerators
            var enumerators = new IEnumerator<bool>[vars.Length];
        restart:
            RemainingBacktracks = 10;
            int tos = 0;
            enumerators[tos] = vars[tos].UniqueValues().GetEnumerator();
            while (true)
            {
                // Find the next solution.
                while (true)
                {
                    if (RemainingBacktracks < 0)
                    {
                        RestoreValues(0);
                        goto restart;
                    }
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
            MaxUndoStackDepth = Math.Max(MaxUndoStackDepth, UndoStack.Count);
            TotalBacktracks++;
            RemainingBacktracks--;
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
            AllVariables.Clear();
        }
        #endregion
    }

    /// <summary>
    /// A variable whose value is of type T.
    /// </summary>
    /// <typeparam name="T">Datatype for value of variable</typeparam>
    [DebuggerDisplay("{Name}={Value}")]
    public class Variable<T> : Variable
    {
        /// <summary>
        /// Creates a new variable
        /// </summary>
        public Variable(string name, T allowableRange) : base(name)
        {
            mValue = allowableRange;
        }

        #region Value tracking
        /// <summary>
        /// 
        /// </summary>
        public T Value
        {
            get { return mValue; }
        }

        /// <summary>
        /// Value of variable before last update.
        /// Only valid immediately after a call to TrySetValue.  Will not be restored when the undo stack is popped.
        /// </summary>
        public T PreviousValue
        {
            get { return mPreviousValue; }
        }
        private T mValue;
        private T mPreviousValue;

        /// <summary>
        /// Attempts to narrow the variables value to VALUE.
        /// </summary>
        public void TrySetValue(T value, ref bool succeeded)
        {
            if (!EqualityComparer<T>.Default.Equals(mValue, value))
            {
                if (lastSaveFramePointer != currentFramePointer)
                    SaveValue();
                mPreviousValue = mValue;
                mValue = value;
                foreach (var c in constraints)
                {
                    c.Narrowed(this, ref succeeded);
                    if (!succeeded)
                        return;
                }
                Constraint.ResolveCurrentConstraints(ref succeeded);
            }
        }

        /// <summary>
        /// Attempts to set variable to the specified value.  Throws an exception if it fails.
        /// Intended to be used for initialization only.
        /// </summary>
        public void SetValueOrThrowException(T value, string failureMessage)
        {
            bool succeeded = true;
            TrySetValue(value, ref succeeded);
            if (!succeeded)
                throw new Exception(failureMessage??"Setting variable failed.");
        }
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
