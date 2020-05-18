using System;

namespace NewContext.Queries
{
    public class Paging
    {
        public Paging(int start, int limit)
        {
            Start = Math.Max(start, 0);
            Limit = Math.Max(limit, 1);
        }

        public int Start { get; }
        public int Limit { get; }
    }
}
