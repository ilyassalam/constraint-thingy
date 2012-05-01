using System;

namespace ConstraintThingy
{
    /// <summary>
    /// Represents a finite set that forms the possible values of a FiniteDomainVariable.
    /// </summary>
    public class FiniteDomain
    {
        /// <summary>
        /// Represents a finite set that forms the possible values of a FiniteDomainVariable.
        /// </summary>
        public FiniteDomain(params string[] elts)
        {
            Elements = elts;
        }
        /// <summary>
        /// String names of the different elements
        /// </summary>
        public string[] Elements { get; private set; }

        /// <summary>
        /// Number of elements in the domain
        /// </summary>
        public int Size
        {
            get { return Elements.Length; }
        }

        public UInt64 UniverseMask { get { return (1UL << Size) - 1; } }

        /// <summary>
        /// Returns the bitmask corresponding to the given element.
        /// </summary>
        public UInt64 Bitmask(string element)
        {
            int bit = Array.IndexOf(Elements, element);
            if (bit<0)
                throw new ArgumentException("Not an element of domain: "+element);
            return 1UL << bit;
        }
    }
}
