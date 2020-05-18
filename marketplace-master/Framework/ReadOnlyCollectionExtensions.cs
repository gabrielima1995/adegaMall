using System.Collections.Generic;

namespace System.Collections.ObjectModel
{
    public static class ReadOnlyCollection
    {
        public static ReadOnlyCollection<T> Empty<T>()
        {
            return new ReadOnlyCollection<T>(new List<T>(0));
        }
    }
}
