namespace Intervals
{
    /// <summary>
    /// An axis-aligned bounding box.
    /// </summary>
    public struct AABB
    {
        /// <summary>
        /// The upper-left corner of the AABB
        /// </summary>
        public Vector2 UpperLeft { get; set; }

        /// <summary>
        /// The upper-right corner of the AABB
        /// </summary>
        public Vector2 UpperRight { get { return new Vector2(LowerRight.X, UpperLeft.Y);} }

        /// <summary>
        /// The lower-left corner of the AABB
        /// </summary>
        public Vector2 LowerLeft { get { return new Vector2(UpperLeft.X, LowerRight.Y); } }

        /// <summary>
        /// The lower-right corner of the AABB
        /// </summary>
        public Vector2 LowerRight { get; set; }

        /// <summary>
        /// The center point of the AABB
        /// </summary>
        public Vector2 Center { get { return UpperLeft + Size * 0.5f; } }

        /// <summary>
        /// The width of the AABB
        /// </summary>
        public float Width { get { return LowerRight.X - UpperLeft.X; } }

        /// <summary>
        /// The Height of the AABB
        /// </summary>
        public float Height { get { return LowerRight.Y - UpperLeft.Y; } }

        /// <summary>
        /// True if the struct represents a valid AABB.
        /// </summary>
        public bool IsValid { get { return LowerRight.X >= UpperLeft.X && LowerRight.Y >= UpperLeft.Y; } }

        /// <summary>
        /// Returns a vector of {Width, Height}
        /// </summary>
        public Vector2 Size { get { return new Vector2(Width, Height);} }

        /// <summary>
        /// Creates a new AABB with the provided center and the specified width and height.
        /// </summary>
        public AABB(Vector2 center, float width, float height) : this()
        {
            Vector2 halfSize = new Vector2(width * 0.5f, height * 0.5f);
            UpperLeft = center - halfSize;
            LowerRight = center + halfSize;
        }
        
        /// <summary>
        /// Creates a new AABB from the provided upper left and lowe
        /// </summary>
        /// <param name="upperLeft"></param>
        /// <param name="lowerRight"></param>
        public AABB(Vector2 upperLeft, Vector2 lowerRight) : this()
        {
            LowerRight = lowerRight;
            UpperLeft = upperLeft;
        }
    }
}
