namespace ConstraintThingy
{
    /// <summary>
    /// Defines options for expanding nodes through the search
    /// </summary>
    public enum ExpansionOrder
    {
        /// <summary>
        /// Expands options in a non-randomized order
        /// </summary>
        Deterministic, 

        /// <summary>
        /// Expands options in a random order
        /// </summary>
        Random
    }
}