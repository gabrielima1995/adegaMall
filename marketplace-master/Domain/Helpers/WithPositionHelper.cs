using Domain.Entities;
using System.Collections.Generic;
using System.Linq;

namespace Framework.Helpers
{
    public static class WithPositionHelper
    {
        public static IList<TEntity> FixPosition<TEntity>(this IList<TEntity> list, bool sortBeforeFixing = false) where TEntity : class, IWithPosition
        {
            if (sortBeforeFixing)
                list = list.OrderBy(i => i.Position).ToList();

            var pos = 0;
            foreach (var item in list)
                item.Position = pos++;

            return list;
        }
    }
}