using System;
using System.Collections.Generic;
using Intervals;

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

        /// <summary>
        /// Loads a graph from a CSV file
        /// </summary>
        public static UndirectedGraph FromSpreadsheet(string path, float coordinateScaling)
        {
            var graph = new UndirectedGraph();
            var data = Spreadsheet.ConvertAllNumbers(Spreadsheet.Read(path, ','));
            var heading = new string[] {"Name", "X", "Y", "Connections"};

            // Check heading
            for (int i = 0; i<heading.Length; i++)
                if ((data[0][i] as string) != heading[i])
                    throw new Exception("Spreadsheet has the wrong format: Incorrect heading");

            // Create all the nodes
            for (int rowNumber=1; rowNumber<data.Length; rowNumber++)
            {
                var row = data[rowNumber];
                if (!(row[0] is string && row[1] is double && row[2] is double))
                    throw new Exception("Spreadsheet has the wrong format in row "+rowNumber);
                graph.AddNode(new Node((string)row[0],
                                new AABB(new Vector2(70+coordinateScaling*Convert.ToSingle(row[1]),
                                                     70+coordinateScaling*Convert.ToSingle(row[2])),
                                        130, 130)));
            }

            // Create all the edges
            for (int rowNumber = 1; rowNumber < data.Length; rowNumber++)
            {
                var row = data[rowNumber];
                Node start = graph.FindNode((string)row[0]);
                for (int column = 3; column < row.Length; column++)
                {
                    if (!(row[column] is string))
                        throw new Exception(string.Format("Spreadsheet has the wrong format in row {0}; bad connection name '{1}'.", rowNumber, row[column]));
                    if ((string)row[column] != "")
                    {
                        Node end = graph.FindNode((string) row[column]);
                        if (end == null)
                            throw new Exception(string.Format("Unknown node '{0}'referenced in row {1}, column {2}",
                                                              row[column], rowNumber, column));
                        graph.AddEdge(new UndirectedEdge(start, end));
                    }
                }
            }

            return graph;
        }

        public Node FindNode(string name)
        {
            foreach (var n in Nodes)
                if (n.Name == name)
                    return n;
            return null;
        }
    }
}