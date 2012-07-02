namespace ConstraintThingy
{
    /// <summary>
    /// Represents a connection between two nodes
    /// </summary>
    public class Edge : IEdge
    {
        /// <summary>
        /// The source of the connection
        /// </summary>
        public INode From { get; private set; }

        /// <summary>
        /// The destination of the connection
        /// </summary>
        public INode To { get; private set; }

        /// <summary>
        /// Creates a new edge
        /// </summary>
        public Edge(INode from, INode to)
        {
            From = from;
            To = to;
        }
    }
}