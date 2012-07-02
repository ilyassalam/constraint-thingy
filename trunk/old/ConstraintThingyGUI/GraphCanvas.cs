using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Intervals;

namespace ConstraintThingyGUI
{
    class GraphCanvas : Canvas 
    {
        public GraphCanvas()
        {
            ClipToBounds = true;
        }

        public GraphCanvas(UndirectedGraph graph)
        {
            Graph = graph;
        }

        private Dictionary<Node, Rectangle> nodeMapping;
        private Dictionary<Node, TextBlock> textMapping;
        private Dictionary<UndirectedEdge, Line> edgeMapping;
        private Dictionary<Node, List<UndirectedEdge>> nodesToEdges; 

        private UndirectedGraph _graph;

        public UndirectedGraph Graph
        {
            get { return _graph; }
            set
            {
                DetachGraph();
                _graph = value;
                AttachGraph();
            }
        } 

        private void DetachGraph()
        {
            if (nodeMapping != null)
            {
                foreach (var kvp in nodeMapping)
                {
                    Children.Remove(kvp.Value);
                }

                foreach (var kvp in textMapping)
                {
                    Children.Remove(kvp.Value);
                }

                _graph.OnNodeAdded -= AddNode;
                _graph.OnNodeRemoved -= RemoveNode;

                foreach (var kvp in edgeMapping)
                {
                    Children.Remove(kvp.Value);
                }

                _graph.OnEdgeAdded -= AddEdge;
                _graph.OnEdgeRemoved -= RemoveEdge;
            }

            nodeMapping = new Dictionary<Node, Rectangle>();
            textMapping = new Dictionary<Node, TextBlock>();
            edgeMapping = new Dictionary<UndirectedEdge, Line>();
            nodesToEdges = new Dictionary<Node, List<UndirectedEdge>>();
        }

        private void AttachGraph()
        {
            foreach (var node in _graph.Nodes)
            {
                AddNode(node);
            }

            _graph.OnNodeAdded += AddNode;
            _graph.OnNodeRemoved += RemoveNode;

            foreach (var edge in _graph.Edges)
            {
                AddEdge(edge);
            }

            _graph.OnEdgeAdded += AddEdge;
            _graph.OnEdgeRemoved += RemoveEdge;
        }

        private static readonly Brush[] brushes = {
                                             Brushes.Red, Brushes.Green, Brushes.Blue, Brushes.Yellow, Brushes.Cyan,
                                             Brushes.Magenta
                                         };

        private int brushCounter;
        readonly Dictionary<Node,Brush> nodeBrushes = new Dictionary<Node, Brush>();

        Brush Fill(Node n)
        {
            if (n.SupportRecipient != null)
                return Fill(n.SupportRecipient);
            if (n.Support.Count == 0)
                return Brushes.White;
            Brush b;
            if (!nodeBrushes.TryGetValue(n, out b))
                nodeBrushes[n] = b = brushes[brushCounter++ % brushes.Length];
            return b;
        }

        private void AddNode(Node node)
        {
            var rectangle = new Rectangle
                                {
                                    Width = node.AABB.Width,
                                    Height = node.AABB.Height,
                                    RenderTransform = new TranslateTransform(node.AABB.UpperLeft.X, node.AABB.UpperLeft.Y),
                                    Stroke = Brushes.Black,
                                    Fill = Fill(node)
                                };
            nodeMapping[node] = rectangle;
            Children.Add(rectangle); 
            
            var text = new TextBlock
                           {
                               Width = node.AABB.Width,
                               Height = node.AABB.Height,
                               RenderTransform = new TranslateTransform(node.AABB.UpperLeft.X, node.AABB.UpperLeft.Y)
                           };

            textMapping[node] = text;
            text.Text = Labeling.FormatLabels(node);
            Children.Add(text);
        }

        public void UpdateText()
        {
            foreach (var pair in textMapping)
                pair.Value.Text = Labeling.FormatLabels(pair.Key);
        }

        private void RemoveNode(Node node)
        {
            Children.Remove(nodeMapping[node]);
            Children.Remove(textMapping[node]);

            nodeMapping.Remove(node);
            textMapping.Remove(node);
        }

        private void AddEdge(UndirectedEdge edge)
        {
            AssociateNodeWithEdge(edge.First, edge);
            AssociateNodeWithEdge(edge.Second, edge);

            Vector2 center1 = edge.First.AABB.Center;
            Vector2 center2 = edge.Second.AABB.Center;

            var line = new Line
                           {
                               X1 = center1.X,
                               Y1 = center1.Y,

                               X2 = center2.X,
                               Y2 = center2.Y,

                               Stroke = Brushes.Black
                           };

            edgeMapping.Add(edge, line);

            Children.Add(line);
        }

        private void AssociateNodeWithEdge(Node node, UndirectedEdge edge)
        {
            if (!nodeMapping.ContainsKey(node)) throw new InvalidOperationException("A node referenced by an edge was not in the graph.");

            List<UndirectedEdge> edges;
            nodesToEdges.TryGetValue(node, out edges);

            if (edges == null)
            {
                edges = new List<UndirectedEdge>();
                nodesToEdges[node] = edges;
            }

            edges.Add(edge);
        }

        private void UnassociateNodeWithEdge(Node node, UndirectedEdge edge)
        {
            List<UndirectedEdge> edges;
            nodesToEdges.TryGetValue(node, out edges);

            if (edges != null)
            {
                edges.Remove(edge);
            }
        }

        private void RemoveEdge(UndirectedEdge edge)
        {
            Children.Remove(edgeMapping[edge]);
            edgeMapping.Remove(edge);

            UnassociateNodeWithEdge(edge.First, edge);
            UnassociateNodeWithEdge(edge.Second, edge);
        }
    }
}
