namespace CSharpUtils.Collections
{
    /// <summary>
    /// An {item, priorty} pair.
    /// </summary>
    public struct HeapItemL<T>
    {
        internal HeapItemL(T item, long priority)
            : this()
        {
            Priority = priority;
            Item = item;
        }

        /// <summary>
        /// The priority of the item.
        /// </summary>
        public long Priority { get; private set; }

        /// <summary>
        /// The item.
        /// </summary>
        public T Item { get; private set; }
    }
}