using System;
using System.Collections.Generic;
using System.Diagnostics;
using ConstraintThingy;
using System.Linq;
using Intervals;

namespace ConstraintThingyGUI
{
    /// <summary>
    /// A labeling for a min-type path function.
    /// </summary>
    public class PathMinimumLabeling : IntervalLabeling
    {
        /// <summary>
        /// Creates a new labeling for a min-type path function.
        /// </summary>
        public PathMinimumLabeling(string name, IntervalLabeling integrand, Func<Node, IEnumerable<Node>> predecessors)
            : base(name, new Interval(float.MinValue, float.MaxValue))
        {
            this.integrand = integrand;
            this.predecessors = predecessors;
        }

        private readonly IntervalLabeling integrand;
        private readonly Func<Node, IEnumerable<Node>> predecessors;

        /// <summary>
        /// Makes a variable to hold the value of the labeling on node N.
        /// </summary>
        protected override IntervalVariable MakeVariable(Node n)
        {
            var result = base.MakeVariable(n);
            var preds = new List<Node>(predecessors(n));
            if (preds.Count>0)
                new PathMinimumConstraint(result,
                                          (n.Support.Count == 0)
                                              ? integrand.ValueVariable(n)
                                              : IntervalVariable.Sum(n.Support.Select(s => integrand.ValueVariable(s)).Concat(new[] { integrand.ValueVariable(n) })),
                                          ValueVariables(preds));
            else
                result.SetValueOrThrowException(new Interval(0,0), "Could not create path variable");
            return result;
        }

        class PathMinimumConstraint : Constraint<IntervalVariable>
        {
            public PathMinimumConstraint(IntervalVariable sum, IntervalVariable value, IEnumerable<IntervalVariable> predecessors) : base(sum, value, predecessors)
            {
                // The code relies on the sum being the first element of Variables, value being the second, and the predecessors being the others.
                Debug.Assert(Variables[sumPosition] == sum);
                Debug.Assert(Variables[integrandPosition] == value);
            }

            private const int sumPosition = 0;
            private const int integrandPosition = 1;
            private const int predecessorStart = 2;

            public override void Narrowed(Variable narrowedVariable, ref bool succeeded)
            {
                // The integrand value or a predecessor was narrowed.
                Interval min = Variables[predecessorStart].Value;
                for (int i = predecessorStart+1; i < Variables.Length; i++)
                    min = Interval.Min(min, Variables[i].Value);

                if (narrowedVariable == Variables[sumPosition])
                {
                    // The sum was narrowed
                    Interval sum = Variables[sumPosition].Value;
                    //Variables[integrandPosition].NarrowTo(new Interval(sum.LowerBound-min.UpperBound, sum.UpperBound-min.LowerBound));
                    Variables[integrandPosition].NarrowTo(sum - min, ref succeeded);
                    if (!succeeded) return;
                    var integrand = Variables[integrandPosition].Value;
                    //Interval newMin = new Interval(sum.LowerBound - integrand.UpperBound,
                    //                               sum.UpperBound - integrand.LowerBound);
                    Interval newMin = sum - integrand;
                    for (int i = predecessorStart + 1; i < Variables.Length; i++)
                    {
                        Variables[i].NarrowTo(newMin, ref succeeded);
                        if (!succeeded) return;
                    }
                }
                else if (narrowedVariable == Variables[integrandPosition])
                {
                    // The integrand was narrowed
                    Interval integrand = Variables[integrandPosition].Value;
                    //Variables[integrandPosition].NarrowTo(new Interval(sum.LowerBound-min.UpperBound, sum.UpperBound-min.LowerBound));
                    Variables[sumPosition].NarrowTo(integrand + min, ref succeeded);
                    if (!succeeded) return;
                    var sum = Variables[sumPosition].Value;
                    //Interval newMin = new Interval(sum.LowerBound - integrand.UpperBound,
                    //                               sum.UpperBound - integrand.LowerBound);
                    Interval newMin = sum - integrand;
                    for (int i = predecessorStart + 1; i < Variables.Length; i++)
                    {
                        Variables[i].NarrowTo(newMin, ref succeeded);
                        if (!succeeded) return;
                    }
                }
                else
                {
                    // Min changed
                    Variables[sumPosition].NarrowTo(min + Variables[integrandPosition].Value, ref succeeded);
                    if (!succeeded) return;
                    Variables[integrandPosition].NarrowTo(Variables[sumPosition].Value-min, ref succeeded);
                    // if (!succeeded) return;
                }
            }

            public override void UpdateVariable(IntervalVariable var, ref bool succeeded)
            {
                throw new NotImplementedException();
            }
        }
    }

    /// <summary>
    /// A min-type path function with a designated start and end node.
    /// </summary>
    public class StartEndPathLabeling : PathMinimumLabeling
    {
        /// <summary>
        /// Creates a min-type path function with a designated start and end node.
        /// </summary>
        public StartEndPathLabeling(string name, IntervalLabeling integrand, UndirectedGraph graph, float startValue, Node start, Node end)
            : base(name, integrand, node => Predecessors(node, graph, start, end))
        {
            AssignVariableToNode(start, new IntervalVariable(name+" start", new Interval(startValue, startValue)));
        }

// ReSharper disable UnusedParameter.Local
        private static IEnumerable<Node> Predecessors(Node node, UndirectedGraph graph, Node start, Node end)
// ReSharper restore UnusedParameter.Local
        {
            //foreach (var n in node.Neighbors)
            //    if (graph.Distance(end, n) > graph.Distance(end, node)
            //        && graph.Distance(start, n)<= graph.Distance(start, end))
            //        yield return n;
            foreach (var e in graph.Edges)
                if (e.Second == node && e.First.SupportRecipient == null)
                    yield return e.First;
        }
    }
}
