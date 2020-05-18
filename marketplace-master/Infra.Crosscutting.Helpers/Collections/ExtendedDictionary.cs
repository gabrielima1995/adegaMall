namespace System.Collections.Generic
{
    public class ExtendedDictionary<TKey, TValue> : Dictionary<TKey, TValue>
    {
        public ExtendedDictionary()
            : base()
        {

        }

        public ExtendedDictionary(int capacity)
            : base(capacity)
        {

        }

        public ExtendedDictionary(IEqualityComparer<TKey> comparer)
            : base(comparer)
        {

        }

        public ExtendedDictionary(IDictionary<TKey, TValue> dictionary)
            : base(dictionary)
        {

        }

        public ExtendedDictionary(int capacity, IEqualityComparer<TKey> comparer)
            : base(capacity, comparer)
        {

        }

        public ExtendedDictionary(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer)
            : base(dictionary, comparer)
        {

        }

        public new void Add(TKey key, TValue value)
        {
            if (ContainsKey(key))
            {
                this[key] = value;
            }
            else
            {
                base.Add(key, value);
            }
        }

        public ExtendedDictionary<TKey, TValue> Extend(params IDictionary<TKey, TValue>[] sources)
        {
            foreach (var source in sources)
            {
                foreach (var item in source)
                {
                    Add(item.Key, item.Value);
                }
            }

            return this;
        }
    }
}