using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using CSharpUtils;
using CSharpUtils.Collections;

namespace ConstraintThingy
{
    public partial class Constraint
    {
        #region Integers

        /// <summary>
        /// Constrains <paramref name="sum"/> to be the sum of <paramref name="a"/> and <paramref name="b"/>
        /// </summary>
        public static void Sum(IntegerVariable sum, IntegerVariable a, IntegerVariable b)
        {
            new IntegerSumConstraint(sum, a, b);
        }

        /// <summary>
        /// Creates and returns an integer variable that represents the sum of <paramref name="a"/> and <paramref name="b"/>
        /// </summary>
        public static IntegerVariable Add(IntegerVariable a, IntegerVariable b)
        {
            IntegerVariable sum;

            if (a.ConstraintThingySolver.SubexpressionEliminator.TryGetValue(Add, a, b, out sum))
            {
                return sum;
            }

            sum = new IntegerVariable(a.ConstraintThingySolver, null, IntegerVariable.DefaultRange);

            Constraint.InRange(sum, a.AllowableValues + b.AllowableValues);

            Sum(sum, a, b);

            a.ConstraintThingySolver.SubexpressionEliminator.Store(Add, a, b, sum);

            return sum;
        }

        /// <summary>
        /// Creates and returns an integer variable that represents the sum of <paramref name="variables"/>
        /// </summary>
        public static IntegerVariable Add(params IntegerVariable[] variables)
        {
            return variables.Aggregate(Add);
        }

        /// <summary>
        /// Creates and returns an integer variable that represents the sum of <paramref name="variables"/>
        /// </summary>
        public static IntegerVariable Add(IEnumerable<IntegerVariable> variables)
        {
            return variables.Aggregate(Add);
        }

        /// <summary>
        /// Creates and returns an integer variable that represents the sum of <paramref name="variables"/>
        /// </summary>
        public static IntegerVariable Add(IEnumerable<IntegerVariable> variables, int minValue, int maxValue)
        {
            return variables.Aggregate(Add);
        }

        /// <summary>
        /// Creates and returns an integer variable that represents the difference of <paramref name="a"/> and <paramref name="b"/>
        /// </summary>
        public static void Difference(IntegerVariable difference, IntegerVariable a, IntegerVariable b)
        {
            new IntegerDifferenceConstraint(difference, a, b);
        }

        /// <summary>
        /// Creates and returns an integer variable that represents the difference of <paramref name="a"/> and <paramref name="b"/>
        /// </summary>
        public static IntegerVariable Subtract(IntegerVariable a, IntegerVariable b)
        {
            IntegerVariable difference;

            if (a.ConstraintThingySolver.SubexpressionEliminator.TryGetValue(Subtract, a, b, out difference))
            {
                return difference;
            }

            difference = new IntegerVariable(a.ConstraintThingySolver, null, IntegerVariable.DefaultRange);

            Constraint.InRange(difference, a.AllowableValues - b.AllowableValues);
            
            Difference(difference, a, b);

            a.ConstraintThingySolver.SubexpressionEliminator.Store(Subtract, a, b, difference);

            return difference;
        }

        /// <summary>
        /// Constraints <paramref name="variable"/> to be exactly <paramref name="value"/>
        /// </summary>
        public static void Equal(IntegerVariable variable, int value)
        {
            InRange(variable, value, value);
        }

        /// <summary>
        /// Constrains <paramref name="variable" /> to be in <paramref name="range"/>
        /// </summary>
        public static void InRange(IntegerVariable variable, IntegerInterval range)
        {
            variable.BackdoorSet(IntegerInterval.Intersection(variable.AllowableValues, range));
        }

        /// <summary>
        /// Constrains <paramref name="variable"/> to be between <paramref name="minValue"/> and <paramref name="maxValue"/>
        /// </summary>
        public static void InRange(IntegerVariable variable, int minValue, int maxValue)
        {
            InRange(variable, new IntegerInterval(minValue, maxValue));
        }

