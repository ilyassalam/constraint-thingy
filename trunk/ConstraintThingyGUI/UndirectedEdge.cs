namespace ConstraintThingyGUI
{
    /// <summary>
    /// Represents an edge of an undirected graph
    /// </summary>
    public class UndirectedEdge
    {
        /// <summary>
        /// Represents an edge of an undirected graph
        /// </summary>
        public UndirectedEdge(Node first, Node second)
        {
            First = first;
            Second = second;
        }

        /// <summary>
        /// One of the nodes of the edge
        /// </summary>
        public Node First { get; private set; }

        /// <summary>
        /// The other node of the edge.
        /// </summary>
        public Node Second { get; private set; }

        /// <summary>
        /// True if the edge involves NODE
        /// </summary>
        public bool Contains(Node node)
        {
            // I think this is only necessary for generic methods, right?  -ian
            //return EqualityComparer<Node>.Default.Equals(node, First) || EqualityComparer<Node>.Default.Equals(node, Second);
            return node == First || node == Second;
        }

        /// <summary>
        /// Tests if this is the same as OBJ
        /// </summary>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (UndirectedEdge)) return false;
            return Equals((UndirectedEdge) obj);
        }

        /// <summary>
        /// Tests if edge is equivalent to OTHER
        /// </summary>
        public bool Equals(UndirectedEdge other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.First, First) && Equals(other.Second, Second);
        }

        /// <summary>
        /// Computes a hash for the edge
        /// </summary>
        public override int GetHashCode()
        {
            unchecked
            {
                return ((First != null ? First.GetHashCode() : 0)*397) ^ (Second != null ? Second.GetHashCode() : 0);
            }
        }
    }
}