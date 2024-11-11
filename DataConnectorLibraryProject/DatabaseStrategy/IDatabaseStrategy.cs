using DataConnectorLibraryProject.Interface;

namespace DataConnectorLibraryProject.DatabaseStrategy
{
    public interface IDatabaseStrategy
    {
        IRepository<TEntity> GetRepository<TEntity>() where TEntity : class, IEntity;
        Task SaveShangesAsync();
    }
}