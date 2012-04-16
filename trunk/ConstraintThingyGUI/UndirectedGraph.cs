using System;
using System.Collections.Generic;

namespace ConstraintThingyGUI
{
    /// <summary>
    /// An implementation of an undirected graph.
    /// </summary>
    class UndirectedGraph
    {
        private readonly List<Node> _nodes = new List<Node>();
        private readonly List<UndirectedEdge> _edges = new List<UndirectedEdge>();

        public IEnumerable<Node> Nodes
        {
            get { return _nodes; }
        }

        public IEnumerable<UndirectedEdge> Edges
        {
            get { return _edges; }
        }

        public event Action<Node> OnNodeAdded;
        public event Action<Node> OnNodeRemoved;
        public event Action<UndirectedEdge> OnEdgeAdded;
        public event Action<UndirectedEdge> OnEdgeRemoved;

        public void AddNode(Node node)
        {
            if (_nodes.Contains(node)) throw new InvalidOperationException("The node was already present in the graph.");
            _nodes.Add(node);
            if (OnNodeAdded != null) OnNodeAdded(node);
        }

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

        public void AddEdge(UndirectedEdge node)
        {
            if (_edges.Contains(node)) throw new InvalidOperationException("The edge was already present in the graph.");
            _edges.Add(node);
            if (OnEdgeAdded != null) OnEdgeAdded(node);
        }

        public void RemoveEdge(UndirectedEdge node)
        {
            if (!_edges.Remove(node)) throw new InvalidOperationException("The edge was not in the graph.");
            if (OnEdgeRemoved != null) OnEdgeRemoved(node);
        }
    }
}