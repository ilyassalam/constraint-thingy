using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;

namespace CSharpUtils.Collections
{
    /// <summary>
    /// Contains several operations that operate on immutable linked lists.
    /// </summary>
    public static class LL
    {
        /// <summary>
        /// Creates a list with the integers from 0 to <paramref name="max"/> - 1.
        /// </summary>
        [Pure]
        public static LL<int> Range(int max)
        {
            return Range(0, max);
        }

        /// <summary>
        /// Creates a list with the integers from <paramref name="min"/> to <paramref name="max"/> - 1.
        /// </summary>
        [Pure]
        public static LL<int> Range(int min, int max)
        {
            LL<int> result = LL<int>.Nil;
            for (int i = max - 1; i >= min; i--)
            {
                result = result.AddFront(i);
            }
            return result;
        }

        /// <summary>
        /// Creates a list with the specified items.
        /// </summary>
        [Pure]
        public static LL<object> Create(params object[] items)
        {
            return items.ToLinkedList();
        }

        /// <summary>
        /// Creates a list with the specified items.
        /// </summary>
        [Pure]
        public static LL<object> Create(List<object> items)
        {
            return items.ToLinkedList();
        }

        /// <summary>
        /// Creates a list with the specified items.
        /// </summary>
        [Pure]
        public static LL<object> Create(IEnumerable<object> items)
        {
            return items.ToLinkedList();
        }

        /// <summary>
        /// Creates a list with the specified items.
        /// </summary>
        [Pure]
        public static LL<T> Create<T>(params T[] items)
        {
            return items.ToLinkedList();
        }

        /// <summary>
        /// Creates a list with the specified items.
        /// </summary>
        [Pure]
        public static LL<T> Create<T>(List<T> items)
        {
            return items.ToLinkedList();
        }

        /// <summary>
        /// Creates a list with the specified items.
        /// </summary>
        [Pure]
        public static LL<T> Create<T>(IEnumerable<T> items)
        {
            return items.ToLinkedList();
        }

        /// <summary>
        /// Creates a list with the specified items.
        /// </summary>
        [Pure]
        public static LL<T> ToLinkedList<T>(this IEnumerable<T> items)
        {
            return LL<T>.Nil.AddFront(items);
        }

        /// <summary>
        /// Creates a list with the specified items.
        /// </summary>
        [Pure]
        public static LL<T> ToLinkedList<T>(this List<T> items)
        {
            return LL<T>.Nil.AddFront(items);
        }

        /// <summary>
        /// Creates a list with the specified items.
        /// </summary>
        [Pure]
        public static LL<T> ToLinkedList<T>(this T[] items)
        {
            return LL<T>.Nil.AddFront(items);
        }

        /// <summary>
        /// Maps a delegate over each element of the list and returns the resulting list.
        /// </summary>
        [Pure]
        public static LL<TOut> Map<TIn, TOut>(this LL<TIn> list, Func<TIn, TOut> func)
        {
            LL<TOut> result = LL<TOut>.Nil;

            foreach (var item in list)
            {
                result = result.AddFront(func(item));
            }

            return result.Reverse();
        }

        /// <summary>
        /// Returns a reversed copy of the list.
        /// </summary>
        [Pure]
        public static LL<T> Reverse<T>(this LL<T> list)
        {
            LL<T> result = LL<T>.Nil;
            foreach (var item in list)
            {
                result = result.AddFront(item);
            }
            return result;
        }

        /// <summary>
        /// Appends one or more lists to the tail of the current list. The resulting list is returned.
        /// </summary>
        [Pure]
        public static LL<T> Append<T>(this LL<T> list, params LL<T>[] lists)
        {
            var last = lists[lists.Length - 1];
            for (int i = lists.Length - 2; i >= 0; i--)
            {
                last = last.AddFront(lists[i]);
            }
            return last.AddFront(list);
        }

        /// <summary>
        /// Adds a collection of items to the front of the list and returns the new list. O(k), where k is the number if items added.
        /// </summary>
        [Pure]
        public static LL<T> AddFront<T>(this LL<T> list, IEnumerable<T> items)
        {
            return list.AddFront(items.ToArray());
        }

        /// <summary>
        /// Adds a collection of items to the front of the list and returns the new list. O(k), where k is the number if items added.
        /// </summary>
        [Pure]
        public static LL<T> AddFront<T>(this LL<T> list, List<T> items)
        {
            LL<T> result = list;
            for (int i = items.Count - 1; i >= 0; i--)
            {
                result = result.AddFront(items[i]);
            }
            return result;
        }