        /// <summary>
        /// Constraints <paramref name="variable"/> to be at most <paramref name="value"/>
        /// </summary>
        public static void LessThanOrEqual(IntegerVariable variable, int value)
        {
            InRange(variable, int.MinValue, value);
        }

        /// <summary>
        /// Constrains <paramref name="variable"/> to be at least <paramref name="value"/>
        /// </summary>
        public static void GreaterThanOrEqual(IntegerVariable variable, int value)
        {
            InRange(variable, value, int.MaxValue);
        }

        /// <summary>
        /// Constrains that the values of <paramref name="a"/> and <paramref name="b"/> are not equal.
        /// </summary>
        public static void NotEqual(IntegerVariable a, IntegerVariable b)
        {
            new IntegerInequalityConstraint(a, b);
        }

        /// <summary>
        /// Constraints that the values of <paramref name="variables"/> are not equal.
        /// </summary>
        public static void AllDifferent(params IntegerVariable[] variables)
        {
            NotEqual(variables);
        }

        /// <summary>
        /// Constraints that the values of <paramref name="variables"/> are not equal.
        /// </summary>
        public static void AllDifferent(IEnumerable<IntegerVariable> variables)
        {
            NotEqual(variables);
        }

        /// <summary>
        /// Constrains that the values of <paramref name="variables"/> are not equal.
        /// </summary>
        public static void NotEqual(params IntegerVariable[] variables)
        {
            for (int i = 0; i < variables.Length - 1; i++)
                NotEqual(variables[i], variables[i + 1]);
        }

        /// <summary>
        /// Constrains that the values of <paramref name="variables"/> are not equal.
        /// </summary>
        public static void NotEqual(IEnumerable<IntegerVariable> variables)
        {
            NotEqual(variables.ToArray());
        }

        #endregion

        #region Reals

        /// <summary>
        /// Creates a real variable whose value can be any one of a set of intervals, which correspond to values of the specified finite domain
        /// </summary>
        public static RealVariable ScoreVariable<T>(FiniteDomainVariable<T> finiteDomainVariable, ScoreMapping<T> scoreMapping)
        {
            RealVariable realVariable = new RealVariable(finiteDomainVariable.ConstraintThingySolver, null, RealVariable.DefaultRange);

            Constraint.InRange(realVariable, scoreMapping.Select(pair => pair.Second).Aggregate(Interval.Union));

            Constraint.Score(realVariable, finiteDomainVariable, scoreMapping);

            return realVariable;
        }

        /// <summary>
        /// Creates and returns a real variable which represents the maximum of <paramref name="a"/> and <paramref name="b"/>
        /// </summary>
        public static RealVariable Maximize(RealVariable a, RealVariable b)
        {
            RealVariable max;

            if (a.ConstraintThingySolver.SubexpressionEliminator.TryGetValue(Maximize, a, b, out max))
            {
                return max;
            }

            max = new RealVariable(a.ConstraintThingySolver, null, RealVariable.DefaultRange);

            // calculate the possible range for the sum so we improve the speed of the search
            Constraint.InRange(max, MultiInterval.Max(a.AllowableValues.First, b.AllowableValues.First));

            Max(max, a, b);

            a.ConstraintThingySolver.SubexpressionEliminator.Store(Maximize, a, b, max);

            return max;
        }

        /// <summary>
        /// Creates and returns a real variable which represents the minimum of <paramref name="a"/> and <paramref name="b"/>
        /// </summary>
        public static RealVariable Minimize(RealVariable a, RealVariable b)
        {
            RealVariable min;

            if (a.ConstraintThingySolver.SubexpressionEliminator.TryGetValue(Minimize, a, b, out min))
            {
                return min;
            }

            min = new RealVariable(a.ConstraintThingySolver, null, RealVariable.DefaultRange);

            // calculate the possible range for the sum so we improve the speed of the search
            Constraint.InRange(min, MultiInterval.Min(a.AllowableValues.First, b.AllowableValues.First));

            Min(min, a, b);

            a.ConstraintThingySolver.SubexpressionEliminator.Store(Minimize, a, b, min);

            return min;
        }

