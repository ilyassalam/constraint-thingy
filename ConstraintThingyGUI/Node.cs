using Intervals;

namespace ConstraintThingyGUI
{
    /// <summary>
    /// A node in the graph representing the dungeon
    /// </summary>
    public class Node
    {
        /// <summary>
        /// The bounding box of the node on the screen
        /// </summary>
        public AABB AABB { get; set; }
    }
}