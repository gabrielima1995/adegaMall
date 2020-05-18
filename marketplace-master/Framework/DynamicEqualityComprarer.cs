using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Framework
{
    public static class DynamicEqualityComprarer<T>
    {
        public static DynamicEqualityComprarer<T, TProperty> Create<TProperty>(Expression<Func<T, TProperty>> keySelector)
        {
            return new DynamicEqualityComprarer<T, TProperty>(keySelector);
        }
    }

    public class DynamicEqualityComprarer<T, TProperty> : IEqualityComparer<T>
    {
        private readonly Func<T, TProperty> _keySelector;

        public bool Equals(T x, T y)
        {
            var xValue = _keySelector.Invoke(x);
            var yValue = _keySelector.Invoke(y);

            return Equals(xValue, yValue);
        }

        public int GetHashCode(T obj)
        {
            return obj.GetHashCode();
        }

        public DynamicEqualityComprarer(Expression<Func<T, TProperty>> keySelector)
        {
            _keySelector = keySelector.Compile();
        }
    }
}