        /// <summary>
        /// Constrains <paramref name="max"/> to be the maximum of <paramref name="a"/> and <paramref name="b"/>
        /// </summary>
        public static void Max(RealVariable max, RealVariable a, RealVariable b)
        {
            new RealMaxConstraint(max, a, b);
        }

        /// <summary>
        /// Constrains <paramref name="min"/> to be the minimum of <paramref name="a"/> and <paramref name="b"/>
        /// </summary>
        public static void Min(RealVariable min, RealVariable a, RealVariable b)
        {
            new RealMinConstraint(min, a, b);
        }

        /// <summary>
        /// Constrains <paramref name="sum"/> to be the sum of <paramref name="a"/> and <paramref name="b"/>
        /// </summary>
        public static void Sum(RealVariable sum, RealVariable a, RealVariable b)
        {
            new RealSumConstraint(sum, a, b);
        }

        /// <summary>
        /// Creates and returns an real variable that represents the sum of <paramref name="a"/> and <paramref name="b"/>
        /// </summary>
        public static RealVariable Add(RealVariable a, RealVariable b)
        {
            RealVariable sum;

            if (a.ConstraintThingySolver.SubexpressionEliminator.TryGetValue(Add, a, b, out sum))
            {
                return sum;
            }

            sum = new RealVariable(a.ConstraintThingySolver, null, RealVariable.DefaultRange);

            // calculate the possible range for the sum so we improve the speed of the search
            Constraint.InRange(sum, a.AllowableValues.First + b.AllowableValues.First);

            Sum(sum, a, b);
            
            a.ConstraintThingySolver.SubexpressionEliminator.Store(Add, a, b, sum);

            return sum;
        }

        /// <summary>
        /// Creates and returns an real variable that represents the sum of <paramref name="variables"/>
        /// </summary>
        public static RealVariable Add(params RealVariable[] variables)
        {
            return variables.Aggregate(Add);
        }

        /// <summary>
        /// Creates and returns an real variable that represents the sum of <paramref name="variables"/>
        /// </summary>
        public static RealVariable Add(IEnumerable<RealVariable> variables)
        {
            return variables.Aggregate(Add);
        }

        /// <summary>
        /// Constraints <paramref name="product"/> to be the product of <paramref name="a"/> and <paramref name="b"/>
        /// </summary>
        public static void Product(RealVariable product, RealVariable a, RealVariable b)
        {
            new RealProductConstraint(product, a, b);
        }

        /// <summary>
        /// Creates and returns an real variable that represents the product of <paramref name="a"/> and <paramref name="b"/>
        /// </summary>
        public static RealVariable Multiply(RealVariable a, RealVariable b)
        {
            RealVariable product;

            if (a.ConstraintThingySolver.SubexpressionEliminator.TryGetValue(Add, a, b, out product))
            {
                return product;
            }

            product = new RealVariable(a.ConstraintThingySolver, null, RealVariable.DefaultRange);

            Constraint.InRange(product, a.AllowableValues.First * b.AllowableValues.First);

            Product(product, a, b);

            a.ConstraintThingySolver.SubexpressionEliminator.Store(Multiply, a, b, product);

            return product;
        }

        /// <summary>
        /// Creates and returns an real variable that represents the product of <paramref name="variables"/>
        /// </summary>
        public static RealVariable Multiply(params RealVariable[] variables)
        {
            return variables.Aggregate(Multiply);
        }

        /// <summary>
        /// Creates and returns an real variable that represents the product of <paramref name="variables"/>
        /// </summary>
        public static RealVariable Multiply(IEnumerable<RealVariable> variables)
        {
            return variables.Aggregate(Multiply);
        }

        /// <summary>
        /// Creates a new real variable that represents the square of <paramref name="variable"/>
        /// </summary>
        public static RealVariable Square(RealVariable variable)
        {
            var result = variable * variable;

            Constraint.GreaterThanOrEqual(result, 0.0);

            return result;
        }

