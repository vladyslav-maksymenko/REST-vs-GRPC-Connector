using DataConnectorLibraryProject.Interface;
using Microsoft.EntityFrameworkCore;
using DataConnectorLibraryProject.Extensions;
using System.Linq.Expressions;
using DataConnectorLibraryProject.RepositoryWrapper;

namespace DataConnectorLibraryProject.Repository
{
    internal class Repository<TEntity> : IRepository<TEntity>, IRepositoryWrapper
        where TEntity : class, IEntity
    {
        private readonly DbContext dbContext;
        private readonly DbSet<TEntity> dbSet;
        private readonly List<Expression<Func<TEntity, object>>> includeExpressions;
        public Repository(DbContext dbContext)
        {
            dbSet = dbContext.Set<TEntity>();
            this.dbContext = dbContext;
            includeExpressions = new List<Expression<Func<TEntity, object>>>();
        }

        public async Task<IList<TEntity>> GetAllAsync() 
        {
            return await dbSet.IncludeIncludes(includeExpressions).ToListAsync();
        }

        public async Task AddAsync(TEntity entity) => await dbSet.AddAsync(entity);
 
        public async Task DeleteAsync(Guid id)
        {
            var entity = await dbSet.FirstOrDefaultAsync(x => x.Id == id);
            if (entity != null)
            {
                dbSet.Remove(entity);
            }
        }

        public async Task<TEntity?> GetByIdAsync(Guid id) 
        {
            var entity = await dbSet.IncludeIncludes(includeExpressions).SingleOrDefaultAsync(x => x.Id == id);
            return entity ?? throw new KeyNotFoundException("Entity not found");
        } 

        public async Task UpdateAsync(TEntity entity, bool saveImmediately = false)
        {
            dbSet.Update(entity);
            if (saveImmediately)
            {
                await dbContext.SaveChangesAsync();
            }
        }

        public IRepository<TEntity> IncludeProperty(Expression<Func<TEntity, object>> includeExpression)
        {
            includeExpressions.Add(includeExpression);
            return this;
        }

        public Type EntityType  => typeof(TEntity);
    }
}
