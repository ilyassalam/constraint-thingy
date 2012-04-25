using Intervals;

namespace ConstraintThingy
{
    public class IntervalVariable : Variable<Interval>
    {
        public IntervalVariable(string name, Interval initialValue)
            : base(name, initialValue)
        {
        }

        public override bool IsUnique
        {
            get { return Value.IsUnique; }
        }

        public override bool IsEmpty
        {
            get { return Value.IsEmpty; }
        }
    }
}