        /// <summary>
        /// Constrains <paramref name="quotient"/> to be the quotient of <paramref name="dividend"/> and <paramref name="divisor"/>
        /// </summary>
        public static void Quotient(RealVariable quotient, RealVariable dividend, RealVariable divisor)
        {
            new RealQuotientConstraint(quotient, dividend, divisor);
        }

        /// <summary>
        /// Creates and returns an real variable that represents the quotient of <paramref name="dividend"/> and <paramref name="divisor"/>
        /// </summary>
        public static RealVariable Divide(RealVariable dividend, RealVariable divisor)
        {
            RealVariable quotient;

            if (dividend.ConstraintThingySolver.SubexpressionEliminator.TryGetValue(Divide, dividend, divisor, out quotient))
            {
                return quotient;
            }

            quotient = new RealVariable(dividend.ConstraintThingySolver, null, RealVariable.DefaultRange);

            Constraint.InRange(quotient, dividend.AllowableValues.First / divisor.AllowableValues.First);

            Quotient(quotient, dividend, divisor);

            dividend.ConstraintThingySolver.SubexpressionEliminator.Store(Divide, dividend, divisor, quotient);

            return quotient;
        }

        /// <summary>
        /// Creates and returns an real variable that represents the difference of <paramref name="a"/> and <paramref name="b"/>
        /// </summary>
        public static void Difference(RealVariable difference, RealVariable a, RealVariable b)
        {
            new RealDifferenceConstraint(difference, a, b);
        }

        /// <summary>
        /// Creates and returns an real variable that represents the difference of <paramref name="a"/> and <paramref name="b"/>
        /// </summary>
        public static RealVariable Subtract(RealVariable a, RealVariable b)
        {
            RealVariable difference;

            if (a.ConstraintThingySolver.SubexpressionEliminator.TryGetValue(Subtract, a, b, out difference))
            {
                return difference;
            }

            difference = new RealVariable(a.ConstraintThingySolver, null, RealVariable.DefaultRange);

            Constraint.InRange(difference, a.AllowableValues.First - b.AllowableValues.First);
            
            Difference(difference, a, b);

            a.ConstraintThingySolver.SubexpressionEliminator.Store(Subtract, a, b, difference);

            return difference;
        }

        /// <summary>
        /// Constrains that the values of <paramref name="a"/> and <paramref name="b"/> are equal.
        /// </summary>
        public static void Equal(RealVariable a, RealVariable b)
        {
            new RealEqualityConstraint(a, b);
        }

        /// <summary>
        /// Constrains that the values of <paramref name="variables"/> all be equal.
        /// </summary>
        public static void Equal(params RealVariable[] variables)
        {
            for (int i = 0; i < variables.Length - 1; i++)
            {
                Equal(variables[i], variables[i + 1]);
            }
        }

        /// <summary>
        /// Constrains that the values of <paramref name="variables"/> all be equal.
        /// </summary>
        public static void Equal(IEnumerable<RealVariable> variables)
        {
            Equal(variables.ToArray());
        }

        /// <summary>
        /// Constraints <paramref name="variable"/> to be exactly <paramref name="value"/>
        /// </summary>
        public static void Equal(RealVariable variable, double value)
        {
            InRange(variable, value, value);
        }

        /// <summary>
        /// Constrains <paramref name="variable"/> to never take the specified value <paramref name="value"/>
        /// </summary>
        public static void Exclude(RealVariable variable, double value)
        {
            Exclude(variable, new Interval(value));
        }

        /// <summary>
        /// Constrains <paramref name="variable"/> to never take values in <paramref name="multiInterval"/>
        /// </summary>
        public static void Exclude(RealVariable variable, MultiInterval multiInterval)
        {
            foreach (var interval in multiInterval)
            {
                Exclude(variable, interval);
            }
        }

        /// <summary>
        /// Constraints <paramref name="variable"/> to never take values in <paramref name="interval"/>
        /// </summary>
        public static void Exclude(RealVariable variable, Interval interval)
        {
            InRange(variable, new MultiInterval(new Interval(double.NegativeInfinity, interval.LowerBound), new Interval(interval.UpperBound, double.PositiveInfinity)));
        }

