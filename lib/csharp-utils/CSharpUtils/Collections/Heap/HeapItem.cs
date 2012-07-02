namespace CSharpUtils.Collections
{
    /// <summary>
    /// An {item, priorty} pair.
    /// </summary>
    public struct HeapItem<T>
    {
        internal HeapItem(T item, double priority)
            : this()
        {
            Priority = priority;
            Item = item;
        }

        /// <summary>
        /// The priority of the item.
        /// </summary>
        public double Priority { get; private set; }

        /// <summary>
        /// The item.
        /// </summary>
        public T Item { get; private set; }
    }
}