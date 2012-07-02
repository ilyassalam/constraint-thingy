namespace Intervals
{
    /// <summary>
    /// A pair of coordinates
    /// </summary>
    public struct Vector2
    {
        /// <summary>
        /// The X coordinate of the vector
        /// </summary>
        public float X;

        /// <summary>
        /// The Y coordinate of the vector
        /// </summary>
        public float Y;

        /// <summary>
        /// Creates a new Vector2 struct from the provided X and Y coordinates
        /// </summary>
        public Vector2(float x, float y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// Adds two vectors
        /// </summary>
        public static Vector2 Add(Vector2 a, Vector2 b)
        {
            return new Vector2(a.X + b.X, a.Y + b.Y);
        }

        /// <summary>
        /// Subtracts vector <paramref name="a"/> from vector <paramref name="b"/>
        /// </summary>
        public static Vector2 Subtract(Vector2 a, Vector2 b)
        {
            return new Vector2(a.X - b.X, a.Y - b.Y);
        }

        /// <summary>
        /// Adds two vectors
        /// </summary>
        public static Vector2 operator +(Vector2 a, Vector2 b)
        {
            return Add(a, b);
        }

        /// <summary>
        /// Subtracts vector <paramref name="a"/> from vector <paramref name="b"/>
        /// </summary>
        public static Vector2 operator -(Vector2 a, Vector2 b)
        {
            return Subtract(a, b);
        }

        /// <summary>
        /// Multiplies a vector by a scalar.
        /// </summary>
        public static Vector2 Multiply(Vector2 vector, float scalar)
        {
            return new Vector2(vector.X * scalar, vector.Y * scalar);
        }

        /// <summary>
        /// Multiplies a vector by a scalar.
        /// </summary>
        public static Vector2 operator *(Vector2 vector, float scalar)
        {
            return Multiply(vector, scalar);
        }
    }
}