        /// <summary>
        /// Constrains <paramref name="variable"/> to be in <paramref name="multiInterval"/>
        /// </summary>
        public static void InRange(RealVariable variable, MultiInterval multiInterval)
        {
            variable.BackdoorSet(variable.AllowableValues.Rest.AddFront(MultiInterval.Intersection(variable.AllowableValues.First, multiInterval)));
        }

        /// <summary>
        /// Constrains <paramref name="variable" /> to be in <paramref name="range"/>
        /// </summary>
        public static void InRange(RealVariable variable, Interval range)
        {
            InRange(variable, new MultiInterval(range));
        }

        /// <summary>
        /// Constrains <paramref name="variable"/> to be between <paramref name="minValue"/> and <paramref name="maxValue"/>
        /// </summary>
        public static void InRange(RealVariable variable, double minValue, double maxValue)
        {
            InRange(variable, new Interval(minValue, maxValue));
        }

        /// <summary>
        /// Constraints <paramref name="variable"/> to be at most <paramref name="value"/>
        /// </summary>
        public static void LessThanOrEqual(RealVariable variable, double value)
        {
            InRange(variable, double.NegativeInfinity, value);
        }

        /// <summary>
        /// Constrains <paramref name="variable"/> to be at least <paramref name="value"/>
        /// </summary>
        public static void GreaterThanOrEqual(RealVariable variable, double value)
        {
            InRange(variable, value, double.PositiveInfinity);
        }

        #endregion

        #region Vector 2

        /// <summary>
        /// Constrains <paramref name="sum"/> to be the sum of <paramref name="a"/> and <paramref name="b"/>
        /// </summary>
        public static void Sum(Vector2Variable sum, Vector2Variable a, Vector2Variable b)
        {
            Constraint.Sum(sum.X, a.X, b.X);
            Constraint.Sum(sum.Y, a.Y, b.Y);
        }

        /// <summary>
        /// Creates and returns an real variable that represents the sum of <paramref name="a"/> and <paramref name="b"/>
        /// </summary>
        public static Vector2Variable Add(Vector2Variable a, Vector2Variable b)
        {
            return new Vector2Variable(Constraint.Add(a.X, b.X), Constraint.Add(a.Y, b.Y));
        }

        /// <summary>
        /// Creates and returns an real variable that represents the sum of <paramref name="variables"/>
        /// </summary>
        public static Vector2Variable Add(params Vector2Variable[] variables)
        {
            return variables.Aggregate(Add);
        }

        /// <summary>
        /// Creates and returns an real variable that represents the sum of <paramref name="variables"/>
        /// </summary>
        public static Vector2Variable Add(IEnumerable<Vector2Variable> variables)
        {
            return variables.Aggregate(Add);
        }

        /// <summary>
        /// Constraints <paramref name="product"/> to be the dot product of <paramref name="a"/> and <paramref name="b"/>
        /// </summary>
        public static void DotProduct(Vector2Variable product, Vector2Variable a, Vector2Variable b)
        {
            Constraint.Product(product.X, a.X, b.X);
            Constraint.Product(product.Y, a.Y, b.Y);
        }

        /// <summary>
        /// Creates and returns an real variable that represents the dot product of <paramref name="a"/> and <paramref name="b"/>
        /// </summary>
        public static Vector2Variable Dot(Vector2Variable a, Vector2Variable b)
        {
            return new Vector2Variable(Constraint.Multiply(a.Y, b.Y), Constraint.Multiply(a.Y, b.Y));
        }
        
        /// <summary>
        /// Creates and returns an real variable that represents the difference of <paramref name="a"/> and <paramref name="b"/>
        /// </summary>
        public static void Difference(Vector2Variable difference, Vector2Variable a, Vector2Variable b)
        {
            Constraint.Difference(difference.X, a.X, b.X);
            Constraint.Difference(difference.Y, a.Y, b.Y);
        }

