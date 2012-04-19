using System;
using System.Collections.Generic;

namespace ConstraintThingyGUI
{
    /// <summary>
    /// An implementation of an undirected graph.
    /// </summary>
    public class UndirectedGraph
    {
        /// <summary>
        /// The graph currently being worked on
        /// This should get refactored to a different location.
        /// </summary>
        public static UndirectedGraph CurrentGraph;

        private readonly List<Node> _nodes = new List<Node>();
        private readonly List<UndirectedEdge> _edges = new List<UndirectedEdge>();

        /// <summary>
        /// Nodes of the graph
        /// </summary>
        public IList<Node> Nodes
        {
            get { return _nodes; }
        }

        /// <summary>
        /// Edges of the graph
        /// </summary>
        public IList<UndirectedEdge> Edges
        {
            get { return _edges; }
        }

        /// <summary>
        /// Called when a node is added to the graph
        /// </summary>
        public event Action<Node> OnNodeAdded;
        /// <summary>
        /// Called when a node is removed from the graph
        /// </summary>
        public event Action<Node> OnNodeRemoved;
        /// <summary>
        /// Called when an edge is added to the graph
        /// </summary>
        public event Action<UndirectedEdge> OnEdgeAdded;
        /// <summary>
        /// Called when an edge is removed from the graph.
        /// </summary>
        public event Action<UndirectedEdge> OnEdgeRemoved;

        /// <summary>
        /// Adds node the the graph
        /// </summary>
        public void AddNode(Node node)
        {
            if (_nodes.Contains(node)) throw new InvalidOperationException("The node was already present in the graph.");
            _nodes.Add(node);
            if (OnNodeAdded != null) OnNodeAdded(node);
        }

        /// <summary>
        /// Removes node from the graph.
        /// </summary>
        public void RemoveNode(Node node)
        {
            if (!_nodes.Remove(node)) throw new InvalidOperationException("The node was not in the graph.");
            if (OnNodeRemoved != null) OnNodeRemoved(node);


            // remove all edges that use this node
            List<UndirectedEdge> toRemove = new List<UndirectedEdge>(0);
            foreach (var edge in _edges)
            {
                if (edge.Contains(node))
                {
                    toRemove.Add(edge);
                }
            }

            foreach (var edge in toRemove)
            {
                RemoveEdge(edge);
            }
        }

        /// <summary>
        /// Add edge to the graph.
        /// </summary>
        public void AddEdge(UndirectedEdge node)
        {
            if (_edges.Contains(node)) throw new InvalidOperationException("The edge was already present in the graph.");
            _edges.Add(node);
            if (OnEdgeAdded != null) OnEdgeAdded(node);
        }

        /// <summary>
        /// Remove edge from the graph.
        /// </summary>
        /// <param name="node"></param>
        public void RemoveEdge(UndirectedEdge node)
        {
            if (!_edges.Remove(node)) throw new InvalidOperationException("The edge was not in the graph.");
            if (OnEdgeRemoved != null) OnEdgeRemoved(node);
        }
    }
}