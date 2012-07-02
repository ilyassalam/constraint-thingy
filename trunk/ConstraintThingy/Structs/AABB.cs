using ConstraintThingy;

namespace ConstraintThingy
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
        public Vector2 UpperRight { get { return new Vector2(LowerRight.X, UpperLeft.Y); } }

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
        public double Width { get { return LowerRight.X - UpperLeft.X; } }

        /// <summary>
        /// The Height of the AABB
        /// </summary>
        public double Height { get { return LowerRight.Y - UpperLeft.Y; } }

        /// <summary>
        /// True if the struct represents a valid AABB.
        /// </summary>
        public bool IsValid { get { return LowerRight.X >= UpperLeft.X && LowerRight.Y >= UpperLeft.Y; } }

        /// <summary>
        /// Returns a vector of {Width, Height}
        /// </summary>
        public Vector2 Size { get { return new Vector2(Width, Height); } }

        /// <summary>
        /// Creates a new AABB with the provided center and the specified width and height.
        /// </summary>
        public AABB(Vector2 center, double width, double height)
            : this()
        {
            Vector2 halfSize = new Vector2(width * 0.5, height * 0.5);
            UpperLeft = center - halfSize;
            LowerRight = center + halfSize;
        }

        /// <summary>
        /// Creates a new AABB from the provided upper left and lowe
        /// </summary>
        public AABB(Vector2 upperLeft, Vector2 lowerRight)
            : this()
        {
            LowerRight = lowerRight;
            UpperLeft = upperLeft;
        }

        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <returns>
        /// true if <paramref name="obj"/> and this instance are the same type and represent the same value; otherwise, false.
        /// </returns>
        /// <param name="obj">Another object to compare to. </param><filterpriority>2</filterpriority>
        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        /// <summary>
        /// True if equal to the other AABB
        /// </summary>
        public bool Equals(AABB other)
        {
            return other.UpperLeft.Equals(UpperLeft) && other.LowerRight.Equals(LowerRight);
        }

        /// <summary>
        /// True if <paramref name="a"/> and <paramref name="b"/> are equal.
        /// </summary>
        public static bool operator==(AABB a, AABB b)
        {
            return a.Equals(b);
        }

        /// <summary>
        /// True if <paramref name="a"/> and <paramref name="b"/> are not equal
        /// </summary>
        public static bool operator !=(AABB a, AABB b)
        {
            return !(a == b);
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>
        /// A 32-bit signed integer that is the hash code for this instance.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override int GetHashCode()
        {
            unchecked
            {
                return (UpperLeft.GetHashCode()*397) ^ LowerRight.GetHashCode();
            }
        }
    }
}