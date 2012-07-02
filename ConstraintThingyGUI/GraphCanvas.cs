using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using CSharpUtils.Collections;
using ConstraintThingy;
using ConstraintThingyGUI;

namespace ConstraintThingyGUI
{
    class GraphCanvas : Canvas
    {
        private IGraph _graph;
        public IGraph Graph
        {
            get { return _graph; }
            set
            {
                if (_graph != null) DetachGraph();

                _graph = value;

                if (_graph != null) AttachGraph();
            }
        }

        private void DetachGraph()
        {
            foreach (var node in Graph.Nodes)
            {
                RemoveNode(node);
            }

            foreach (var edge in Graph.Edges)
            {
                RemoveEdge(edge);
            }

            Graph.NodeAdded -= AddNode;
            Graph.EdgeAdded -= AddEdge;

            Graph.NodeRemoved -= RemoveNode;
            Graph.EdgeRemoved -= RemoveEdge;
        }

        private void AttachGraph()
        {
            foreach (var node in Graph.Nodes)
            {
                AddNode(node);
            }

            foreach (var edge in Graph.Edges)
            {
                AddEdge(edge);
            }

            Graph.NodeAdded += AddNode;
            Graph.EdgeAdded += AddEdge;

            Graph.NodeRemoved += RemoveNode;
            Graph.EdgeRemoved += RemoveEdge;
        }

        private readonly Dictionary<INode, Shape> _nodesToShapes = new Dictionary<INode, Shape>();

        private readonly Dictionary<IEdge, Line> _edgesToLines = new Dictionary<IEdge, Line>(); 

        private readonly Dictionary<INode, Action> _detachmentActions = new Dictionary<INode, Action>();

        private void RegisterDetachmentAction(INode node, Action action)
        {
            Action detachmentAction;
            if (_detachmentActions.TryGetValue(node, out detachmentAction))
            {
                detachmentAction += action;
            }

            _detachmentActions[node] = detachmentAction;
        }

        private void AddNode(INode node)
        {
            Shape shape;

            if (node is AABBNode)
            {
                var aabbNode = node as AABBNode;

                shape = new Rectangle()
                            {
                                Width = aabbNode.AABB.Width,
                                Height = aabbNode.AABB.Height,

                            };

                Action<Vector2> sizeChanged = vector2 =>
                                                  {
                                                      shape.Width = vector2.X;
                                                      shape.Height = vector2.Y;
                                                  };

                aabbNode.SizeChanged += sizeChanged;

                RegisterDetachmentAction(aabbNode, () => aabbNode.SizeChanged -= sizeChanged);
            }
            else if (node is CircleNode)
            {
                var circleNode = node as CircleNode;

                shape = new Ellipse()
                            {
                                Width = circleNode.Radius * 2,
                                Height = circleNode.Radius * 2
                            };

                Action<double> radiusChanged = r =>
                                                   {
                                                       shape.Width = r * 2;
                                                       shape.Height = r * 2;
                                                   };

                circleNode.RadiusChanged += radiusChanged;

                RegisterDetachmentAction(circleNode, () => circleNode.RadiusChanged -= radiusChanged);
            }
            else throw new NotImplementedException();

            shape.RenderTransform = new TranslateTransform(node.Position.X - shape.Width / 2, node.Position.Y - shape.Height / 2);
            shape.Fill = Brushes.SlateGray;
            shape.StrokeThickness = 5;

            Action<Vector2> positionChanged = vector2 =>
                                                  {
                                                      shape.RenderTransform = new TranslateTransform(node.Position.X - shape.Width / 2, node.Position.Y - shape.Height / 2);
                                                  };

            node.PositionChanged += positionChanged;

            RegisterDetachmentAction(node, ()=> node.PositionChanged -= positionChanged);

            _nodesToShapes.Add(node, shape);

            SetUpDragShape(node, shape);

            Children.Add(shape);
        }

        static readonly double MinimumHorizontalDragDistance = SystemParameters.MinimumHorizontalDragDistance / 4;
        static readonly double MinimumVerticalDragDistance = SystemParameters.MinimumVerticalDragDistance / 4;

        enum NodeState { Clicked, Dragging, Unclicked }

        private INode _lastClicked;

        public event Action<INode> OnNodeSelected;

        public void SelectNode(INode node)
        {
            DeselectNode();

            _lastClicked = node;
            _nodesToShapes[node].Stroke = Brushes.CornflowerBlue;

            if (OnNodeSelected != null) OnNodeSelected(node);
        }

        private void DeselectNode()
        {
            if (_lastClicked != null)
            {
                _nodesToShapes[_lastClicked].Stroke = Brushes.Transparent;   
            }
            _lastClicked = null;
        }

