using System.Collections;
using System.Collections.Generic;

namespace CSharpUtils.Collections
{
    /// <summary>
    /// Represents a mapping between values of two types
    /// </summary>
    /// <typeparam name="T1">The value of the first type</typeparam>
    /// <typeparam name="T2">The value of the second type</typeparam>
    public class TwoWayDictionary<T1, T2> : IEnumerable<Pair<T1, T2>>
    {
        private readonly Dictionary<T1, T2> _a = new Dictionary<T1, T2>();
        private readonly Dictionary<T2, T1> _b = new Dictionary<T2, T1>();

        /// <summary>
        /// Adds a two-way mapping between <paramref name="a"/> and <paramref name="b"/>
        /// </summary>
        public void Add(T1 a, T2 b)
        {
            _a.Add(a, b);
            _b.Add(b, a);
        }

        /// <summary>
        /// Gets and sets items in the collection
        /// </summary>
        public T1 this[T2 key]
        {
            get { return _b[key]; }
            set
            {
                var newValue = value;

                var otherKey = _b[key];
                if (_a.ContainsKey(otherKey)) _a.Remove(otherKey);

                if (_b.ContainsKey(key)) _b.Remove(key);

                _b[key] = newValue;
                _a[newValue] = key;
            }
        }

        /// <summary>
        /// Gets and sets items in the collection
        /// </summary>
        public T2 this[T1 key]
        {
            get { return _a[key]; }
            set
            {
                var newValue = value;

                var otherKey = _a[key];
                if (_b.ContainsKey(otherKey)) _b.Remove(otherKey);

                if (_a.ContainsKey(key)) _a.Remove(key);

                _a[key] = newValue;
                _b[newValue] = key;
            }
        }

        /// <summary>
        /// Removes the pair mapping with <paramref name="a"/>
        /// </summary>
        public void Remove(T1 a)
        {
            _b.Remove(_a[a]);
            _a.Remove(a);
        }

        /// <summary>
        /// Removes the mapping with <paramref name="b"/>
        /// </summary>
        public void Remove(T2 b)
        {
            _a.Remove(_b[b]);
            _b.Remove(b);
        }

        /// <summary>
        /// Clears the dictionary
        /// </summary>
        public void Clear()
        {
            _a.Clear();
            _b.Clear();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        public IEnumerator<Pair<T1, T2>> GetEnumerator()
        {
            foreach (var kvp in _a)
            {
                yield return new Pair<T1, T2>(kvp.Key, kvp.Value);
            }
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
    }
}