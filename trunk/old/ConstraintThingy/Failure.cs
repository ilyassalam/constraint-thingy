using System;

namespace ConstraintThingy
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2237:MarkISerializableTypesWithSerializable")]
    class Failure : Exception
    {
        public Failure(string reason) : base(reason)
        { }
    }
}
