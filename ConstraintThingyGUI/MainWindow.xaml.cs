using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ConstraintThingy;
using Intervals;

namespace ConstraintThingyGUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
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

            for (int x = 0; x < 500; x += 50)
            {
                var n = new Node(new AABB(new Vector2(x, 125), new Vector2(x+40, 125+40)));
                graph.AddNode(n);
                graph.AddEdge(new UndirectedEdge(hub, n));
            }
            
            graphCanvas.Graph = UndirectedGraph.CurrentGraph = graph;
            var type = new FiniteDomainLabeling("type", new FiniteDomain("hub", "forest", "swamp", "cave", "other"));
            type.LimitOccurences("hub", 1, 1);
            type.LimitOccurences("forest", 1, 1);
            type.LimitOccurences("swamp", 1, 1);
            type.LimitOccurences("cave", 1, 1);
            var solutionIterator = Variable.Solutions().GetEnumerator();
            solutionIterator.MoveNext();
        }
        
    }
}
