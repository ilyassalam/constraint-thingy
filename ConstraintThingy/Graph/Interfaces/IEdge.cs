namespace ConstraintThingy
{
    /// <summary>
    /// Represents a directional link from one node to another
    /// </summary>
    public interface IEdge
    {
        /// <summary>
        /// The source of the connection
        /// </summary>
        INode From { get; }

        /// <summary>
        /// The destination of the connection
        /// </summary>
        INode To { get; }
    }
}