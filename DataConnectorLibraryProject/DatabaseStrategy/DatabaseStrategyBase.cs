using DataConnectorLibraryProject.Interface;
using Microsoft.EntityFrameworkCore;
using DataConnectorLibraryProject.Repository;
using DataConnectorLibraryProject.RepositoryWrapper;

namespace DataConnectorLibraryProject.DatabaseStrategy
{
    public class DatabaseStrategyBase : IDatabaseStrategy, IDisposable
    {
        protected readonly DbContext dbContext;
        private readonly Dictionary<Type, IRepositoryWrapper> repositories = new();
        private bool disposed = false;
        protected DatabaseStrategyBase(DbContext dbContext)
        {
            this.dbContext = dbContext;
            repositories = new();
        }

        public IRepository<TEntity> GetRepository<TEntity>() where TEntity : class, IEntity
        {
            if (!repositories.TryGetValue(typeof(TEntity), out var repository))
            {
                repository = new Repository<TEntity>(dbContext);
                repositories[typeof(TEntity)] = repository;
            }

            return (repository as IRepository<TEntity>)!;
        }

        public async Task SaveShangesAsync()
        {
            await dbContext.SaveChangesAsync();
        }
        
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    dbContext?.Dispose();
                }
                disposed = true;
            }
        }
        
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}