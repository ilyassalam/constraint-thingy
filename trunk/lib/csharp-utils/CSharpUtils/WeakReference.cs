using System;

namespace CSharpUtils
{
    /// <summary>
    /// A type-safe variant of weak references
    /// </summary>
    public class WeakReference<T> : WeakReference 
    {
        /// <summary>
        /// Creates a new type-safe weak reference
        /// </summary>
        public WeakReference(T value) : base(value)
        {
        }

        /// <summary>
        /// Gets or sets the object referenced by the weak reference
        /// </summary>
        public new T Target { get { return (T) base.Target; } set { base.Target = value;} }
    }
}