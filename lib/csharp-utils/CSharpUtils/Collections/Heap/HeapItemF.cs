namespace CSharpUtils.Collections
{
    /// <summary>
    /// An {item, priorty} pair.
    /// </summary>
    public struct HeapItemF<T>
    {
        internal HeapItemF(T item, float priority)
            : this()
        {
            Priority = priority;
            Item = item;
        }

        /// <summary>
        /// The priority of the item.
        /// </summary>
        public float Priority { get; private set; }

        /// <summary>
        /// The item.
        /// </summary>
        public T Item { get; private set; }
    }
}