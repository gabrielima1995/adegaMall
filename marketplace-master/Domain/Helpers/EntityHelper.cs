using System;

namespace Framework.Helpers
{
    public static class EntityHelper
    {
        public static TEntity IfNull<TEntity>(this TEntity entity, Func<TEntity> func)
            where TEntity : class
        {
            return entity ?? func();
        }

        public static TEntity IfNull<TEntity>(this TEntity entity, Action func)
            where TEntity : class
        {
            if (entity == null)
                func();

            return entity;
        }

        public static TEntity IfNotNull<TEntity>(this TEntity entity, Action<TEntity> func)
            where TEntity : class
        {
            if (entity != null)
                func(entity);

            return entity;
        }

        public static TEntity IfNotNull<TEntity>(this TEntity entity, Action func)
            where TEntity : class
        {
            if (entity != null)
                func();

            return entity;
        }
    }
}