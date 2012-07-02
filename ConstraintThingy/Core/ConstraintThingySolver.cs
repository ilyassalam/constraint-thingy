using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using CSharpUtils;
using CSharpUtils.Collections;

namespace ConstraintThingy
{
    /// <summary>
    /// Represents a constraint solver
    /// </summary>
    public class ConstraintThingySolver
    {
        internal readonly Random Random;

        /// <summary>
        /// Creates a new constraint solver
        /// </summary>
        public ConstraintThingySolver() : this(
 #if DEBUG
            new Random(1)
#else
            new Random()
#endif     
            )
        {
            // in debug mode, use the same random seed

        }

        /// <summary>
        /// Creates a new constraint solver with the specified random seed
        /// </summary>
        public ConstraintThingySolver(int randomSeed) : this(new Random(randomSeed))
        {
            
        }

        private ConstraintThingySolver(Random random)
        {
            Random = random;

            ExpansionOrder = ExpansionOrder.Random;

            Solutions = new SolutionSet(SolutionsEnumerable);
        }

        private readonly Queue<ConstraintArc> _workList = new Queue<ConstraintArc>(); 

        // 3 stacks which store the state of variables
        private readonly Stack<Variable> _variableStack = new Stack<Variable>();
        private readonly Stack<object> _valueStack = new Stack<object>();
        private readonly Stack<int> _frameStack = new Stack<int>();

        internal int StackDepth { get { return _variableStack.Count; } }

        internal int CurrentFramePointer { get; private set; }

        /// <summary>
        /// Determines the order in which possible options are explored.
        /// </summary>
        public ExpansionOrder ExpansionOrder { get; set; }

        internal void ResolveConstraints(out bool success)
        {
            while (_workList.Count > 0)
            {
                var arc = _workList.Dequeue();
                arc.QueuedForUpdate = false;

                arc.Update(out success);

                if (!success)
                {
                    while (_workList.Count > 0)
                    {
                        arc = _workList.Dequeue();
                        arc.QueuedForUpdate = false;
                    }

                    return;
                }
            }

            success = true;
            return;
        }

        internal void SaveValue(Variable variable, object value, int frame)
        {
            _variableStack.Push(variable);
            _valueStack.Push(value);
            _frameStack.Push(frame);
        }

        internal int SaveValues()
        {
            return CurrentFramePointer = StackDepth;
        }

        internal void Restore(int framePointer)
        {
            for (int i = StackDepth; i > framePointer; i--)
            {
                var variable = _variableStack.Pop();
                var value = _valueStack.Pop();
                var frame = _frameStack.Pop();

                variable.BackdoorSet(value);
                variable.LastSaveFramePointer = frame;
            }

            CurrentFramePointer = framePointer;

            _workList.Clear();
        }

        private readonly List<Constraint> _constraints = new List<Constraint>();

        /// <summary>
        /// All constraints in the system
        /// </summary>
        public IEnumerable<Constraint> Constraints { get { return _constraints; } } 

        private readonly List<Variable> _variables = new List<Variable>();

        /// <summary>
        /// All variables in the constraint system
        /// </summary>
        public IEnumerable<Variable> Variables { get { return _variables; } } 

        private readonly Dictionary<String, Variable> _nameToVariable = new Dictionary<string, Variable>(); 

        /// <summary>
        /// Looks up the variable with name = <paramref name="variableName"/>
        /// </summary>
        public Variable this[String variableName]
        {
            get { return _nameToVariable[variableName]; }
        }

        private uint _unnamedVariableIndex;

        internal void AddVariable(Variable variable)
        {
            _variables.Add(variable);

            if (variable.Name == null)
            {
                variable.Name = "_" + _unnamedVariableIndex;
                _unnamedVariableIndex++;
            }
            else if (variable.Name.StartsWith("_"))
            {
                throw new InvalidOperationException("Variable names cannot start with an underscore.");
            }
            else
            {
                _nameToVariable.Add(variable.Name, variable);
            }
        }

        internal void AddConstraint(Constraint constraint)
        {
            _constraints.Add(constraint);
        }

        internal void QueueForUpdate(ConstraintArc arc)
        {
            arc.QueuedForUpdate = true;
            _workList.Enqueue(arc);
        }

