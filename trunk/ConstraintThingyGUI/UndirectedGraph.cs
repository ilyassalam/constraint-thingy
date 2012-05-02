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
        public void AddEdge(UndirectedEdge edge)
        {
            if (_edges.Contains(edge)) throw new InvalidOperationException("The edge was already present in the graph.");
            _edges.Add(edge);
            edge.First.AddNeighbor(edge.Second);
            edge.Second.AddNeighbor(edge.First);
            if (OnEdgeAdded != null) OnEdgeAdded(edge);
        }

        /// <summary>
        /// Remove edge from the graph.
        /// </summary>
        /// <param name="edge"></param>
        public void RemoveEdge(UndirectedEdge edge)
        {
            if (!_edges.Remove(edge)) throw new InvalidOperationException("The edge was not in the graph.");
            edge.First.RemoveNeighbor(edge.Second);
            edge.Second.RemoveNeighbor(edge.First);
            if (OnEdgeRemoved != null) OnEdgeRemoved(edge);
        }

        /// <summary>
        /// Computes the distances from start to all nodes lying on paths from start to end.
        /// Note: this will not necessarily include all nodes in the graph, since some will not
        /// lie on any simple path from start to end.
        /// </summary>
        private Dictionary<Node, int> ComputeDistances(Node start)
        {
            var q = new Queue<Node>();
            var distances = new Dictionary<Node, int>();
            distances[start] = 0;
            q.Enqueue(start);
            while (q.Count != 0)
            {
                Node current = q.Dequeue();
                int dist = distances[current];
                foreach (var neighbor in current.Neighbors)
                {
                    if (!distances.ContainsKey(neighbor))
                    {
                        distances[neighbor] = dist + 1;
                        q.Enqueue(neighbor);
                    }
                }
            }
            return distances;
        }

        private readonly Dictionary<Node, Dictionary<Node, int>> distanceMaps = new Dictionary<Node, Dictionary<Node, int>>();

        Dictionary<Node, int> DistanceMap(Node n)
        {
            Dictionary<Node, int> result;
            if (!distanceMaps.TryGetValue(n, out result))
                result = distanceMaps[n] = ComputeDistances(n);
            return result;
        }

        /// <summary>
        /// Returns the distance from start node to end node.
        /// </summary>
        public int Distance(Node start, Node end)
        {
            return DistanceMap(start)[end];
        }
    }
}