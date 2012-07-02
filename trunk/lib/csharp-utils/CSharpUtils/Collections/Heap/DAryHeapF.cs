using System;
using System.Diagnostics;

namespace CSharpUtils.Collections
{
    /// <summary>
    /// Represents a DAry Heap, a generalization of a binary heap where nodes can have an arbitrary number of children.
    /// </summary>
    public sealed class DAryHeapF<T> : DAryHeap
    {
        private T[] _items;
        private float[] _priorities;

        private readonly int _numberOfChildren;
        private int _count;

        /// <summary>
        /// The number of items in the heap.
        /// </summary>
        public int Count { get { return _count; } }

        /// <summary>
        /// The number of children per node.
        /// </summary>
        public int NumberOfChildren { get { return _numberOfChildren; } }

        /// <summary>
        /// Removes all values from the heap.
        /// </summary>
        public void Clear()
        {
            Array.Clear(_items, 0, _count);
            _count = 0;
        }

        /// <summary>
        /// Creates a new DAryHeap with the provided initial size, number of children per node, and a flag specifying a flag specifying whether the heap removes minimum or maximum values.
        /// </summary>
        /// <param name="initialSize">The initial size of the heap. Size is doubled when full.</param>
        /// <param name="numberOfChildren">The number of children per node. Increasing this number will speed up Insert() calls while slowing down ExtractMin() calls.</param>
        public DAryHeapF(int initialSize = DefaultHeapSize, int numberOfChildren = DefaultNumberOfChildren)
        {
            if (initialSize < 1)
            {
                throw new ArgumentOutOfRangeException("Initial heap size cannot be less than 1.");
            }
            if (numberOfChildren < 2) // 1 works, but its safe to say O(n) performance on all operations is less than ideal and almost certainly a user error
            {
                throw new ArgumentOutOfRangeException("Number of children cannot be less than 2.");
            }

            _items = new T[initialSize];
            _priorities = new float[initialSize];

            _numberOfChildren = numberOfChildren;
        }

        /// <summary>
        /// Returns the item with minimum priority.
        /// </summary>
        /// <returns>The item with minimum priority.</returns>
        public HeapItemF<T> FindMin()
        {
            if (_count <= 0) throw new InvalidOperationException("Cannot get an item in an empty heap.");
            return new HeapItemF<T>(_items[0], _priorities[0]);
        }

        /// <summary>
        /// Removes and returns the item with minimum priority.
        /// </summary>
        /// <returns>The item with minimum priority.</returns>
        public HeapItemF<T> ExtractMin()
        {
            var minimum = FindMin();

            int lastLeaf = _count - 1;

            _items[0] = _items[lastLeaf];
            _priorities[0] = _priorities[lastLeaf];

            // remove any references in the heap so things can be garbage collected
            _items[lastLeaf] = default(T);

            _count--;

            SiftDown(0);

            return minimum;
        }

        /// <summary>
        /// Inserts a new item into the queue with the provided priority.
        /// </summary>
        /// <param name="item">The item to insert.</param>
        /// <param name="priority">The priority at of theitem</param>
        public void Insert(T item, float priority)
        {
            if (_count == _items.Length)
            {
                Debug.Assert(_items.Length == _priorities.Length);

                Array.Resize(ref _items, _items.Length * 2);
                Array.Resize(ref _priorities, _priorities.Length * 2);
            }

            _items[_count] = item;
            _priorities[_count] = priority;

            // bubble up
            SiftUp(_count);

            _count++;
        }

        #region Helper Methods
        private void SiftUp(int currentIndex)
        {
            int parent = GetParent(currentIndex);
            // swap until we are the root or we are larger than our parent
            while (currentIndex > 0 && _priorities[parent] >= _priorities[currentIndex])
            {
                Swap(currentIndex, parent);
                currentIndex = parent;
                parent = GetParent(currentIndex);
            }
        }

        private void SiftDown(int currentPosition)
        {
            int smallestChild = currentPosition;
            while (true)
            {
                currentPosition = smallestChild;
                smallestChild = GetSmallestChild(currentPosition);

                if (smallestChild < _count && _priorities[currentPosition] >= _priorities[smallestChild])
                {
                    Swap(smallestChild, currentPosition);
                }
                else
                {
                    break;
                }
            }
        }

        private int GetChild(int node, int index)
        {
            return _numberOfChildren * node + index + 1;
        }

        private int GetSmallestChild(int node)
        {
            int start = GetChild(node, 0);
            int smallestChild = GetChild(node, 0);
            for (int i = 1; i < _numberOfChildren; i++)
            {
                int currentChild = start + i;
                if (currentChild < _count && _priorities[currentChild] < _priorities[smallestChild])
                {
                    smallestChild = currentChild;
                }
            }
            return smallestChild;
        }

        private int GetParent(int node)
        {
            return (node - 1) / _numberOfChildren;
        }

        private void Swap(int node1, int node2)
        {
            // swap the item
            var tempItem = _items[node1];
            _items[node1] = _items[node2];
            _items[node2] = tempItem;

            var tempPriority = _priorities[node1];
            _priorities[node1] = _priorities[node2];
            _priorities[node2] = tempPriority;
        }

        #endregion
    }
}