        /// <summary>
        /// Creates and returns an real variable that represents the difference of <paramref name="a"/> and <paramref name="b"/>
        /// </summary>
        public static Vector2Variable Subtract(Vector2Variable a, Vector2Variable b)
        {
            return new Vector2Variable(Constraint.Subtract(a.X, b.X), Constraint.Subtract(a.Y, b.Y));
        }

        /// <summary>
        /// Creates and returns an real variable that represents the distance squared between <paramref name="a"/> and <paramref name="b"/>
        /// </summary>
        public static RealVariable DistanceSquared(Vector2Variable a, Vector2Variable b)
        {
            RealVariable x1MinusX2 = a.X - b.X;
            RealVariable y1MinusY2 = a.Y - b.Y;

            RealVariable result = Square(x1MinusX2) + Square(y1MinusY2);
            Constraint.GreaterThanOrEqual(result, 0.0);

            return result;
        }

        /// <summary>
        /// Constrains that the values of <paramref name="a"/> and <paramref name="b"/> are equal.
        /// </summary>
        public static void Equal(Vector2Variable a, Vector2Variable b)
        {
            Constraint.Equal(a.X, b.X);
            Constraint.Equal(a.Y, b.Y);
        }

        /// <summary>
        /// Constrains that the values of <paramref name="variables"/> all be equal.
        /// </summary>
        public static void Equal(params Vector2Variable[] variables)
        {
            for (int i = 0; i < variables.Length - 1; i++)
            {
                Equal(variables[i], variables[i + 1]);
            }
        }

        /// <summary>
        /// Constrains that the values of <paramref name="variables"/> all be equal.
        /// </summary>
        public static void Equal(IEnumerable<Vector2Variable> variables)
        {
            Equal(variables.ToArray());
        }

        /// <summary>
        /// Constraints <paramref name="variable"/> to be exactly <paramref name="value"/>
        /// </summary>
        public static void Equal(Vector2Variable variable, Vector2 value)
        {
            Constraint.Equal(variable.X, value.X);
            Constraint.Equal(variable.Y, value.Y);
        }

        #endregion

        #region Finite domains

        /// <summary>
        /// Limits the number of occurences of <paramref name="value"/> in <paramref name="variables"/> between <paramref name="minOccurences"/> and <paramref name="maxOccurences"/>
        /// </summary>
        public static void LimitOccurences<T>(T value, int minOccurences, int maxOccurences, params FiniteDomainVariable<T>[] variables)
        {
            new CardinalityConstraint<T>(value, minOccurences, maxOccurences, variables);
        }

        /// <summary>
        /// Requires the number of occurences of <paramref name="value"/> in <paramref name="variables"/> to be exactly <paramref name="occurences"/>
        /// </summary>
        public static void RequireOccurences<T>(T value, int occurences, params FiniteDomainVariable<T>[] variables)
        {
            LimitOccurences(value, occurences, occurences, variables);
        }

        /// <summary>
        /// Limits the number of occurences of <paramref name="value"/> in <paramref name="variables"/> at most <paramref name="maxOccurences"/>
        /// </summary>
        public static void MaximumOccurences<T>(T value, int maxOccurences, params FiniteDomainVariable<T>[] variables)
        {
            LimitOccurences(value, 0, maxOccurences, variables);
        }

        /// <summary>
        /// Limits the number of occurences of <paramref name="value"/> in <paramref name="variables"/> at least <paramref name="minOccurences"/>
        /// </summary>
        public static void MinimumOccurences<T>(T value, int minOccurences, params FiniteDomainVariable<T>[] variables)
        {
            LimitOccurences(value, minOccurences, int.MaxValue, variables);
        }

        /// <summary>
        /// Constrains that the values of <paramref name="a"/> and <paramref name="b"/> are equal.
        /// </summary>
        public static void Equal<T>(FiniteDomainVariable<T> a, FiniteDomainVariable<T> b)
        {
            new EqualityConstraint<T>(a, b);
        }

