using Framework;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Domain.Interfaces.Repositories
{
    [Obsolete("Não crie novos repositórios a partir desta interface. Ao invés disso, crie uma interface com métodos relevantes para a entidade específica.")]
    public interface ICrudRepository<TEntity> where TEntity : class
    {
        TEntity GetById(params object[] idValues);

        TEntity Get(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] includes);

        List<TEntity> List(params Expression<Func<TEntity, object>>[] includes);

        OperationResult Add(TEntity entity, bool saveChanges = false);

        OperationResult Update(TEntity entity, bool saveChanges = false);

        OperationResult Delete(TEntity entity, bool saveChanges = false);

        OperationResult SaveChanges();
    }
}
