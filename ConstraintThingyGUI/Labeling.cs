using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using ConstraintThingy;
using Intervals;

namespace ConstraintThingyGUI
{
    /// <summary>
    /// A labeling is a mapping from graph nodes to values, in this case, represented by constrained variables
    /// </summary>
    public abstract class Labeling
    {
        /// <summary>
        /// The current value of the labeling on the specified node.  Type depending on the kind of labeling.
        /// </summary>
        public abstract object this[Node n] { get; set; }

        /// <summary>
        /// Returns a string with the value of each label on the node, separated by line breaks
        /// </summary>
        public static string FormatLabels(Node n)
        {
            var result = new StringBuilder();
            foreach (var l in AllLabelings)
                result.AppendFormat("{0}={1}\n", l.Name, l[n]);
            return result.ToString();
        }

        /// <summary>
        /// List of all Labelings that have been created.
        /// </summary>
        public static readonly List<Labeling> AllLabelings = new List<Labeling>();

        /// <summary>
        /// Name of label, for debugging purposes
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Creates a Labeling object
        /// </summary>
        protected Labeling(string name)
        {
            Name = name;
            AllLabelings.Add(this);
        }
    }

    /// <summary>
    /// Specialization of a labeling for a particular kind of type variable
    /// </summary>
    /// <typeparam name="T">Type of Variable to use for this labeling</typeparam>
    public abstract class Labeling<T> : Labeling where T: Variable
    {
        /// <summary>
        /// Creates a Labeling object
        /// </summary>
        protected Labeling(string name) : base(name)
        { }

        private readonly Dictionary<Node,T> variables = new Dictionary<Node,T>();

        /// <summary>
        /// Returns the Variable used to represent the value of this labeling on this node, creating it, if necessary.
        /// </summary>
        public T ValueVariable(Node n)
        {
            T result;
            if (!variables.TryGetValue(n, out result))
                variables[n] = result = MakeVariable(n);
            return result;
        }

        /// <summary>
        /// Returns an array of all variables corresponding to the specified nodes.
        /// For use in creating constraints.
        /// </summary>
        public T[] ValueVariables(IList<Node> nodes)
        {
            var result = new T[nodes.Count];
            for (int i = 0; i < result.Length; i++)
                result[i] = ValueVariable(nodes[i]);
            return result;
        }

        /// <summary>
        /// Returns an array of all variables corresponding to the nodes of GRAPH.
        /// For use in creating constraints.
        /// </summary>
        public T[] ValueVariables(UndirectedGraph graph)
        {
            return ValueVariables(graph.Nodes);
        }

        /// <summary>
        /// Returns an array of all variables corresponding to the nodes of the current graph.
        /// For use in creating constraints.
        /// </summary>
        public T[] ValueVariables()
        {
            return ValueVariables(UndirectedGraph.CurrentGraph);
        }

        /// <summary>
        /// Creates a new variable to represent the value of this labeling on the specified node.
        /// </summary>
        protected abstract T MakeVariable(Node n);
    }

    /// <summary>
    /// A labeling over a finite domain
    /// </summary>
    public class FiniteDomainLabeling : Labeling<FiniteDomainVariable>
    {

        /// <summary>
        /// A labeling over finite domain D
        /// </summary>
        public FiniteDomainLabeling(string name, FiniteDomain d)
            : base(name)
        {
            Domain = d;
        }

        /// <summary>
        /// The domain over which this labeling takes values.
        /// </summary>
        public FiniteDomain Domain { get; private set; }

        /// <summary>
        /// Make the variable to hold a value for n
        /// </summary>
        protected override FiniteDomainVariable MakeVariable(Node n)
        {
            return new FiniteDomainVariable(string.Format("{0}:{1}", n.Name, Name), Domain);
        }

        /// <summary>
        /// Gets or sets a value for the node.
        /// </summary>
        public override object this[Node n]
        {
            get { return ValueVariable(n).ValueString; }
            set { ValueVariable(n).UniqueValue = (string)value; }
        }

        /// <summary>
        /// Limits how many times VALUE can occur in the labeling
        /// </summary>
        public void LimitOccurences(string value, int minOccurences, int maxOccurences)
        {
            new CardinalityConstraint(value, minOccurences, maxOccurences, ValueVariables());
        }
    }

    /// <summary>
    /// A labeling whose value is a numeric interval.
    /// </summary>
    public class IntervalLabeling : Labeling<IntervalVariable>
    {
        public IntervalLabeling(string name, Interval range) : base(name)
        {
            InitialRange = range;
        }

        /// <summary>
        /// Initial range assigned to nodes before constraint propagation or other operations.
        /// </summary>
        public Interval InitialRange;

        protected override IntervalVariable MakeVariable(Node n)
        {
            return new IntervalVariable(string.Format("{0}:{1}", n.Name, Name), InitialRange);
        }

