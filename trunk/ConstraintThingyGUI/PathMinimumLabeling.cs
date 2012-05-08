using System;
using System.Collections.Generic;
using System.Diagnostics;
using ConstraintThingy;
using Intervals;

namespace ConstraintThingyGUI
{
    public class PathMinimumLabeling : IntervalLabeling
    {
        public PathMinimumLabeling(string name, IntervalLabeling integrand, Func<Node, IEnumerable<Node>> predecessors)
            : base(name, new Interval(float.MinValue, float.MaxValue))
        {
            this.integrand = integrand;
            this.predecessors = predecessors;
        }

        private readonly IntervalLabeling integrand;
        private readonly Func<Node, IEnumerable<Node>> predecessors;

        protected override IntervalVariable MakeVariable(Node n)
        {
            var result = base.MakeVariable(n);
            var preds = new List<Node>(predecessors(n));
            if (preds.Count>0)
                new PathMinimumConstraint(result, integrand.ValueVariable(n), this.ValueVariables(preds));
            else
                result.Value = new Interval(0,0);
            return result;
        }

        class PathMinimumConstraint : Constraint<IntervalVariable>
        {
            public PathMinimumConstraint(IntervalVariable sum, IntervalVariable value, IEnumerable<IntervalVariable> predecessors) : base(sum, value, predecessors)
            {
                // The code relies on the sum being the first element of Variables, value being the second, and the predecessors being the others.
                Debug.Assert(Variables[0] == sum);
                Debug.Assert(Variables[1] == value);
            }

            public override void Narrowed(Variable narrowedVariable)
            {
                // The integrand value or a predecessor was narrowed.
                Interval min = Variables[2].Value;
                for (int i = 3; i < Variables.Length; i++)
                    min = Interval.Min(min, Variables[i].Value);

                if (narrowedVariable == Variables[0])
                {
                    // The sum was narrowed
                    Interval sum = Variables[0].Value;
                    //Variables[1].NarrowTo(new Interval(sum.LowerBound-min.UpperBound, sum.UpperBound-min.LowerBound));
                    Variables[1].NarrowTo(sum - min);
                    var integrand = Variables[1].Value;
                    //Interval newMin = new Interval(sum.LowerBound - integrand.UpperBound,
                    //                               sum.UpperBound - integrand.LowerBound);
                    Interval newMin = sum - integrand;
                    for (int i=3; i<Variables.Length; i++)
                        Variables[i].NarrowTo(newMin);
                }
                else
                {
                    Interval calculatedSum = min + Variables[1].Value;
                    // Update sum variable
                    Variables[0].NarrowTo(calculatedSum);
                }
            }

            public override void UpdateVariable(IntervalVariable var)
            {
                throw new NotImplementedException();
            }
        }
    }

    public class StartEndPathLabeling : PathMinimumLabeling
    {
        public StartEndPathLabeling(string name, IntervalLabeling integrand, UndirectedGraph graph, float startValue, Node start, Node end)
            : base(name, integrand, node => Predecessors(node, graph, start, end))
        {
            AssignVariableToNode(start, new IntervalVariable(name+" start", new Interval(startValue, startValue)));
        }

        private static IEnumerable<Node> Predecessors(Node node, UndirectedGraph graph, Node start, Node end)
        {
            //foreach (var n in node.Neighbors)
            //    if (graph.Distance(end, n) > graph.Distance(end, node)
            //        && graph.Distance(start, n)<= graph.Distance(start, end))
            //        yield return n;
            foreach (var e in graph.Edges)
                if (e.Second == node)
                    yield return e.First;
        }
    }
}
