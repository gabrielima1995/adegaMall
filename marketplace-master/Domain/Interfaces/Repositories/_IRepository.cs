using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Framework
{
    [Obsolete("Não crie novos repositórios a partir desta interface. Ao invés disso, crie uma interface com métodos relevantes para a entidade específica.")]
    public interface IRepository : IDisposable
    {
        void SaveChanges();
    }

    [Obsolete("Não crie novos repositórios a partir desta interface. Ao invés disso, crie uma interface com métodos relevantes para a entidade específica.")]
    public interface IRepository<T> : IRepository
    {
        void Insert(T obj);

        void InsertRange(IEnumerable<T> list);

        void Update(T obj);

        void Delete(T obj);

        void DeleteRange(IEnumerable<T> list);

        T Get(object id);

        IQueryable<T> Query(params Expression<Func<T, object>>[] includes);

        T Get(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes);
    }
}