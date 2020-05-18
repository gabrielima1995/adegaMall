using Framework.Helpers;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace Framework
{
    public abstract class ServiceBase<TEntity> : IService<TEntity> where TEntity : class
    {
        protected ServiceBase(IRepository<TEntity> repository)
        {
            Repository = repository;
        }

        protected IRepository<TEntity> Repository { get; }

        /// <summary>
        /// Executa uma ação de UPDATE da entidade no banco de dados.
        /// </summary>
        /// <param name="entity">A entidade a ser persistida.</param>
        /// <param name="update">Função que retorna uma entidade do tipo TEntity, pronta para ser ATUALIZADA no banco de dados.</param>
        /// <param name="update"></param>
        private void UpdateInternal(TEntity entity, Action<TEntity> update)
        {
            update(entity);
            Repository.Update(entity);
            Repository.SaveChanges();
        }

        /// <summary>
        /// Persiste uma entidade no banco de dados.
        /// </summary>
        /// <param name="get">Função aplicada para descobrir se a entidade precisa ser criada ou apenas atualizada no banco de dados.</param>
        /// <param name="create">Função que retorna uma entidade do tipo TEntity, pronta para ser CADASTRADA no banco de dados.</param>
        /// <param name="update">Função que retorna uma entidade do tipo TEntity, pronta para ser ATUALIZADA no banco de dados.</param>
        /// <param name="includes">Especifica as sub-entidades, aninhadas à entidade principal, que devem ser recuperadas na operação de get.</param>
        /// <returns>A entidade, conforme persistida no banco de dados.</returns>
        public virtual TEntity Save(Expression<Func<TEntity, bool>> get, Func<TEntity> create, Action<TEntity> update, params Expression<Func<TEntity, object>>[] includes)
        {
            return Get(get, includes)
                .IfNotNull(entity => UpdateInternal(entity, update))
                .IfNull(() => Create(create));
        }

        /// <summary>
        /// Cadastra uma entidade no banco de dados
        /// </summary>
        /// <param name="create">Função que retorna uma entidade do tipo TEntity, pronta para ser CADASTRADA no banco de dados.</param>
        /// <returns>A entidade, conforme persistida no banco de dados.</returns>
        public virtual TEntity Create(Func<TEntity> create)
        {
            var entity = create();

            Repository.Insert(entity);
            Repository.SaveChanges();

            return entity;
        }

        /// <summary>
        /// Persiste as alterações de uma entidade no banco de dados.
        /// </summary>
        /// <param name="get">Função aplicada para descobrir se a entidade precisa ser criada ou apenas atualizada no banco de dados.</param>
        /// <param name="update">Função que retorna uma entidade do tipo TEntity, pronta para ser ATUALIZADA no banco de dados.</param>
        /// <param name="includes">Especifica as sub-entidades, aninhadas à entidade principal, que devem ser recuperadas na operação de get.</param>
        /// <returns>A entidade, conforme persistida no banco de dados.</returns>
        public virtual TEntity Update(Expression<Func<TEntity, bool>> get, Action<TEntity> update, params Expression<Func<TEntity, object>>[] includes)
        {
            return Get(get, includes)
                .IfNotNull(entity => UpdateInternal(entity, update));
        }

        /// <summary>
        /// Exclui uma entidade do banco de dados.
        /// </summary>
        /// <param name="get">Função aplicada para descobrir se a entidade precisa ser criada ou apenas atualizada no banco de dados.</param>
        /// <returns>A entidade excluída do banco de dados.</returns>
        public virtual TEntity Delete(Expression<Func<TEntity, bool>> get, params Expression<Func<TEntity, object>>[] includes)
        {
            return Get(get, includes)
                .IfNotNull(entity =>
                {
                    Repository.Delete(entity);
                    Repository.SaveChanges();
                });
        }

        /// <summary>
        /// Recupera uma entidade do banco de dados.
        /// </summary>
        /// <param name="get">Função aplicada para filtrar a entidade a ser recuperada.</param>
        /// <param name="includes">Especifica as sub-entidades, aninhadas à entidade principal, que devem ser recuperadas na operação de get.</param>
        /// <returns>A entidade encontrada, ou null.</returns>
        public virtual TEntity Get(Expression<Func<TEntity, bool>> get, params Expression<Func<TEntity, object>>[] includes)
        {
            return Repository.Get(get, includes);
        }

        /// <summary>
        /// Recupera uma lista de entidades do banco de dados.
        /// </summary>
        /// <param name="get">Função aplicada para filtrar as entidades a serem recuperadas.</param>
        /// <param name="includes">Especifica as sub-entidades, aninhadas à entidade principal, que devem ser recuperadas na operação de get.</param>
        /// <returns>As entidades encontradas.</returns>
        public virtual IQueryable<TEntity> Query(params Expression<Func<TEntity, object>>[] includes)
        {
            return Repository.Query(includes);
        }

        /// <summary>
        /// Verifica se uma entidade existe no banco de dados.
        /// </summary>
        /// <param name="get">Função aplicada para filtrar a entidade a ser verificada.</param>
        /// <returns>true se a entidade existir no banco de dados; do contrário, false.</returns>
        public virtual bool Exists(Expression<Func<TEntity, bool>> get)
        {
            return Repository.Query().Any(get);
        }
    }
}