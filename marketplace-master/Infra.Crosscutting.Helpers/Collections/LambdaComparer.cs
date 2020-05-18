using System.Linq.Expressions;

namespace System.Collections.Generic
{
    public class LambdaComparer<T> : IComparer<T>
    {
        private readonly Expression<Func<T, T, int>> _expression;

        // Constructor
        public LambdaComparer(Expression<Func<T, T, int>> expression)
        {
            _expression = expression;
        }

        // Execute the compare
        public int Compare(T x, T y)
        {
            if (x == null || y == null)
            {
                if (y == null && x == null) return 0;
                if (y == null) return 1;

                return -1;
            }

            return _expression.Compile().Invoke(x, y);
        }
    }
}