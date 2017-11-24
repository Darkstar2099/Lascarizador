using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Lascarizador.Core.Repositories
{
    // Interface IRepository(Repositório) do padrão Repository(Repositório).
    public interface IRepository<TEntity> where TEntity : class
    {
        // Achando objetos.
        TEntity Get(int id);
        IEnumerable<TEntity> GetAll();
        IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate);
        TEntity SingleOrDefault(Expression<Func<TEntity, bool>> predicate);

        // Adicionando objetos.
        void Add(TEntity entity);
        void AddRange(IEnumerable<TEntity> entities);

        // Removendo objetos.
        void Remove(TEntity entity);
        void RemoveRange(IEnumerable<TEntity> entities);
    }
}
