using System.Collections.Generic;

namespace ConstraintThingyGUI
{
    class UndirectedEdge
    {
        public UndirectedEdge(Node first, Node second)
        {
            First = first;
            Second = second;
        }

        public Node First { get; private set; }

        public Node Second { get; private set; }

        public bool Contains(Node node)
        {
            return EqualityComparer<Node>.Default.Equals(node, First) || EqualityComparer<Node>.Default.Equals(node, Second);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (UndirectedEdge)) return false;
            return Equals((UndirectedEdge) obj);
        }

        public bool Equals(UndirectedEdge other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.First, First) && Equals(other.Second, Second);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((First != null ? First.GetHashCode() : 0)*397) ^ (Second != null ? Second.GetHashCode() : 0);
            }
        }
    }
}