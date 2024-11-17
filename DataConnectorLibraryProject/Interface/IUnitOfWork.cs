using DataConnectorLibraryProject.DatabaseStrategy;

namespace DataConnectorLibraryProject.Interface
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<TEntity> GetRepository<TEntity>() where TEntity : class, IEntity;
        Task SaveShangesAsync();
        void SwitchContext(IDatabaseStrategy strategy);
        DbAccessStrategies DbContexts { get; }
    }
}
