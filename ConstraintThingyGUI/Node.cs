using System.Collections.Generic;
using Intervals;

namespace ConstraintThingyGUI
{
    /// <summary>
    /// A node in the graph representing the dungeon
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("{Name}")]
    public class Node
    {
        /// <summary>
        /// The bounding box of the node on the screen
        /// </summary>
        public AABB AABB { get; set; }

        /// <summary>
        /// Name of the node (for debugging purposes)
        /// </summary>
        public string Name { get; private set; }

        private static int uidCounter;
        static string MakeName()
        {
            return string.Format("N{0}", uidCounter++);
        }

        /// <summary>
        /// Makes a node with the specified name and bounding box.
        /// </summary>
        public Node(string name, AABB aabb)
        {
            Name = name;
            AABB = aabb;
        }

        /// <summary>
        /// Makes a node with the specified name and bounding box.
        /// </summary>
        public Node(AABB aabb)
            : this(MakeName(), aabb)
        {}

        readonly List<Node> neigbors = new List<Node>();

        /// <summary>
        /// All neighboring nodes of this node.
        /// </summary>
        public IList<Node> Neighbors
        {
            get { return neigbors;  }
        }

        internal void AddNeighbor(Node n)
        {
            neigbors.Add(n);
        }

        internal void RemoveNeighbor(Node n)
        {
            neigbors.Remove(n);
        }
    }
}