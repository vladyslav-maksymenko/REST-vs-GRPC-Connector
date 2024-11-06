using DataConnectorLibraryProject.Interface;
using Microsoft.EntityFrameworkCore;

namespace DataConnectorLibraryProject.Repository
{
    internal class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        private readonly DbSet<TEntity> dbSet;
        public Repository(DbContext dbContext)
        {
            dbSet = dbContext.Set<TEntity>();
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync() => await dbSet.ToArrayAsync();

        public async Task AddAsync(TEntity entity) => await dbSet.AddAsync(entity);
        

        public async Task DeleteAsync(Guid Id)
        {
            //var gg = dbSet.FirstOrDefaultAsync(Id);
            throw new NotImplementedException();
        }

        


        public async Task<TEntity> GetByIdAsync(Guid Id) => await dbSet.SingleOrDefaultAsync();

        public Task UpdateAsync(TEntity entity)
        {
            throw new NotImplementedException();
        }
    }
}
