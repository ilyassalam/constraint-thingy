using System;
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
        /// Nodes that are in a cul-de-sac connected to this node.
        /// Assumes this node is *not* in a cul-de-sc
        /// </summary>
        public readonly List<Node> Support = new List<Node>();

        /// <summary>
        /// If this node is in a cul-de-sac, this is the node that connects the cul-de-sac to the rest of the graph.
        /// If this node is not on a cul-de-sac, this must be set to null.
        /// </summary>
        public Node SupportRecipient { get; private set; }

        /// <summary>
        /// Declares that this node provides support to RECIPIENT.
        /// </summary>
        public void SetSupport(Node recipient)
        {
            if (recipient.SupportRecipient != null)
                throw new Exception(string.Format("Node {0} set to support {1}, which already supports {2}", Name, recipient.Name, recipient.SupportRecipient.Name));
            SupportRecipient = recipient;
            recipient.Support.Add(this);
        }

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