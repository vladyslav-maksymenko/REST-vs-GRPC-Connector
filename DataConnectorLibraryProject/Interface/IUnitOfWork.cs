namespace DataConnectorLibraryProject.Interface
{
    internal interface IUnitOfWork : IDisposable
    {
        IRepository<TEntity> GetRepository<TEntity>(Type entryType) where TEntity : class, IEntity;
        Task SaveShangesAsync();
    }
}