        public override object this[Node n]
        {
            get
            {
                return ValueVariable(n).Value;
            }
            set
            {
                Debug.Assert(value != null, "value != null");
                if (value is float || value is double || value is int)
                    ValueVariable(n).Value = new Interval(Convert.ToSingle(value), Convert.ToSingle(value));
                else
                    ValueVariable(n).Value = (Interval)value;
            }
        }

        public static IntervalLabeling operator +(IntervalLabeling a, IntervalLabeling b)
        {
            return new IntervalSumLabeling("sum", a, b);
        }
    }

    class IntervalSumLabeling : IntervalLabeling
    {
        public IntervalSumLabeling(string name, IntervalLabeling a, IntervalLabeling b)
            : base(name, a.InitialRange + b.InitialRange)
        {
            LHS = a;
            RHS = b;
        }

        public readonly IntervalLabeling LHS;
        public readonly IntervalLabeling RHS;

        protected override IntervalVariable MakeVariable(Node n)
        {
            IntervalVariable result = base.MakeVariable(n);
            new IntervalSumConstraint(result, LHS.ValueVariable(n), RHS.ValueVariable(n));
            return result;
        }
    }

    class ScoreLabeling : IntervalLabeling
    {
        public ScoreLabeling(string name, FiniteDomainLabeling labeling, float defScore, params object[] scores) : base(name, new Interval(0,0))
        {
            baseLabeling = labeling;
            defaultScore = defScore;
            scoredLabels = new ulong[scores.Length/2];
            labelScores = new float[scores.Length / 2];
            float minScore = defScore;
            float maxScore = defScore;
            int label = 0;
            UInt64 mask = 0;
            for (int i=0; i<scores.Length;)
            {
                mask |= scoredLabels[label] = labeling.Domain.Bitmask((string) scores[i]);
                i++;
                var score = Convert.ToSingle(scores[i]);
                labelScores[label] = score;
                if (score < minScore)
                    minScore = score;
                if (score > maxScore)
                    maxScore = score;
                i++;
                label++;
            }
            InitialRange = new Interval(minScore, maxScore);
            unscoredLabelMask = ~mask & labeling.Domain.UniverseMask;
        }

        private readonly UInt64[] scoredLabels;
        private readonly UInt64 unscoredLabelMask;
        private readonly float[] labelScores;
        private readonly float defaultScore;
        private readonly FiniteDomainLabeling baseLabeling;

        /// <summary>
        /// Given a bitmask for the set of possible values for a variable, return the interval of possible scores for those values.
        /// </summary>
        Interval ScoreInterval(UInt64 mask)
        {
            Interval answer = new Interval(0,0);
            bool gotOne = false;
            for (int i=0; i<scoredLabels.Length; i++)
            {
                if ((scoredLabels[i]&mask)!=0)
                {
                    if (gotOne)
                        answer.Extend(labelScores[i]);
                    else
                    {
                        gotOne = true;
                        answer = new Interval(labelScores[i], labelScores[i]);
                    }
                }
            }
            if (gotOne)
            {
                if (HasUnscoredLabels(mask))
                    answer.Extend(defaultScore);
            }
            else
                answer = new Interval(defaultScore,defaultScore);
            return answer;
        }

        /// <summary>
        /// Given an interval of possible scores, return the bitmask for the set of labels whose scores are in that interval.
        /// </summary>
        UInt64 IntervalMask(Interval scoreRange)
        {
            UInt64 result = 0;
            if (scoreRange.Contains(defaultScore))
                result |= unscoredLabelMask;
            for (int i = 0; i<scoredLabels.Length;i++)
                if (scoreRange.Contains(labelScores[i]))
                    result |= scoredLabels[i];
            return result;
        }

        private bool HasUnscoredLabels(ulong mask)
        {
            return (mask&unscoredLabelMask)!=0;
        }

        protected override IntervalVariable MakeVariable(Node n)
        {
            var scoreVariable = base.MakeVariable(n);
            new ScoringConstraint(scoreVariable, baseLabeling.ValueVariable(n), this);
            return scoreVariable;
        }

        class ScoringConstraint : Constraint<Variable>
        {
            public ScoringConstraint(IntervalVariable score, FiniteDomainVariable label, ScoreLabeling sLabeling) : base(new Variable[] { score, label })
            {
                scoreLabeling = sLabeling;
                this.score = score;
                this.label = label;
            }

            private readonly ScoreLabeling scoreLabeling;
            private readonly IntervalVariable score;
            private readonly FiniteDomainVariable label;

            public override void Narrowed(Variable narrowedVariable)
            {
                if (narrowedVariable == score)
                    label.Value &= scoreLabeling.IntervalMask(score.Value);
                else
                    // narrowedVariable == label
                    score.Value = Interval.Intersection(score.Value, scoreLabeling.ScoreInterval(label.Value));
            }

            public override void UpdateVariable(Variable var)
            {
                throw new NotImplementedException();
            }
        }
    }
}
