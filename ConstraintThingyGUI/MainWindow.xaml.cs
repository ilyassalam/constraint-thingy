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

            var node1 = new Node()
                            {
                                AABB = new AABB(new Vector2(0, 0), new Vector2(100, 100))
                            };

            graph.AddNode(node1);

            var node2 = new Node()
                            {
                                AABB = new AABB(new Vector2(125, 125), new Vector2(200, 200))
                            };

            graph.AddNode(node2);

            var edge = new UndirectedEdge(node1, node2);

            graph.AddEdge(edge);
            
            graphCanvas.Graph = graph;
        }
        
    }
}
