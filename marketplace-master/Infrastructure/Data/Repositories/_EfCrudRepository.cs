using Domain.Interfaces.Repositories;
using Framework;
using Infrastructure.Data.Context;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Linq.Expressions;

namespace Infrastructure.Data.Repositories
{
    public abstract class EfCrudRepository : EfRepository
    {
        public EfCrudRepository(ScaffoldContext context) : base(context)
        {
        }

        public virtual TEntity GetById<TEntity>(params object[] idValues) where TEntity : class
        {
            return Context.Set<TEntity>().Find(idValues);
        }

        public List<TEntity> List<TEntity>() where TEntity : class
        {
            return Context.Set<TEntity>().ToList();
        }

        public OperationResult Add<TEntity>(TEntity entity, bool saveChanges = false) where TEntity : class
        {
            try
            {
                Context.Set<TEntity>().Add(entity);
                if (saveChanges) return SaveChanges();
                return OperationResult.Success;
            }
            catch (Exception ex)
            {
                return OperationResult.Fail(ex.Message);
            }
        }

        public OperationResult Update<TEntity>(TEntity entity, bool saveChanges = false) where TEntity : class
        {
            try
            {
                Context.Entry(entity).State = EntityState.Modified;
                if (saveChanges) return SaveChanges();
                return OperationResult.Success;
            }
            catch (Exception ex)
            {
                return OperationResult.Fail(ex.Message);
            }
        }

        public OperationResult Delete<TEntity>(TEntity entity, bool saveChanges = false) where TEntity : class
        {
            try
            {
                Context.Set<TEntity>().Remove(entity);
                if (saveChanges) return SaveChanges();
                return OperationResult.Success;
            }
            catch (Exception ex)
            {
                return OperationResult.Fail(ex.Message);
            }
        }

        public OperationResult SaveChanges()
        {
            try
            {
                if (Context.ChangeTracker.HasChanges())
                    Context.SaveChanges();

                return OperationResult.Success;
            }
            catch (DbEntityValidationException ex)
            {
                var errorMessages = ex.EntityValidationErrors
                    .SelectMany(x => x.ValidationErrors)
                    .Select(x => x.ErrorMessage);

                var fullErrorMessage = string.Join("; ", errorMessages);
                var exceptionMessage = string.Concat(ex.Message, " The validation errors are: ", fullErrorMessage);
                return OperationResult.Fail(exceptionMessage);
            }
            catch (Exception ex)
            {
                return OperationResult.Fail(ex.Message);
            }
        }
    }

    [Obsolete]
    public abstract class EfCrudRepository<TEntity> : EfRepository, ICrudRepository<TEntity> where TEntity : class
    {
        public EfCrudRepository(ScaffoldContext context) : base(context)
        {
        }

        protected IQueryable<TEntity> Query(params Expression<Func<TEntity, object>>[] includes)
        {
            IQueryable<TEntity> result = Context.Set<TEntity>();

            if (includes.Any())
                foreach (var include in includes)
                    result = result.Include(include);

            return result;
        }

        public virtual TEntity GetById(params object[] idValues)
        {
            return Context.Set<TEntity>().Find(idValues);
        }

        public virtual TEntity Get(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] includes)
        {
            return Query(includes).FirstOrDefault(predicate);
        }

        public List<TEntity> List(params Expression<Func<TEntity, object>>[] includes)
        {
            return Query(includes).ToList();
        }

        public OperationResult Add(TEntity entity, bool saveChanges = false)
        {
            try
            {
                Context.Set<TEntity>().Add(entity);
                if (saveChanges) return SaveChanges();
                return OperationResult.Success;
            }
            catch (Exception ex)
            {
                return OperationResult.Fail(ex.Message);
            }
        }

        public OperationResult Update(TEntity entity, bool saveChanges = false)
        {
            try
            {
                Context.Entry(entity).State = EntityState.Modified;
                if (saveChanges) return SaveChanges();
                return OperationResult.Success;
            }
            catch (Exception ex)
            {
                return OperationResult.Fail(ex.Message);
            }
        }

        public OperationResult Delete(TEntity entity, bool saveChanges = false)
        {
            try
            {
                Context.Set<TEntity>().Remove(entity);
                if (saveChanges) return SaveChanges();
                return OperationResult.Success;
            }
            catch (Exception ex)
            {
                return OperationResult.Fail(ex.Message);
            }
        }

        public OperationResult SaveChanges()
        {
            try
            {
                if (Context.ChangeTracker.HasChanges())
                    Context.SaveChanges();

                return OperationResult.Success;
            }
            catch (DbEntityValidationException ex)
            {
                var errorMessages = ex.EntityValidationErrors
                    .SelectMany(x => x.ValidationErrors)
                    .Select(x => x.ErrorMessage);

                var fullErrorMessage = string.Join("; ", errorMessages);
                var exceptionMessage = string.Concat(ex.Message, " The validation errors are: ", fullErrorMessage);
                return OperationResult.Fail(exceptionMessage);
            }
            catch (Exception ex)
            {
                return OperationResult.Fail(ex.Message);
            }
        }
    }
}