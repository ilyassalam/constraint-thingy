using System;
using System.Collections.Generic;

namespace ConstraintThingy
{
    class SubexpressionEliminator
    {
        private readonly Dictionary<Subexpression, Variable> _subexpressions = new Dictionary<Subexpression, Variable>();

        public bool TryGetValue<TArg1, TArg2, TResult>(Func<TArg1, TArg2, TResult> func, TArg1 arg1, TArg2 arg2, out TResult result)
            where TArg1 : Variable
            where TArg2 : Variable
            where TResult : Variable
        {
            Variable res = null;

            bool success = _subexpressions.TryGetValue(new Subexpression(func, arg1, arg2), out res);

            result = (TResult) res;

            return success;
        }

        public void Store<TArg1, TArg2, TResult>(Func<TArg1, TArg2, TResult> func, TArg1 arg1, TArg2 arg2, TResult result)
            where TArg1 : Variable
            where TArg2 : Variable
            where TResult : Variable
        {
            _subexpressions.Add(new Subexpression(func, arg1, arg2), result);
        }
    }
}