        /// <summary>
        /// Creates a new finite domain variable with the specified name, finite domain, and set of allowable values
        /// </summary>
        public FiniteDomainVariable<T> CreateFiniteDomainVariable<T>(String name, FiniteDomain<T> finiteDomain, IEnumerable<T> allowableValues)
        {
            return new FiniteDomainVariable<T>(this, name, finiteDomain, allowableValues);
        }
        
        /// <summary>
        /// Creates a new finite domain variable with the specified name, finite domain, and set of allowable values
        /// </summary>
        public FiniteDomainVariable<T> CreateFiniteDomainVariable<T>(String name, FiniteDomain<T> finiteDomain, params T[] allowableValues)
        {
            return new FiniteDomainVariable<T>(this, name, finiteDomain, allowableValues);
        }

        /// <summary>
        /// Creates a new finite domain variable from the specified finite domain and set of allowable values
        /// </summary>
        public FiniteDomainVariable<T> CreateFiniteDomainVariable<T>(FiniteDomain<T> finiteDomain, IEnumerable<T> allowableValues)
        {
            return new FiniteDomainVariable<T>(this, null, finiteDomain, allowableValues);
        }

        /// <summary>
        /// Creates a new finite domain variable from the specified finite domain and set of allowable values
        /// </summary>
        public FiniteDomainVariable<T> CreateFiniteDomainVariable<T>(FiniteDomain<T> finiteDomain, params T[] allowableValues)
        {
            return new FiniteDomainVariable<T>(this, null, finiteDomain, allowableValues);
        }

        /// <summary>
        /// Creates a new finite domain variable from the specified finite domain and set of all possible values in the finite domain
        /// </summary>
        public FiniteDomainVariable<T> CreateFiniteDomainVariable<T>(FiniteDomain<T> finiteDomain)
        {
            return new FiniteDomainVariable<T>(this, null, finiteDomain, finiteDomain.Values);
        }

        /// <summary>
        /// Creates a new real variable with the specified name, minimum value, and maximum value
        /// </summary>
        public RealVariable CreateRealVariable(String name, double minValue, double maxValue)
        {
            return new RealVariable(this, name, new Interval(minValue, maxValue));
        }

        /// <summary>
        /// Creates a new real variable with the specified name and range
        /// </summary>
        public RealVariable CreateRealVariable(String name, Interval range)
        {
            return new RealVariable(this, name, range);
        }

        /// <summary>
        /// Creates a new real variable with the specified name and the default range
        /// </summary>
        public RealVariable CreateRealVariable(String name)
        {
            return new RealVariable(this, name, RealVariable.DefaultRange);
        }

        /// <summary>
        /// Creates a new real variable with the specified minimum and maximum values
        /// </summary>
        public RealVariable CreateRealVariable(double minValue, double maxValue)
        {
            return new RealVariable(this, null, new Interval(minValue, maxValue));
        }

        /// <summary>
        /// Creates a new real variable with the specified value
        /// </summary>
        public RealVariable CreateRealVariable(double value)
        {
            return new RealVariable(this, null, new Interval(value, value));
        }

        /// <summary>
        /// Creates a new real variable with the specified range
        /// </summary>
        public RealVariable CreateRealVariable(Interval range)
        {
            return new RealVariable(this, null, range);
        }

        /// <summary>
        /// Creates a new real variable with the default range
        /// </summary>
        public RealVariable CreateRealVariable()
        {
            return new RealVariable(this, null, RealVariable.DefaultRange);
        }

        /// <summary>
        /// Creates a new vector 2 variable with the specified coordinates
        /// </summary>
        public Vector2Variable CreateVector2Variable(RealVariable x, RealVariable y)
        {
            return new Vector2Variable(x, y);
        }

        /// <summary>
        /// Creates a new vector 2 variable
        /// </summary>
        public Vector2Variable CreateVector2Variable()
        {
            return new Vector2Variable(this);
        }
        
        /// <summary>
        /// Creates a new Integer variable with the specified name, minimum value, and maximum value
        /// </summary>
        public IntegerVariable CreateIntegerVariable(String name, int minValue, int maxValue)
        {
            return new IntegerVariable(this, name, new IntegerInterval(minValue, maxValue));
        }

