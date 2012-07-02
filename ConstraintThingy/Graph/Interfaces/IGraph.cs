using System;
using System.Collections.Generic;

namespace ConstraintThingy
{
    /// <summary>
    /// Represents a collection of nodes and edges
    /// </summary>
    public interface IGraph : IEnumerable<INode>
    {
        /// <summary>
        /// The nodes in the graph
        /// </summary>
        IEnumerable<INode> Nodes { get; }

        /// <summary>
        /// The edges in the graph
        /// </summary>
        IEnumerable<IEdge> Edges { get; }

        /// <summary>
        /// Adds a node to the graph
        /// </summary>
        void AddNode(INode node);

        /// <summary>
        /// Removes a node from the graph
        /// </summary>
        void RemoveNode(INode node);

        /// <summary>
        /// Fired when a node is added to the graph
        /// </summary>
        event Action<INode> NodeAdded;

        /// <summary>
        /// Fired when a node is removed from the graph
        /// </summary>
        event Action<INode> NodeRemoved;

        /// <summary>
        /// Fired when an edge is added to the graph
        /// </summary>
        event Action<IEdge> EdgeAdded;

        /// <summary>
        /// Fired when an edge is removed from the graph
        /// </summary>
        event Action<IEdge> EdgeRemoved;
    }
}