//#define fancy
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
            //filePath = "c:/users/ian/desktop/quad.csv";
            ReloadGraph();
        }

        private void ReloadGraph()
        {
            Variable.ResetStatistics();
            Variable.ResetVariableSystemForTesting();
            Labeling.ResetLabelings();
            var graph = UndirectedGraph.FromSpreadsheet(filePath, 200);

            graphCanvas.Graph = UndirectedGraph.CurrentGraph = graph;
            //graph.WriteASPFormat("c:/users/ian/desktop/residentevil.lp", "health", "ammo", "lock");

#if fancy
            var contentType = new FiniteDomain("empty", "small-health-pack", "big-health-pack", "small-ammo", "big-ammo", "zombie", "two-zombies", "dog", "boss", "trap", "locked-door");
            var contents = new FiniteDomainLabeling("contents", 
                                                    contentType);
            EasyConstraints(contents);
            //HardForMansionConstraints(contents);
            //HardForQuadConstraints(contents);

            var healthDelta = new ScoreLabeling("health delta", contents,
                                                0,
                                                "small-health-pack", 10, "big-health-pack", 20,
                                                "zombie", -10, "two-zombies", -25, "dog", -13, "trap", -10, "boss", -30);

            var totalHealth = new StartEndPathLabeling("health", healthDelta, graph, 20, graph.FindNode("N24"), graph.FindNode("N12"));

            var ammoDelta = new ScoreLabeling("ammo delta", contents,
                                                0,
                                                "small-ammo", 6, "big-ammo", 12,
                                                "zombie", -1, "two-zombies", -2, "dog", -2, "boss", -8);

            var totalAmmo = new StartEndPathLabeling("ammo", ammoDelta, graph, 5, graph.FindNode("N24"), graph.FindNode("N12"));

            var lockDelta = new ScoreLabeling("lock delta", contents,
                                                0,
                                                "locked-door", -1, "boss", 2);

            var totalLock = new StartEndPathLabeling("lock", lockDelta, graph, 0, graph.FindNode("N24"), graph.FindNode("N12"));
#else
            var contentType = new FiniteDomain("empty", "big-health-pack", "zombie", "two-zombies");
            var contents = new FiniteDomainLabeling("contents",
                                                    contentType);
            var healthDelta = new ScoreLabeling("health delta", contents,
                                                0,
                                                "big-health-pack", 20,
                                                "zombie", -10, "two-zombies", -25);

            contents.LimitOccurences("big-health-pack", 1, 7);
            contents.LimitOccurences("zombie", 1, 5);
            contents.LimitOccurences("two-zombies", 1, 3);

            var totalHealth = new StartEndPathLabeling("health", healthDelta, graph, 20, graph.FindNode("N24"), graph.FindNode("N12"));
#endif

            bool succeeded = true;
            foreach (var n in graph.Nodes)
                if (n.SupportRecipient == null)
                {
                    totalHealth.ValueVariable(n).NarrowTo(new Interval(1, float.MaxValue), ref succeeded);
#if fancy
                    totalAmmo.ValueVariable(n).NarrowTo(new Interval(0, float.MaxValue), ref succeeded);
                    totalLock.ValueVariable(n).NarrowTo(new Interval(0, 2f), ref succeeded);
#endif
                }
#if fancy
                else
                    contents.ValueVariable(n).TrySetValue(contentType.UniverseMask & ~(contentType.Bitmask("boss")|contentType.Bitmask("locked-door")), ref succeeded);

            totalLock[graph.FindNode("N12")] = 1;
#endif
            if (!succeeded)
                throw new Exception("Initialization of label variables failed.");

            solutionIterator = Variable.SolutionsAllVariables().GetEnumerator();
            graphCanvas.UpdateText();
        }

        private static void EasyConstraints(FiniteDomainLabeling contents)
        {
            contents.LimitOccurences("small-health-pack", 0, 0);
            contents.LimitOccurences("big-health-pack", 1, 40);
            contents.LimitOccurences("small-ammo", 0, 0);
            contents.LimitOccurences("big-ammo", 1, 40);
            contents.LimitOccurences("zombie", 1, 40);
            contents.LimitOccurences("two-zombies", 1, 40);
            contents.LimitOccurences("dog", 1, 40);
            contents.LimitOccurences("boss", 1, 1);
            contents.LimitOccurences("trap", 1, 2);
            contents.LimitOccurences("locked-door", 1, 1);
        }

        private static void HardForMansionConstraints(FiniteDomainLabeling contents)
        {
            contents.LimitOccurences("small-health-pack", 0, 0);
            contents.LimitOccurences("big-health-pack", 0, 10);
            contents.LimitOccurences("small-ammo", 0, 0);
            contents.LimitOccurences("big-ammo", 0, 10);
            contents.LimitOccurences("zombie", 3);
            contents.LimitOccurences("two-zombies", 2);
            contents.LimitOccurences("dog", 2);
            contents.LimitOccurences("boss", 1);
            contents.LimitOccurences("trap", 1);
            contents.LimitOccurences("locked-door", 1);
        }

        private static void HardForQuadConstraints(FiniteDomainLabeling contents)
        {
            contents.LimitOccurences("small-health-pack", 3, 10);
            contents.LimitOccurences("big-health-pack", 3, 10);
            contents.LimitOccurences("small-ammo", 3, 10);
            contents.LimitOccurences("big-ammo", 3, 10);
            contents.LimitOccurences("zombie", 5);
            contents.LimitOccurences("two-zombies", 5);
            contents.LimitOccurences("dog", 5);
            contents.LimitOccurences("boss", 1);
            contents.LimitOccurences("trap", 2);
            contents.LimitOccurences("locked-door", 1);
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
            Variable.SolutionsAllVariables().GetEnumerator().MoveNext();
            timer.Stop();
            //solutionTime.Content = string.Format("{0}ms; time for exception={1}", timer.ElapsedMilliseconds, timeForException);
            solutionTime.Content = string.Format("{0}ms", timer.ElapsedMilliseconds);
            graphCanvas.UpdateText();
            Cursor = Cursors.Arrow;
        }

        void AllSolutions()
        {
            Cursor = Cursors.Wait;
            GC.Collect(0);
            GC.Collect(1);
            int collections = GC.CollectionCount(0);
            timer.Reset();
            timer.Start();
            int solutions = 0;
            while (solutions<10000)
            {
                if (!Variable.SolutionsAllVariables().GetEnumerator().MoveNext())
                    Debugger.Break();
                solutions++;
            }
            timer.Stop();
            solutionTime.Content = string.Format("{0} solutions, {1}ms, mean={2}\nTotal variables {3}\nTotal backtracks {4}, maximum undo stack depth {5}\n{6} collections at generation 0", 
                solutions, 
                timer.ElapsedMilliseconds, 
                ((double)timer.ElapsedMilliseconds) / solutions,
                Variable.TotalVariables,
                Variable.TotalBacktracks,
                Variable.MaxUndoStackDepth,
                GC.CollectionCount(0)-collections);
            graphCanvas.UpdateText();
            Cursor = Cursors.Arrow;
        }

        void ChooseFile()
        {
            OpenFileDialog d = new OpenFileDialog {Filter = "CSV files (*.csv)|*.csv"};
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
