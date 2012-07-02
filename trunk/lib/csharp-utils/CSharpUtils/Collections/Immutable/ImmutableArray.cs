using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CSharpUtils.Collections
{
    /// <summary>
    /// An immutable array.
    /// </summary>
    public class ImmutableArray<T> : IEnumerable<T>
    {
        private readonly T[] _values;

        /// <summary>
        /// The number of items in the collection.
        /// </summary>
        public int Length { get { return _values.Length; } }

        /// <summary>
        /// Acesses the item at the specified index.
        /// </summary>
        public T this[int index]
        {
            get { return _values[index]; }
        }

        /// <summary>
        /// Creates a new immutable array from the values.
        /// </summary>
        public ImmutableArray(IEnumerable<T> values)
        {
            _values = values.ToArray();
        }

        /// <summary>
        /// Creates a new immutable array from the values.
        /// </summary>
        public ImmutableArray(params T[] values)
        {
            _values = new T[values.Length];
            Array.Copy(values, _values, values.Length);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the array
        /// </summary>
        public IEnumerator<T> GetEnumerator()
        {
            return ((IEnumerable<T>)_values).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}