        /// <summary>
        /// Adds a collection of items to the front of the list and returns the new list. O(k), where k is the number if items added.
        /// </summary>
        [Pure]
        public static LL<T> AddFront<T>(this LL<T> list, params T[] items)
        {
            LL<T> result = list;
            for (int i = items.Length - 1; i >= 0; i--)
            {
                result = result.AddFront(items[i]);
            }
            return result;
        }

        /// <summary>
        /// Adds an item to the front of the list and returns the new list. O(1)
        /// </summary>
        [Pure]
        public static LL<T> AddFront<T>(this LL<T> list, T item)
        {
            return new LL<T>(item, list);
        }

        /// <summary>
        /// Adds an item to the back of the list and returns the new list. O(n), where n = the length of the list
        /// </summary>
        [Pure]
        public static LL<T> AddBack<T>(this LL<T> list, T item)
        {
            return new LL<T>(item).AddFront(list);
        }

        /// <summary>
        /// Adds several items to the back of the list and returns the new list. O(n + k), where n = length of the list and k is the number of items appended
        /// </summary>
        [Pure]
        public static LL<T> AddBack<T>(this LL<T> list, params T[] items)
        {
            return Create(items).AddFront(list);
        }

        /// <summary>
        /// Adds several items to the back of the list and returns the new list. O(n + k), where n = length of the list and k is the number of items appended
        /// </summary>
        [Pure]
        public static LL<T> AddBack<T>(this LL<T> list, List<T> items)
        {
            return Create(items).AddFront(list);
        }

        /// <summary>
        /// Adds several items to the back of the list and returns the new list. O(n + k), where n = length of the list and k is the number of items appended
        /// </summary>
        [Pure]
        public static LL<T> AddBack<T>(this LL<T> list, IEnumerable<T> items)
        {
            return Create(items).AddFront(list);
        }

        /// <summary>
        /// Selects and returns the items in the list between indicies <paramref name="minIndex"/> and <paramref name="maxIndex"/> - 1.
        /// </summary>
        [Pure]
        public static LL<T> SelectRange<T>(this LL<T> list, int minIndex, int maxIndex)
        {
            int currentIndex = 0;
            while (currentIndex < minIndex)
            {
                list = list.Rest;
                currentIndex++;
            }

            LL<T> result = LL<T>.Nil;

            while (currentIndex < maxIndex)
            {
                result = result.AddFront(list.First);
                list = list.Rest;
                currentIndex++;
            }

            return result.Reverse();
        }

        /// <summary>
        /// Returns the index of the first occurence of <paramref name="value"/> in the list. If not found, -1 will be returned.
        /// </summary>
        [Pure]
        public static int Find<T>(this LL<T> list, T value)
        {
            int index = 0;
            while (list != LL<T>.Nil)
            {
                if (list.First.Equals(value)) return index;

                index++;

                list = list.Rest;
            }

            return -1;
        }

        /// <summary>
        /// Returns the index of the first item in the list that returns true when passed as an argument to the provided delegate. If not found, -1 will be returned.
        /// </summary>
        [Pure]
        public static int FindWhere<T>(this LL<T> list, Func<T, bool> func)
        {
            int index = 0;
            while (list != LL<T>.Nil)
            {
                if (func(list.First)) return index;

                index++;

                list = list.Rest;
            }

            return -1;
        }

        /// <summary>
        /// Removes the item at the specified index.
        /// </summary>
        [Pure]
        public static LL<T> RemoveAt<T>(this LL<T> list, int index)
        {
            int i = 0;

            LL<T> iteratedList = list;

            while (list != LL<T>.Nil)
            {
                if (i == index)
                {
                    return list.SelectRange(0, i).Append(iteratedList.Rest);
                }

                i++;

                iteratedList = iteratedList.Rest;
            }

            return list;
        }

        /// <summary>
        /// Removes the first instance of the item from the list.
        /// </summary>
        [Pure]
        public static LL<T> Remove<T>(this LL<T> list, T value)
        {
            int index = list.Find(value);
            if (index != -1)
            {
                return list.RemoveAt(index);
            }
            else return list;
        }

        /// <summary>
        /// Performs a filter on the list.
        /// </summary>
        [Pure]
        public static LL<T> Filter<T>(this LL<T> list, Func<T, bool> value)
        {
            var newList = LL<T>.Nil;

            while (list != LL<T>.Nil)
            {
                if (value(list.First)) newList = newList.AddFront(list.First);

                list = list.Rest;
            }

            return newList.Reverse();
        }

