using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using ConstraintThingy;
using Intervals;

namespace ConstraintThingyGUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        /// <summary>
        /// The main window for the GUI
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            var graph = new UndirectedGraph();

            var hub = new Node("hub", new AABB(new Vector2(200, 0), new Vector2(300, 100)));
            graph.AddNode(hub);

            for (int x = 0; x < 600; x += 110)
            {
                var n = new Node(new AABB(new Vector2(x, 125), new Vector2(x+100, 125+100)));
                graph.AddNode(n);
                graph.AddEdge(new UndirectedEdge(hub, n));
            }
            
            graphCanvas.Graph = UndirectedGraph.CurrentGraph = graph;
            var type = new FiniteDomainLabeling("type", new FiniteDomain("hub", "forest", "swamp", "cave", "other"));
            type.LimitOccurences("hub", 1, 1);
            type.LimitOccurences("forest", 1, 1);
            type.LimitOccurences("swamp", 1, 1);
            type.LimitOccurences("cave", 1, 1);
            var contents = new FiniteDomainLabeling("contents",
                                                    new FiniteDomain("Big monster", "Little monster", "Health pack",
                                                                     "empty"));
            contents.LimitOccurences("Big monster", 1, 1);
            contents.LimitOccurences("Little monster", 1, 1);
            contents.LimitOccurences("Health pack", 1, 1);
            var score = new ScoreLabeling("health delta", contents, 0, "Big monster", -10, "Little monster", -5, 
                "Health pack", 10);
            var totalHealth = new StartEndPathLabeling("health", score, graph, 10, graph.Nodes[1], graph.Nodes[2]);
            totalHealth.ValueVariable(graph.Nodes[2]).NarrowTo(new Interval(1, float.MaxValue));

            solutionIterator = Variable.SolutionsAllVariables().GetEnumerator();
            graphCanvas.UpdateText();
        }

        readonly Stopwatch timer = new Stopwatch();
        void NextSolution()
        {
            Cursor = Cursors.Wait;
            timer.Reset();
            timer.Start();
            solveButton.IsEnabled = solutionIterator.MoveNext();
            timer.Stop();
            solutionTime.Content = string.Format("{0}ms", timer.ElapsedMilliseconds);
            graphCanvas.UpdateText();
            Cursor = Cursors.Arrow;
        }

        private readonly IEnumerator<bool> solutionIterator;

        private void solveButton_Click(object sender, RoutedEventArgs e)
        {
            NextSolution();
        }
        
    }
}
