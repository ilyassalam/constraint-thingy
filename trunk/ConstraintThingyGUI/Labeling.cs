using System.Collections.Generic;
using System.Text;
using ConstraintThingy;

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
            {
                result.Append(l[n].ToString());
                result.Append('\n');
            }
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
        protected T ValueVariable(Node n)
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
            return new FiniteDomainVariable(string.Format("node:{0}", Name), Domain);
        }

        /// <summary>
        /// Gets or sets a value for the node.
        /// </summary>
        public override object this[Node n]
        {
            get { return ValueVariable(n).ToString(); }
            set { ValueVariable(n).UniqueValue = (string)value; }
        }
    }
}