        /// <summary>
        /// True if the list contains the value, false otherwise.
        /// </summary>
        [Pure]
        public static bool Contains<T>(this LL<T> list, T value)
        {
            return list.Find(value) != -1;
        }
    }

    /// <summary>
    /// An immutable linked list.
    /// </summary>
    public sealed class LL<T> : IEnumerable<T>, IEquatable<LL<T>>
    {
        /// <summary>
        /// The head element of this list.
        /// </summary>
        public T First { get; private set; }

        /// <summary>
        /// The tail of the list.
        /// </summary>
        public LL<T> Rest { get; private set; }

        /// <summary>
        /// Represents the Nil value.
        /// </summary>
        public readonly static LL<T> Nil = new LL<T>();

        private LL()
        {
        }

        /// <summary>
        /// Creates a new linked list with the specified head and a Nil tail
        /// </summary>
        public LL(T head) : this (head, Nil)
        {
        }

        /// <summary>
        /// Creates a new linked list with the specified head and tail.
        /// </summary>
        public LL(T head, LL<T> tail)
        {
            First = head;
            Rest = tail;
        }

        /// <summary>
        /// Returns the <paramref name="i"/>'th item in the list.
        /// </summary>
        public T this[int i]
        {
            get
            {
                int count = 0;
                LL<T> current = this;
                while (count != i)
                {
                    count++;
                    current = current.Rest;
                }
                return current.First;
            }
        }

        /// <summary>
        /// True if there are no items in this list.
        /// </summary>
        public bool Empty
        {
            get { return this == Nil; }
        }

        /// <summary>
        /// The second item in the list.
        /// </summary>
        public T Second
        {
            get
            {
                return Rest.First;
            }
        }

        /// <summary>
        /// The third item in the list.
        /// </summary>
        public T Third
        {
            get
            {
                return Rest.Rest.First;
            }
        }

        /// <summary>
        /// The fourth item in the list.
        /// </summary>
        public T Fourth
        {
            get
            {
                return Rest.Rest.Rest.First;
            }
        }

        /// <summary>
        /// The fifth item in the list.
        /// </summary>
        public T Fifth
        {
            get
            {
                return Rest.Rest.Rest.Rest.First;
            }
        }

        /// <summary>
        /// The sixth item in the list.
        /// </summary>
        public T Sixth
        {
            get
            {
                return Rest.Rest.Rest.Rest.Rest.First;
            }
        }

        /// <summary>
        /// The seventh item in the list.
        /// </summary>
        public T Seventh
        {
            get
            {
                return Rest.Rest.Rest.Rest.Rest.Rest.First;
            }
        }

        /// <summary>
        /// The eighth item in the list.
        /// </summary>
        public T Eighth
        {
            get
            {
                return Rest.Rest.Rest.Rest.Rest.Rest.Rest.First;
            }
        }

        /// <summary>
        /// The ninth item in the list.
        /// </summary>
        public T Ninth
        {
            get
            {
                return Rest.Rest.Rest.Rest.Rest.Rest.Rest.Rest.First;
            }
        }

        /// <summary>
        /// The tenth item in the list.
        /// </summary>
        public T Tenth
        {
            get
            {
                return Rest.Rest.Rest.Rest.Rest.Rest.Rest.Rest.Rest.First;
            }
        }

        
        /// <summary>
        /// The number of items in the list. This is an O(n) operation.
        /// </summary>
        public int Length 
        { 
            get
            {
                int length = 0;

                LL<T> current = this;
                while (current != Nil)
                {
                    length++;
                    current = current.Rest;
                }

                return length;
            }
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
            LL<T> current = this;
            while (current != Nil)
            {
                yield return current.First;
                current = current.Rest;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool Equals(LL<T> other)
        {
            LL<T> self = this;

            while (true)
            {
                if (ReferenceEquals(self, other)) return true;
                else if (ReferenceEquals(self, LL<T>.Nil))
                {
                    if (ReferenceEquals(other, LL<T>.Nil))
                    {
                        return true;
                    }
                    else return false;
                }
                else if (ReferenceEquals(other, LL<T>.Nil))
                {
                    return false;
                }
                else if (!Equals(self.First, other.First))
                {
                    return false;
                }

                self = self.Rest;
                other = other.Rest;
            }
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        [Pure]
        public override string ToString()
        {
            if (Empty) return "()";

            StringBuilder builder = new StringBuilder();
            builder.Append(String.Format("({0}", First));
            foreach (var item in Rest)
            {
                builder.Append(String.Format(", {0}", item));
            }
            builder.Append(")");
            return builder.ToString();
        }
    }
}