using System;
using System.Collections.Generic;
using System.Text;

namespace LinearEquationSolver
{
    public static class ComparisonFactory
    {
        public static Comparison<Term> CreateVariableOrderComparison(Dictionary<string, int> order)
        {
            if (order == null) throw new ArgumentNullException("order can NOT be null");

            Comparison<Term> cmpFn = (Term x, Term y) => {
                if (string.IsNullOrEmpty(x.Variable) || string.IsNullOrEmpty(y.Variable))
                    return y.Variable.CompareTo(x.Variable);

                if (order.ContainsKey(x.Variable) && order.ContainsKey(y.Variable))
                    return order[y.Variable].CompareTo(order[x.Variable]);

                return x.CompareTo(y);
            };
            return cmpFn;
        }

        private static Comparison<Term> reverseComparison = (a, b) => b.CompareTo(a);

        public static Comparison<Term> CreateReverseComparison() => reverseComparison;
    }
}
