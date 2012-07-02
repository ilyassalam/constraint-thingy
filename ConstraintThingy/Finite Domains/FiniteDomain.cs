using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;

namespace ConstraintThingy
{
    /// <summary>
    /// Represents an immutable finite set of values
    /// </summary>
    public sealed class FiniteDomain<T> : IEnumerable<T>
    {
        /// <summary>
        /// Creates a new finite domain with the specified values.
        /// </summary>
        public FiniteDomain(IEnumerable<T> values) : this(values.ToArray())
        {
            
        }

        /// <summary>
        /// Creates a new finite domain with the specified values.
        /// </summary>
        public FiniteDomain(params T[] values)
        {
            if (values.Length > MaxItems) throw new InvalidOperationException(String.Format("This finite domain only supports {0} items.", MaxItems));

            for (int i = 0; i < values.Length; i++)
            {
                Add(values[i]);
            }
        }

        /// <summary>
        /// Creates a bit set with all of the current elements in the finite domain
        /// </summary>
        [Pure]
        public UInt64 CreateFullBitSet()
        {
            UInt64 set = 0UL;
            for (int i = 0; i < _elements.Count; i++)
            {
                set = set.SetBit(i);
            }

            return set;
        }

        /// <summary>
        /// Creates a bit set from the specified indices
        /// </summary>
        [Pure]
        public UInt64 CreateBitSetFromIndices(params int[] indices)
        {
            UInt64 set = 0UL;

            foreach (var index in indices)
            {
                set = set.SetBit(index);
            }

            return set;
        }

        /// <summary>
        /// Creates a bit set with each of the specified elements.
        /// </summary>
        [Pure]
        public UInt64 CreateBitSet(params T[] elements)
        {
            UInt64 set = 0UL;

            foreach (var element in elements)
            {
                set = set.SetBit(IndexOf(element));
            }

            return set;
        }

        /// <summary>
        /// The maximum number of items in a finite domain.
        /// </summary>
        public const int MaxItems = 64;

        private readonly List<T> _elements = new List<T>();

        /// <summary>
        /// Adds a new item to the finite domain
        /// </summary>
        private void Add(T item)
        {
            if (_elements.Contains(item)) throw new InvalidOperationException("The finite domain already contains that item.");

            _elements.Add(item);
        }

        /// <summary>
        /// Returns the index of <paramref name="item"/>
        /// </summary>
        [Pure]
        public int IndexOf(T item)
        {
            int index = _elements.IndexOf(item);

            if (index < 0) throw new InvalidOperationException("The item was not found in the finite domain.");

            return index;
        }

        /// <summary>
        /// Returns the i'th item in the finite domain.
        /// </summary>
        public T this[int i]
        {
            get { return _elements[i]; }
        }

        /// <summary>
        /// Returns the number of elements in the finite domain
        /// </summary>
        public int Count { get { return _elements.Count; }}

        /// <summary>
        /// All possible values in the finite domain
        /// </summary>
        [Pure]
        public IEnumerable<T> Values
        {
            get { return _elements; }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        public IEnumerator<T> GetEnumerator()
        {
            return _elements.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            builder.Append('{');

            builder.Append(_elements[0]);
            
            for (int i = 1; i < _elements.Count; i++)
            {
                builder.Append(", ");
                builder.Append(_elements[i]);
            }

            builder.Append('}');

            String result = builder.ToString();

            builder.Clear();

            return result;
        }
    }
}