﻿using System.Linq.Expressions;

namespace DataConnectorLibraryProject.Interface
{
    public interface IRepository<TEntity> where TEntity : class, IEntity
    {
        Task<TEntity?> GetByIdAsync(string id);
        Task AddAsync(TEntity entity);
        Task UpdateAsync(TEntity entity, bool saveImmediately = false);
        Task DeleteAsync(string id);
        Task<IList<TEntity>> GetAllAsync();
        IRepository<TEntity> IncludeProperty(Expression<Func<TEntity, object>> includeExpression);
    }
}
