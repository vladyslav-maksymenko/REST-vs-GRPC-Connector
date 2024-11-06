using System.Collections.Generic;

namespace DataConnectorLibraryProject.Interface
{
    internal interface IRepository<TEntity> where TEntity : class
    {
        Task<TEntity?> GetByIdAsync(Guid Id);
        Task AddAsync(TEntity entity);
        Task UpdateAsync(TEntity entity, bool saveImmediately = false);
        Task DeleteAsync(Guid Id);
        Task<IList<TEntity>> GetAllAsync();
    }
}
