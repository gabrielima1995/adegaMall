using Framework;
using Infrastructure.Data.Context;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Linq.Expressions;

namespace Infrastructure.Data
{
    [Obsolete("Não crie novos repositórios a partir desta classe. Ao invés disso, crie uma classe específica, partir de uma interface também específica, com métodos relevantes para a entidade específica.")]
    public class Repository : IRepository
    {
        protected ScaffoldContext Context { get; }

        public Repository(ScaffoldContext context)
        {
            Context = context;
        }

        public void SaveChanges()
        {
            try
            {
                if (Context.ChangeTracker.HasChanges())
                    Context.SaveChanges();
            }
            catch (DbEntityValidationException ex)
            {
                var msg = string.Empty;

                foreach (var errors in ex.EntityValidationErrors)
                    foreach (var error in errors.ValidationErrors)
                        msg += $"{error.PropertyName}: {error.ErrorMessage}" + Environment.NewLine;

                throw new InvalidOperationException(msg, ex);
            }
        }

        public void Dispose()
        {
            Context.Dispose();
        }
    }

    [Obsolete("Não crie novos repositórios a partir desta classe. Ao invés disso, crie uma classe específica, partir de uma interface também específica, com métodos relevantes para a entidade específica.")]
    public class Repository<T> : Repository, IRepository<T> where T : class
    {
        public Repository(ScaffoldContext context)
            : base(context)
        {
        }

        public void Insert(T obj)
        {
            Context.Set<T>().Add(obj);
            Context.Entry(obj).State = EntityState.Added;
        }

        public void InsertRange(IEnumerable<T> list)
        {
            Context.Set<T>().AddRange(list);

            foreach (var item in list)
                Context.Entry(item).State = EntityState.Added;
        }

        public void Update(T obj)
        {
            Context.Entry(obj).State = EntityState.Modified;
        }

        public void Delete(T obj)
        {
            Context.Set<T>().Remove(obj);
        }

        public void DeleteRange(IEnumerable<T> list)
        {
            Context.Set<T>().RemoveRange(list);

            foreach (var item in list)
                Context.Entry(item).State = EntityState.Deleted;
        }

        public T Get(object id)
        {
            return Context.Set<T>().Find(id);
        }

        public IQueryable<T> Query(params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> result = Context.Set<T>();

            if (includes.Any())
                foreach (var include in includes)
                    result = result.Include(include);

            return result;
        }

        public T Get(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes)
        {
            return Query(includes)
                .FirstOrDefault(predicate);
        }
    }
}