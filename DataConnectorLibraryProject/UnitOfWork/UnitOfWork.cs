using DataConnectorLibraryProject.DataAccess.Data;
using DataConnectorLibraryProject.DatabaseStrategy;
using DataConnectorLibraryProject.Interface;

namespace DataConnectorLibraryProject.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        public DbAccessStrategies DbContexts { get; private set;}
        private IDatabaseStrategy curentStrategy;
        private bool disposed = false;

        public UnitOfWork(
            SqlDataConnectorDbContext sqlDataConnectorDbContext,
            MongoDataConnectorDbContext mongoDataConnectorDbContext)
        {
            DbContexts = new DbAccessStrategies(sqlDataConnectorDbContext, mongoDataConnectorDbContext);
            curentStrategy = DbContexts.Sql(); // Default strategy.
        }
        
        public IRepository<TEntity> GetRepository<TEntity>() where TEntity : class, IEntity
        {
            return curentStrategy.GetRepository<TEntity>();
        }

        public async Task SaveShangesAsync()
        {
            await DbContexts.Sql().SaveShangesAsync();
            await DbContexts.Mongo().SaveShangesAsync();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    DbContexts.Sql()?.Dispose();
                    DbContexts.Mongo()?.Dispose();
                }
                disposed = true;
            }
        }
        
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void SwitchContext(IDatabaseStrategy strategy)
        {
            if (curentStrategy == null) //Dev point.
            {
                throw new InvalidOperationException("Database context has not been switched. Call 'SwitchContext' first.");
            }

            curentStrategy = strategy;
        }
    }
}