using System;
using Intervals;

namespace ConstraintThingy
{
    /// <summary>
    /// Constrains the sum of one or more variables.
    /// </summary>
    public sealed class IntervalSumConstraint : Constraint<IntervalVariable>
    {
        /// <summary>
        /// Constrains <paramref name="sum"/> = SUM(<paramref name="variables"/>)
        /// </summary>
        public IntervalSumConstraint(IntervalVariable sum, params IntervalVariable[] variables)
            : base(MergeParameters(sum, variables))
        {
            for (int i = 0; i < variables.Length; i++)
            {
                UpdateVariable(Variables[i]);
            }
        }

        private static IntervalVariable[] MergeParameters(IntervalVariable sum, params IntervalVariable[] variables)
        {
            var array = new IntervalVariable[variables.Length + 1];
            array[0] = sum;
            Array.Copy(variables, 0, array, 1, variables.Length);
            return array;
        }

        private IntervalVariable Sum { get { return Variables[0]; } }

        // narrows the value of a variable involved in the constraint
        private Interval NarrowConstraint(Variable narrowedVariable)
        {
            Interval result;

            // Sum = var1 + var2 ... varN
            if (narrowedVariable == Sum)
            {
                // adds up each of the other variables
                Interval value = Variables[1].Value;

                for (int i = 2; i < Variables.Length; i++)
                {
                    value += Variables[i].Value;
                }

                result = value; //Interval.Intersection(value, Sum.Value);
            }
            else
            {
                // identifies the narrowed variable and subtracts every other variable from the sum
                Interval value = Sum.Value;
                IntervalVariable varI = null;

                for (int i = 1; i < Variables.Length; i++)
                {
                    if (Variables[i] != narrowedVariable)
                    {
                        value -= Variables[i].Value;
                    }
                    else varI = Variables[i];
                }

                if (varI == null) throw new InvalidOperationException("The narrowed variable was not found in this constraint's list of variables.");

                result = value; // Interval.Intersection(varI.Value, value);
            }

            return result;
        }

        /// <summary>
        /// Tries to narrow the variable based on the constraint.
        /// </summary>
        public override void UpdateVariable(IntervalVariable var)
        {
            //Interval intersection = NarrowConstraint(var);

            //if (var.Value.Contains(intersection))
            //{
            //    var.Value = intersection;
            //}
            //else
            //{
            //    throw new Failure("The intersection interval was not in the allowable range for this variable.");
            //}
            var.NarrowTo(NarrowConstraint(var));
        }

        
    }
}