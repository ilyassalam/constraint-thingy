using System;
using System.Collections;
using System.Collections.Generic;

namespace ConstraintThingy
{
    /// <summary>
    /// Represents a collection of nodes and edges
    /// </summary>
    public class Graph : IGraph
    {
        /// <summary>
        /// The nodes in the graph
        /// </summary>
        public IEnumerable<INode> Nodes
        {
            get { return _nodes; }
        }

        /// <summary>
        /// The edges in the graph
        /// </summary>
        public IEnumerable<IEdge> Edges
        {
            get { return _edges; }
        }

        /// <summary>
        /// Fired when a node is added to the graph
        /// </summary>
        public event Action<INode> NodeAdded;
        
        /// <summary>
        /// Fired when a node is removed from the graph
        /// </summary>
        public event Action<INode> NodeRemoved;

        /// <summary>
        /// Fired when an edge is added to the graph
        /// </summary>
        public event Action<IEdge> EdgeAdded;

        /// <summary>
        /// Fired when an edge is removed from the graph
        /// </summary>
        public event Action<IEdge> EdgeRemoved;

        private readonly List<INode> _nodes = new List<INode>();

        private readonly List<IEdge> _edges = new List<IEdge>();

        /// <summary>
        /// Adds a node to the graph
        /// </summary>
        public void AddNode(INode node)
        {
            if (_nodes.Contains(node)) throw new InvalidOperationException("That node was already part of the graph.");

            _nodes.Add(node);

            node.EdgeAdded += AddEdge;
            node.EdgeRemoved += RemoveEdge;

            foreach (var edge in node.Edges)
            {
                AddEdge(edge);
            }

            if (NodeAdded != null) NodeAdded(node);
        }

        private void AddEdge(IEdge edge)
        {
            _edges.Add(edge);
            if (EdgeAdded != null) EdgeAdded(edge);
        }

        /// <summary>
        /// Removes a node from the graph
        /// </summary>
        public void RemoveNode(INode node)
        {
            if (!_nodes.Remove(node)) throw new InvalidOperationException("That node was not part of the graph.");

            node.EdgeAdded -= AddEdge;
            node.EdgeRemoved -= EdgeRemoved;

            foreach (var edge in Edges)
            {
                RemoveEdge(edge);
            }

            if (NodeRemoved != null) NodeRemoved(node);
        }

        private void RemoveEdge(IEdge edge)
        {
            _edges.Remove(edge);

            if (EdgeRemoved != null) EdgeRemoved(edge);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        public IEnumerator<INode> GetEnumerator()
        {
            return _nodes.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}