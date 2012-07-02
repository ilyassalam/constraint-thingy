using System;
using System.Collections.Generic;
using System.Linq;
using ConstraintThingy;

namespace ConstraintThingy
{
    /// <summary>
    /// Represents a node in a graph
    /// </summary>
    public abstract class Node : INode
    {
        private readonly List<Variable> _variables = new List<Variable>();

        /// <summary>
        /// All variables associated with this node
        /// </summary>
        public IEnumerable<Variable> Variables { get { return _variables; } }

        /// <summary>
        /// Adds a variable to the node
        /// </summary>
        public void AddVariable(Variable variable)
        {
            _variables.Add(variable);
        }

        /// <summary>
        /// The position of the node
        /// </summary>
        public abstract Vector2 Position { get; set; }

        /// <summary>
        /// Fired when the position changes
        /// </summary>
        public abstract event Action<Vector2> PositionChanged;

        /// <summary>
        /// The edges coming out of this node
        /// </summary>
        public IEnumerable<IEdge> Edges
        {
            get { return _edges; }
        }

        /// <summary>
        /// The neighbors of this node
        /// </summary>
        public IEnumerable<INode> Neighbors
        {
            get { return Edges.Select(e => e.To); }
        }

        private readonly List<IEdge> _edges = new List<IEdge>();

        /// <summary>
        /// Adds a directional edge to another node
        /// </summary>
        public void AddEdge(IEdge edge)
        {
            if (edge.From != this || edge.To == this) throw new InvalidOperationException("Edges must begin at this node and end at another node.");

            if (_edges.Contains(edge)) throw new InvalidOperationException("This node already contains that edge");

            _edges.Add(edge);

            if (EdgeAdded != null) EdgeAdded(edge);
        }

        /// <summary>
        /// Removes a directional edge to another node
        /// </summary>
        /// <param name="edge"></param>
        public void RemoveEdge(IEdge edge)
        {
            if (!_edges.Remove(edge)) throw new InvalidOperationException("The edge was not found");

            if (EdgeRemoved != null) EdgeRemoved(edge);
        }

        /// <summary>
        /// Fired when an edge is added to the node
        /// </summary>
        public event Action<IEdge> EdgeAdded;

        /// <summary>
        /// Fired when an edge is removed from the node
        /// </summary>
        public event Action<IEdge> EdgeRemoved;
    }
}