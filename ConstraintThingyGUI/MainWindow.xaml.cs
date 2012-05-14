using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using ConstraintThingy;
using Intervals;
using Microsoft.Win32;

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

            //var graph = new UndirectedGraph();

            //var hub = new Node("hub", new AABB(new Vector2(200, 0), new Vector2(300, 100)));
            //graph.AddNode(hub);

            //for (int x = 0; x < 600; x += 110)
            //{
            //    var n = new Node(new AABB(new Vector2(x, 125), new Vector2(x+100, 125+100)));
            //    graph.AddNode(n);
            //    graph.AddEdge(new UndirectedEdge(hub, n));
            //}

            filePath = "c:/users/ian/desktop/residentevil.csv";
            ReloadGraph();
        }

        private void ReloadGraph()
        {
            Variable.ResetStatistics();
            Variable.ResetVariableSystemForTesting();
            var graph = UndirectedGraph.FromSpreadsheet(filePath, 200);

            graphCanvas.Graph = UndirectedGraph.CurrentGraph = graph;
            var type = new FiniteDomainLabeling("type", new FiniteDomain("hub", "forest", "swamp", "cave", "other"));
            type.LimitOccurences("hub", 1, 1);
            type.LimitOccurences("forest", 1, 1);
            type.LimitOccurences("swamp", 1, 1);
            type.LimitOccurences("cave", 1, 1);
            var contents = new FiniteDomainLabeling("contents",
                                                    new FiniteDomain("Big monster", "Little monster", "Health pack",
                                                                     "empty"));
            contents[graph.FindNode("N1")] = "empty";
            contents.LimitOccurences("Big monster", 1, 3);
            contents.LimitOccurences("Little monster", 1, 5);
            contents.LimitOccurences("Health pack", 1, 3);
            var score = new ScoreLabeling("health delta", contents, 0, "Big monster", -10, "Little monster", -5,
                                          "Health pack", 10);
            var totalHealth = new StartEndPathLabeling("health", score, graph, 10, graph.FindNode("N1"), graph.FindNode("N10"));
            foreach (var n in graph.Nodes)
                totalHealth.ValueVariable(n).NarrowTo(new Interval(1, float.MaxValue));

            solutionIterator = Variable.SolutionsAllVariables().GetEnumerator();
            graphCanvas.UpdateText();
        }

        readonly Stopwatch timer = new Stopwatch();
        void NextSolution()
        {
            Cursor = Cursors.Wait;
            //timer.Reset();
            //timer.Start();
            //for (int i = 0; i < 1000000; i++)
            //{
            //    try
            //    {
            //        throw new Exception();
            //    }
            //    catch (Exception e)
            //    {
            //    }
            //}
            //timer.Stop();
            //double timeForException = timer.ElapsedMilliseconds/1000000.0;
            timer.Reset();
            timer.Start();
            solveButton.IsEnabled = solutionIterator.MoveNext();
            timer.Stop();
            //solutionTime.Content = string.Format("{0}ms; time for exception={1}", timer.ElapsedMilliseconds, timeForException);
            solutionTime.Content = string.Format("{0}ms", timer.ElapsedMilliseconds);
            graphCanvas.UpdateText();
            Cursor = Cursors.Arrow;
        }

        void AllSolutions()
        {
            Cursor = Cursors.Wait;
            System.GC.Collect(0);
            int collections = System.GC.CollectionCount(0);
            timer.Reset();
            timer.Start();
            int solutions = 0;
            while (solutionIterator.MoveNext()) solutions++;
            timer.Stop();
            solutionTime.Content = string.Format("{0} solutions, {1}ms, mean={2}\nTotal variables {3}\nTotal backtracks {4}\nMaximum undo stack depth {5}\n{6} collections at generation 0", 
                solutions, 
                timer.ElapsedMilliseconds, 
                ((double)timer.ElapsedMilliseconds) / solutions,
                Variable.TotalVariables,
                Variable.TotalBacktracks,
                Variable.MaxUndoStackDepth,
                System.GC.CollectionCount(0)-collections);
            graphCanvas.UpdateText();
            Cursor = Cursors.Arrow;
        }

        void ChooseFile()
        {
            OpenFileDialog d = new OpenFileDialog();
            d.Filter = "CSV files (*.csv)|*.csv";
            d.ShowDialog();
            filePath = d.FileName;
            ReloadGraph();
        }

        private IEnumerator<bool> solutionIterator;
        private string filePath;

        private void solveButton_Click(object sender, RoutedEventArgs e)
        {
            NextSolution();
        }

        private void solveAllButton_Click(object sender, RoutedEventArgs e)
        {
            AllSolutions();
        }

        private void selectFileButton_Click(object sender, RoutedEventArgs e)
        {
            ChooseFile();
        }
        
    }
}
