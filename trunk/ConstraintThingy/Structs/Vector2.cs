namespace ConstraintThingy
{
    /// <summary>
    /// A pair of values
    /// </summary>
    public struct Vector2
    {
        /// <summary>
        /// The X coordinate of the vector
        /// </summary>
        public double X;

        /// <summary>
        /// The Y coordinate of the vector
        /// </summary>
        public double Y;

        /// <summary>
        /// Creates a new Vector2 struct from the provided X and Y coordinates
        /// </summary>
        public Vector2(double xy)
        {
            X = xy;
            Y = xy;
        }

        /// <summary>
        /// Creates a new Vector2 struct from the provided X and Y coordinates
        /// </summary>
        public Vector2(double x, double y)
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
        /// Multiplies a vector by a scalar.
        /// </summary>
        public static Vector2 Multiply(Vector2 vector, double scalar)
        {
            return new Vector2(vector.X * scalar, vector.Y * scalar);
        }

        /// <summary>
        /// Divides a vector by a scalar.
        /// </summary>
        public static Vector2 Divide(Vector2 vector, double scalar)
        {
            return new Vector2(vector.X / scalar, vector.Y / scalar);
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
        public static Vector2 operator *(Vector2 vector, double scalar)
        {
            return Multiply(vector, scalar);
        }

        /// <summary>
        /// Divides a vector by a scalar.
        /// </summary>
        public static Vector2 operator /(Vector2 vector, double scalar)
        {
            return Divide(vector, scalar);
        }

        /// <summary>
        /// True if <paramref name="a"/> and <paramref name="b"/> are equal.
        /// </summary>
        public static bool operator==(Vector2 a, Vector2 b)
        {
            return a.Equals(b);
        }


        /// <summary>
        /// True if <paramref name="a"/> and <paramref name="b"/> are not equal.
        /// </summary>
        public static bool operator !=(Vector2 a, Vector2 b)
        {
            return !(a == b);
        }

        /// <summary>
        /// True when this vector equals the other
        /// </summary>
        public bool Equals(Vector2 other)
        {
            return other.X.Equals(X) && other.Y.Equals(Y);
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
                return (X.GetHashCode()*397) ^ Y.GetHashCode();
            }
        }
    }
}