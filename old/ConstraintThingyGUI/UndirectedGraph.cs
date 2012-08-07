using System;
using System.Collections.Generic;
using System.IO;
using Intervals;
using System.Linq;

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
            var heading = new[] {"Name", "X", "Y", "Supports", "Connections"};

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
                                                     170+coordinateScaling*Convert.ToSingle(row[2])),
                                        130, 130)));
            }

            // Fill in support information
            for (int rowNumber = 1; rowNumber < data.Length; rowNumber++)
            {
                object[] row = data[rowNumber];
                string supportRecipient = row[3] as string;
                if (!string.IsNullOrWhiteSpace(supportRecipient))
                    graph.FindNode((string)row[0]).SetSupport(graph.FindNode(supportRecipient));
            }

            // Create all the edges
            for (int rowNumber = 1; rowNumber < data.Length; rowNumber++)
            {
                var row = data[rowNumber];
                Node start = graph.FindNode((string)row[0]);
                for (int column = 4; column < row.Length; column++)
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

        /// <summary>
        /// Returns the node with the given name.
        /// </summary>
        public Node FindNode(string name)
        {
            foreach (var n in Nodes)
                if (n.Name == name)
                    return n;
            return null;
        }

        public void WriteASPFormat(string path, params string[] pathPredicates)
        {
            using (var writer = File.CreateText(path))
            {
                WriteRoomPredicates(writer);
                foreach (var pred in pathPredicates)
                    WritePathPredicate(writer, pred);
            }
        }

        private void WriteRoomPredicates(StreamWriter writer)
        {
            writer.WriteLine("%");
            writer.WriteLine("% Room declarations");
            writer.WriteLine("%");
            writer.Write("room(");
            bool first = true;
            foreach (var n  in Nodes)
            {
                if (first)
                    first = false;
                else
                    writer.Write(";");
                writer.Write(n.Name.ToLower());
            }
            writer.WriteLine(").");
            writer.Write("has_support(");
            first = true;
            foreach (var n in Nodes)
            {
                if (n.Support.Count > 0)
                {
                    if (first)
                        first = false;
                    else
                        writer.Write(";");
                    writer.Write(n.Name.ToLower());
                }
            }
            writer.WriteLine(").");
            writer.WriteLine();
            writer.Write("is_supporter(");
            first = true;
            foreach (var n in Nodes)
            {
                if (n.SupportRecipient != null)
                {
                    if (first)
                        first = false;
                    else
                        writer.Write(";");
                    writer.Write(n.Name.ToLower());
                }
            }
            writer.WriteLine(").");
            writer.WriteLine();
        }

        private void WritePathPredicate(TextWriter writer, string pred)
        {
            writer.WriteLine("%");
            writer.WriteLine("% {0} declarations", pred);
            writer.WriteLine("%");
            writer.WriteLine();
            WriteMainPredicateAndSupportPredicate(writer, pred);
            writer.WriteLine();
            writer.WriteLine("% inheritance");
            WriteInheritancePredicate(writer, pred);
            writer.WriteLine();
        }

        private void WriteMainPredicateAndSupportPredicate(TextWriter writer, string pred)
        {
            foreach (var n in Nodes)
                WriteMainPredicateAndSupportPredicate(writer, pred, n);
        }

        private void WriteMainPredicateAndSupportPredicate(TextWriter writer, string pred, Node n)
        {
            writer.WriteLine("{0}({1}, Support+Inherit) :-\n    {0}_support({1}, Support), {0}_inheritance({1}, Inherit).", pred, n.Name.ToLower());
            writer.Write("{0}_support({1}, Support0", pred, n.Name.ToLower());
            for (int i=0; i<n.Support.Count;i++)
                writer.Write("+Support{0}",i+1);
            writer.WriteLine(") :-");
            writer.Write("    {0}_delta({1}, Support0)", pred, n.Name.ToLower());
            for (int i = 0; i < n.Support.Count;i++ )
                writer.Write(", {0}_delta({1}, Support{2})", pred, n.Support[i].Name.ToLower(), i + 1);
            writer.WriteLine(".");
            //writer.Write("{0}_support({1}, {0}_delta({1})", pred, n.Name.ToLower());
            //foreach (var s in n.Support)
            //{
            //    writer.Write("+{0}_delta({1})", pred, s.Name.ToLower());
            //}
            //writer.WriteLine(").");
        }

        private void WriteInheritancePredicate(TextWriter writer, string pred)
        {
            foreach (var n in Nodes)
                WriteInheritancePredicate(writer, pred, n);
        }

        private void WriteInheritancePredicate(TextWriter writer, string pred, Node n)
        {
            List<Node> predecessors =
                new List<Node>(Nodes.Where(candidate => candidate.SupportRecipient==null && (Edges.Any(e => (e.First == candidate && e.Second == n)))));
            writer.WriteLine("{0}_inheritance({1}, InheritanceMin) :-", pred, n.Name.ToLower());
            if (predecessors.Count == 0)
                writer.Write("    {0}_delta({1}, InheritanceMin)", pred, n.Name.ToLower());
            else
            {
                writer.Write("    {0}_delta({1}, Inheritance0)", pred, n.Name.ToLower());
                int neighborCount = 0;
                foreach (var s in n.Neighbors)
                {
                    writer.Write(", {0}_delta({1}, Inheritance{2})", pred, s.Name.ToLower(), neighborCount+1);
                    neighborCount++;
                }
                writer.WriteLine();
                writer.Write("    ");
                if (neighborCount==1)
                    writer.Write(", min(Inheritance0, Inheritance1, InheritanceMin)");
                else
                {
                    writer.Write(", min(Inheritance0, Inheritance1, Temp0)");
                    for (int i = 0; i < neighborCount - 2; i++)
                        writer.Write(", min(Temp{0}, Inheritance{1}, Temp{2})", i, i + 2, i + 1);
                    writer.Write(", min(Temp{0}, Inheritance{1}, InheritanceMin)", neighborCount-2, neighborCount);
                }
            }
            writer.WriteLine(".");
        }
    }
}