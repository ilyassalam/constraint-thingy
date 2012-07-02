using System;
using System.Collections.Generic;
using ConstraintThingy;

namespace ConstraintThingy
{
    /// <summary>
    /// Represents a node in a graph
    /// </summary>
    public interface INode
    {
        /// <summary>
        /// The position of the node
        /// </summary>
        Vector2 Position { get; set; }

        /// <summary>
        /// Fired when the position changes
        /// </summary>
        event Action<Vector2> PositionChanged;

        /// <summary>
        /// The edges coming out of this node
        /// </summary>
        IEnumerable<IEdge> Edges { get; }

        /// <summary>
        /// The neighbors of this node
        /// </summary>
        IEnumerable<INode> Neighbors { get; }

        /// <summary>
        /// Adds a directional edge to another node
        /// </summary>
        void AddEdge(IEdge edge);

        /// <summary>
        /// Removes a directional edge to another node
        /// </summary>
        /// <param name="edge"></param>
        void RemoveEdge(IEdge edge);

        /// <summary>
        /// Fired when an edge is added
        /// </summary>
        event Action<IEdge> EdgeAdded;

        /// <summary>
        /// Fired when an edge is removed
        /// </summary>
        event Action<IEdge> EdgeRemoved;

        /// <summary>
        /// All variables associated with this node
        /// </summary>
        IEnumerable<Variable> Variables { get; } 

        /// <summary>
        /// Adds a variable to the node
        /// </summary>
        void AddVariable(Variable variable);
    }
}