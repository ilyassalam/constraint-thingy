namespace CSharpUtils.Collections
{
    /// <summary>
    /// Represents a pair of objects.
    /// </summary>
    /// <typeparam name="TFirst">The type of the first object</typeparam>
    /// <typeparam name="TSecond">The type of the second object</typeparam>
    public struct Pair<TFirst, TSecond>
    {
        /// <summary>
        /// The first object
        /// </summary>
        public TFirst First { get; set; }

        /// <summary>
        /// The second object
        /// </summary>
        public TSecond Second { get; set; }

        /// <summary>
        /// Creates a pair of objects
        /// </summary>
        /// <param name="first">The first object</param>
        /// <param name="second">The second object</param>
        public Pair(TFirst first, TSecond second)
            : this()
        {
            First = first;
            Second = second;
        }
    }
}