using DataConnectorLibraryProject.DataAccess.Data;
using DataConnectorLibraryProject.DatabaseStrategy;
using DataConnectorLibraryProject.Interface;

namespace DataConnectorLibraryProject.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        public DbAccessStrategies dbAccessStrategies { get; private set;}
        private IDatabaseStrategy curentStrategy;
        private bool disposed = false;

        public UnitOfWork(
            SqlDataConnectorDbContext sqlDataConnectorDbContext,
            MongoDataConnectorDbContext mongoDataConnectorDbContext)
        {
            dbAccessStrategies = new DbAccessStrategies(sqlDataConnectorDbContext, mongoDataConnectorDbContext);
        }
        

        public IRepository<TEntity> GetRepository<TEntity>(Type entryType) where TEntity : class, IEntity
        {
            return curentStrategy.GetRepository<TEntity>();
        }

        public async Task SaveShangesAsync()
        {
            await dbAccessStrategies.Sql().SaveShangesAsync();
            await dbAccessStrategies.Mongo().SaveShangesAsync();
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    dbAccessStrategies.Sql()?.Dispose();
                    dbAccessStrategies.Mongo()?.Dispose();
                }
                disposed = true;
            }
        }
        
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void SwitchStrategy(IDatabaseStrategy strategy)
        {
            curentStrategy = strategy;
        }
    }
}