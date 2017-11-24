using Lascarizador.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace Lascarizador.Persistence.Repositories
{
    // Implementação da interface IRepository(Repositório) do padrão Repository(Repositório).

    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        protected readonly DbContext Context;

        public Repository(DbContext context)
        {
            Context = context;
        }

        // Método Get: Retorna uma TEntity cujo Id seja igual ao id recebido.
        public TEntity Get(int id)
        {
            return Context.Set<TEntity>().Find(id);
        }
        
        // Método GetAll: Retorna um IEnumerable com uma ou mais instâncias de TEntity.
        public IEnumerable<TEntity> GetAll()
        {
            return Context.Set<TEntity>().ToList();
        }

        // Método Find: Retorna um IEnumerable com o resultado de uma busca por TEntities
        //   que atendam ao predicado recebido.
        public IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate)
        {
            return Context.Set<TEntity>().Where(predicate);
        }

        // Método SingleOrDefault: Retorna um IEnumerable com o resultado de uma busca por TEntities
        //  que atendam ao predicado recebido. No caso de nada ser encontrado, retorna null.
        public TEntity SingleOrDefault(Expression<Func<TEntity, bool>> predicate)
        {
            return Context.Set<TEntity>().SingleOrDefault(predicate);
        }

        // Método Add: Adiciona a TEntity recebida ao Set de TEntities
        public void Add(TEntity entity)
        {
            Context.Set<TEntity>().Add(entity);
        }

        // Método AddRange: Adiciona as TEntities recebidas ao Set de TEntities
        public void AddRange(IEnumerable<TEntity> entities)
        {
            Context.Set<TEntity>().AddRange(entities);
        }

        // Método Remove: Remove a TEntity recebida do Set de TEntities
        public void Remove(TEntity entity)
        {
            Context.Set<TEntity>().Remove(entity);
        }

        // Método RemoveRange: Remove as TEntities recebidas do Set de TEntities
        public void RemoveRange(IEnumerable<TEntity> entities)
        {
            Context.Set<TEntity>().RemoveRange(entities);
        }
    };
}