        /// <summary>
        /// Constrains that the values of <paramref name="variables"/> are equal.
        /// </summary>
        public static void Equal<T>(params FiniteDomainVariable<T>[] variables)
        {
            for (int i = 0; i < variables.Length - 1; i++)
                Equal(variables[i], variables[i + 1]);
        }

        /// <summary>
        /// Constrains that the values of <paramref name="variables"/> are equal.
        /// </summary>
        public static void Equal<T>(IEnumerable<FiniteDomainVariable<T>> variables)
        {
            Equal(variables.ToArray());
        }

        /// <summary>
        /// Constrains that the values of <paramref name="a"/> and <paramref name="b"/> are not equal.
        /// </summary>
        public static void NotEqual<T>(FiniteDomainVariable<T> a, FiniteDomainVariable<T> b)
        {
            new InequalityConstraint<T>(a, b);
        }

        /// <summary>
        /// Constrains that the values of <paramref name="variables"/> are not equal.
        /// </summary>
        public static void NotEqual<T>(params FiniteDomainVariable<T>[] variables)
        {
            for (int i = 0; i < variables.Length - 1; i++)
                NotEqual(variables[i], variables[i + 1]);
        }

        /// <summary>
        /// Constrains that the values of <paramref name="variables"/> are not equal.
        /// </summary>
        public static void NotEqual<T>(IEnumerable<FiniteDomainVariable<T>> variables)
        {
            NotEqual(variables.ToArray());
        }

        /// <summary>
        /// Constraints that the values of <paramref name="variables"/> are not equal.
        /// </summary>
        public static void AllDifferent<T>(params FiniteDomainVariable<T>[] variables)
        {
            NotEqual(variables);
        }

        /// <summary>
        /// Constraints that the values of <paramref name="variables"/> are not equal.
        /// </summary>
        public static void AllDifferent<T>(IEnumerable<FiniteDomainVariable<T>> variables)
        {
            NotEqual(variables);
        }

        /// <summary>
        /// Constrains <paramref name="variable"/> to <paramref name="value"/>
        /// </summary>
        public static void Equal<T>(FiniteDomainVariable<T> variable, T value)
        {
            variable.BackdoorSet(variable.AllowableValues & BitHelper.GetMask(variable.FiniteDomain.IndexOf(value)));
        }

        /// <summary>
        /// Constrains <paramref name="variable"/> to one element of <paramref name="values"/>
        /// </summary>
        public static void ElementOf<T>(FiniteDomainVariable<T> variable, params T[] values)
        {
            ElementOf(variable, (IEnumerable<T>)values);
        }

        /// <summary>
        /// Constrains <paramref name="variable"/> to one element of <paramref name="values"/>
        /// </summary>
        public static void ElementOf<T>(FiniteDomainVariable<T> variable, IEnumerable<T> values)
        {
            ulong mask = 0UL;

            foreach (var value in values)
            {
                mask = mask.SetBit(variable.FiniteDomain.IndexOf(value));
            }

            variable.BackdoorSet(variable.AllowableValues & mask);
        }

        /// <summary>
        /// Prohibits <paramref name="variable"/> from assuming <paramref name="value"/>
        /// </summary>
        public static void NotEqual<T>(FiniteDomainVariable<T> variable, T value)
        {
            variable.BackdoorSet(variable.AllowableValues.ClearBit(variable.FiniteDomain.IndexOf(value)));
        }


        #endregion

        #region Interfacing

        /// <summary>
        /// Creates a new score constraint, which constrains the value of the <paramref name="score"/> variable to be some assignment of a value in the <paramref name="finiteDomainVariable"/> based on the specified <paramref name="scoreMapping"/> from finite domain items to real-valued intervals
        /// </summary>
        public static void Score<T>(RealVariable score, FiniteDomainVariable<T> finiteDomainVariable, ScoreMapping<T> scoreMapping)
        {
            new ScoreContraint<T>(score, finiteDomainVariable, scoreMapping);
        }

        #endregion
    }
}