        /// <summary>
        /// Creates a new integer variable with the specified name and range
        /// </summary>
        public IntegerVariable CreateIntegerVariable(String name, IntegerInterval range)
        {
            return new IntegerVariable(this, name, range);
        }

        /// <summary>
        /// Creates a new integer variable with the specified name and the default range
        /// </summary>
        public IntegerVariable CreateIntegerVariable(String name)
        {
            return new IntegerVariable(this, name, IntegerVariable.DefaultRange);
        }

        /// <summary>
        /// Creates a new integer variable with the specified minimum and maximum values
        /// </summary>
        public IntegerVariable CreateIntegerVariable(int minValue, int maxValue)
        {
            return new IntegerVariable(this, null, new IntegerInterval(minValue, maxValue));
        }

        /// <summary>
        /// Creates a new integer variable with the specified value
        /// </summary>
        public IntegerVariable CreateIntegerVariable(int value)
        {
            return new IntegerVariable(this, null, new IntegerInterval(value, value));
        }

        /// <summary>
        /// Creates a new integer variable with the specified range
        /// </summary>
        public IntegerVariable CreateIntegerVariable(IntegerInterval range)
        {
            return new IntegerVariable(this, null, range);
        }

        /// <summary>
        /// Creates a new integer variable with the default range
        /// </summary>
        public IntegerVariable CreateIntegerVariable()
        {
            return new IntegerVariable(this, null, IntegerVariable.DefaultRange);
        }

        /// <summary>
        /// Creates a real variable whose value can be any one of a set of intervals, which correspond to values of the specified finite domain
        /// </summary>
        public RealVariable CreateEnumeratedReal<T>(FiniteDomainVariable<T> finiteDomainVariable, ScoreMapping<T> scoreMapping)
        {
            RealVariable realVariable = CreateRealVariable();
            
            Constraint.InRange(realVariable, scoreMapping.Select(pair => pair.Second).Aggregate(Interval.Union));

            Constraint.ScoreConstraint(realVariable, finiteDomainVariable, scoreMapping);

            return realVariable;
        }

        private bool _startedSolving = false;

        /// <summary>
        /// All solutions to the constraint system
        /// </summary>
        public SolutionSet Solutions { get; private set; }

        /// <summary>
        /// Enumerates all solutions to the constraint system
        /// </summary>
        private IEnumerable<Solution> SolutionsEnumerable
        {
            get
            {
                if (_startedSolving) throw new InvalidOperationException("This solver has already begun solving for solutions.");

                _startedSolving = true;

                // shuffle only if the random option has been set
                Variable[] variables = ExpansionOrder == ExpansionOrder.Random ? (_variables.ToShuffled(Random)) : _variables.ToArray();

                uint solutionNumber = 0;

                // if any variable is already empty when we start, don't bother searching for a solution
                foreach (var variable in variables)
                {
                    if (variable.IsEmpty) yield break;
                }
                
                Stopwatch stopwatch = new Stopwatch();
                
                stopwatch.Start();

                Stack<IEnumerator<object>> enumerators = new Stack<IEnumerator<object>>();
                enumerators.Push(variables[enumerators.Count].BoxedUniqueValues.GetEnumerator());
                
                while(enumerators.Count > 0)
                {
                    // if we found a new element
                    if (enumerators.Peek().MoveNext())
                    {
                        int index = enumerators.Count;

                        // are we out of variables?
                        if (index >= variables.Length)
                        {
                            // if so, then we have a solution!

                            stopwatch.Stop();

                            Solution solution = new Solution(this, stopwatch.Elapsed, solutionNumber);

                            yield return solution;

                            solutionNumber++;

                            stopwatch.Restart();
                        }
                        else
                        {
                            // otherwise, push a NEW enumerator for the next variable on the stack
                            enumerators.Push(variables[enumerators.Count].BoxedUniqueValues.GetEnumerator());
                        }
                    }
                    else
                    {
                        enumerators.Pop();
                    }
                }
            }
        }

        #region Subexpression Elimination

        internal readonly SubexpressionEliminator SubexpressionEliminator = new SubexpressionEliminator();

        #endregion
    }
}
