using System;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;

namespace CSharpUtils
{
    /// <summary>
    /// Allows easy access to compiled constructors that take no arguments.
    /// </summary>
    public static class Constructor<T> where T : class, new()
    {
        /// <summary>
        /// The compiled constructor.
        /// </summary>
        public static readonly Func<T> Compiled = GetConstructor();

        private static Func<T> GetConstructor()
        {
            ConstructorInfo constructorInfo = typeof(T).GetConstructor(new Type[0]);

            Debug.Assert(constructorInfo != null, "Constructor info cannot be null! The type was guaranteed to have a default constructor.");

            return Expression.Lambda<Func<T>>(Expression.New(constructorInfo)).Compile();
        }
    }

    /// <summary>
    /// Allows easy access to compiled constructors that take one argument.
    /// </summary>
    public static class Constructor<TArg1, TObj>
    {
        /// <summary>
        /// The compiled constructor.
        /// </summary>
        public static readonly Func<TArg1, TObj> Compiled = GetConstructor();

        private static Func<TArg1, TObj> GetConstructor()
        {
            ConstructorInfo constructorInfo = typeof(TObj).GetConstructor(new[] { typeof(TArg1) });

            if (constructorInfo == null) return null;

            ParameterExpression[] parameterExpressions = new[]
                                                           {
                                                               Expression.Parameter(typeof(TArg1), "p1")
                                                           };

            return Expression.Lambda<Func<TArg1, TObj>>(Expression.New(constructorInfo, parameterExpressions), parameterExpressions).Compile();
        }
    }

    /// <summary>
    /// Allows easy access to compiled constructors that take two arguments.
    /// </summary>
    public static class Constructor<TArg1, TArg2, TObj>
    {
        /// <summary>
        /// The compiled constructor.
        /// </summary>
        public static readonly Func<TArg1, TArg2, TObj> Compiled = GetConstructor();

        private static Func<TArg1, TArg2, TObj> GetConstructor()
        {
            ConstructorInfo constructorInfo = typeof(TObj).GetConstructor(new[] { typeof(TArg1), typeof(TArg2) });

            if (constructorInfo == null) return null;

            ParameterExpression[] parameterExpressions = new[]
                                                           {
                                                               Expression.Parameter(typeof(TArg1), "p1"),
                                                               Expression.Parameter(typeof(TArg2), "p2")
                                                           };

            return Expression.Lambda<Func<TArg1, TArg2, TObj>>(Expression.New(constructorInfo, parameterExpressions), parameterExpressions).Compile();
        }
    }

    /// <summary>
    /// Allows easy access to compiled constructors that take three arguments.
    /// </summary>
    public static class Constructor<TArg1, TArg2, TArg3, TObj>
    {
        /// <summary>
        /// The compiled constructor.
        /// </summary>
        public static readonly Func<TArg1, TArg2, TArg3, TObj> Compiled = GetConstructor();

        private static Func<TArg1, TArg2, TArg3, TObj> GetConstructor()
        {
            ConstructorInfo constructorInfo = typeof(TObj).GetConstructor(new[] { typeof(TArg1), typeof(TArg2), typeof(TArg3) });

            if (constructorInfo == null) return null;

            ParameterExpression[] parameterExpressions = new[]
                                                           {
                                                               Expression.Parameter(typeof(TArg1), "p1"),
                                                               Expression.Parameter(typeof(TArg2), "p2"),
                                                               Expression.Parameter(typeof(TArg3), "p3"),
                                                           };

            return Expression.Lambda<Func<TArg1, TArg2, TArg3, TObj>>(Expression.New(constructorInfo, parameterExpressions), parameterExpressions).Compile();
        }
    }

    /// <summary>
    /// Allows easy access to compiled constructors that take four arguments.
    /// </summary>
    public static class Constructor<TArg1, TArg2, TArg3, TArg4, TObj>
    {
        /// <summary>
        /// The compiled constructor.
        /// </summary>
        public static readonly Func<TArg1, TArg2, TArg3, TArg4, TObj> Compiled = GetConstructor();

        private static Func<TArg1, TArg2, TArg3, TArg4, TObj> GetConstructor()
        {
            ConstructorInfo constructorInfo = typeof(TObj).GetConstructor(new[] { typeof(TArg1), typeof(TArg2), typeof(TArg3), typeof(TArg4) });

            if (constructorInfo == null) return null;

            ParameterExpression[] parameterExpressions = new[]
                                                           {
                                                               Expression.Parameter(typeof(TArg1), "p1"),
                                                               Expression.Parameter(typeof(TArg2), "p2"),
                                                               Expression.Parameter(typeof(TArg3), "p3"),
                                                               Expression.Parameter(typeof(TArg4), "p4"),
                                                           };

            return Expression.Lambda<Func<TArg1, TArg2, TArg3, TArg4, TObj>>(Expression.New(constructorInfo, parameterExpressions), parameterExpressions).Compile();
        }
    }

    /// <summary>
    /// Allows easy access to compiled constructors that take five arguments.
    /// </summary>
    public static class Constructor<TArg1, TArg2, TArg3, TArg4, TArg5, TObj>
    {
        /// <summary>
        /// The compiled constructor.
        /// </summary>
        public static readonly Func<TArg1, TArg2, TArg3, TArg4, TArg5, TObj> Compiled = GetConstructor();

        private static Func<TArg1, TArg2, TArg3, TArg4, TArg5, TObj> GetConstructor()
        {
            ConstructorInfo constructorInfo = typeof(TObj).GetConstructor(new[] { typeof(TArg1), typeof(TArg2), typeof(TArg3), typeof(TArg4), typeof(TArg5) });

            if (constructorInfo == null) return null;

            ParameterExpression[] parameterExpressions = new[]
                                                           {
                                                               Expression.Parameter(typeof(TArg1), "p1"),
                                                               Expression.Parameter(typeof(TArg2), "p2"),
                                                               Expression.Parameter(typeof(TArg3), "p3"),
                                                               Expression.Parameter(typeof(TArg4), "p4"),
                                                               Expression.Parameter(typeof(TArg5), "p5"),
                                                           };

            return Expression.Lambda<Func<TArg1, TArg2, TArg3, TArg4, TArg5, TObj>>(Expression.New(constructorInfo, parameterExpressions), parameterExpressions).Compile();
        }
    }
}