        private void SetUpDragShape(INode node, Shape shape)
        {
            NodeState state = NodeState.Unclicked;

            Vector2 elementStartPosition = new Vector2();
            Point startPosition = new Point();

            MouseButtonEventHandler mouseLeftDown = (o, args) =>
                                                        {
                                                            state = NodeState.Clicked;
                                                            shape.CaptureMouse();
                                                            
                                                            if (_lastClicked != null && _lastClicked != node)
                                                            {
                                                                if (!_lastClicked.Neighbors.Contains(node))
                                                                {
                                                                    _lastClicked.AddEdge(new Edge(_lastClicked, node));
                                                                    node.AddEdge(new Edge(node, _lastClicked));   
                                                                }
                                                            }

                                                            SelectNode(node);
                                                            
                                                            elementStartPosition = node.Position;
                                                            startPosition = args.GetPosition(this);
                                                        };

            MouseButtonEventHandler mouseUp = (o, args) =>
                                                    {
                                                        state = NodeState.Unclicked;
                                                        shape.ReleaseMouseCapture();
                                                    };

            MouseEventHandler mouseMove = (o, args) =>
                                              {
                                                  Point currentPosition = args.GetPosition(this);

                                                  Vector difference = currentPosition - startPosition;

                                                  if (state == NodeState.Clicked)
                                                  {
                                                      if (Math.Abs(difference.X) >= MinimumHorizontalDragDistance && Math.Abs(difference.Y) >= MinimumVerticalDragDistance)
                                                      {
                                                          state = NodeState.Dragging;
                                                      }
                                                  }

                                                  if (state == NodeState.Dragging)
                                                  {
                                                      DeselectNode();
                                                      node.Position = new Vector2(elementStartPosition.X + difference.X, elementStartPosition.Y + difference.Y);
                                                  }
                                              };

            shape.MouseLeftButtonDown += mouseLeftDown;
            shape.MouseMove += mouseMove;
            shape.MouseLeftButtonUp += mouseUp;

            RegisterDetachmentAction(node, () =>
                                               {
                                                   shape.MouseLeftButtonDown -= mouseLeftDown;
                                                   shape.MouseMove -= mouseMove;
                                                   shape.MouseLeftButtonUp -= mouseUp;
                                               });
        }

        private void RemoveNode(INode node)
        {
            Action action;
            if (_detachmentActions.TryGetValue(node, out action))
            {
                action();
                _detachmentActions.Remove(node);
            }

            Children.Remove(_nodesToShapes[node]);

            _nodesToShapes.Remove(node);
        }

        private void AddEdge(IEdge edge)
        {
            Line line = new Line()
                            {
                                StrokeThickness = 2,
                                Stroke = Brushes.Black,

                                X1 = edge.From.Position.X,
                                Y1 = edge.From.Position.Y,
                                
                                X2 = edge.To.Position.X,
                                Y2 = edge.To.Position.Y,
                            };

            SetZIndex(line, -1);

            edge.From.PositionChanged += vector2 =>
                                             {
                                                 line.X1 = vector2.X;
                                                 line.Y1 = vector2.Y;
                                             };

            edge.To.PositionChanged += vector2 =>
                                           {
                                               line.X2 = vector2.X;
                                               line.Y2 = vector2.Y;
                                           };

            _edgesToLines.Add(edge, line);

            Children.Add(line);
        }

        private void RemoveEdge(IEdge edge)
        {
            Children.Remove(_edgesToLines[edge]);

            _edgesToLines.Remove(edge);
        }

        public GraphCanvas()
        {
            Graph = new Graph();

            ClipToBounds = true;
            DragEnter += OnDragEnter;
            Drop += OnDrop;
        }

        private void OnDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DragableAreaTypeListItem.AreaTypeDataFormat))
            {
                Point position = e.GetPosition(this);

                var data = e.Data.GetData(DragableAreaTypeListItem.AreaTypeDataFormat);

                AreaType areaType = (AreaType) data;

                Node node;

                switch (areaType)
                {
                    case AreaType.Rectangle:
                        node = new AABBNode
                                   {
                                       AABB = new AABB(new Vector2(position.X, position.Y), 50, 50)
                                   };
                        break;
                    case AreaType.Circle:
                        node = new CircleNode()
                                   {
                                       Position = new Vector2(position.X, position.Y),
                                       Radius = 25
                                   };
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                
                Graph.AddNode(node);
            }
        }

        private void OnDragEnter(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(DragableAreaTypeListItem.AreaTypeDataFormat))
            {
                e.Effects = DragDropEffects.None;
            }
        }
    }
}
