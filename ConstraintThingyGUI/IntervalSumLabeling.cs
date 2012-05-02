using ConstraintThingy;

namespace ConstraintThingyGUI
{
    class IntervalSumLabeling : IntervalLabeling
    {
        public IntervalSumLabeling(string name, IntervalLabeling a, IntervalLabeling b)
            : base(name, a.InitialRange + b.InitialRange)
        {
            LHS = a;
            RHS = b;
        }

        public readonly IntervalLabeling LHS;
        public readonly IntervalLabeling RHS;

        protected override IntervalVariable MakeVariable(Node n)
        {
            IntervalVariable result = base.MakeVariable(n);
            new IntervalSumConstraint(result, LHS.ValueVariable(n), RHS.ValueVariable(n));
            return result;
        }
    }
}
