using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DataConnectorLibraryProject.Extensions
{
    internal static class DbSetExtensions
    {
        public static IQueryable<TEntity> IncludeIncludes<TEntity>(this DbSet<TEntity> dbSet, List<Expression<Func<TEntity, object>>> includeExpressions)
            where TEntity : class
        {
            if (includeExpressions == null || !includeExpressions.Any())
            {
                return dbSet;
            }

            return includeExpressions.Aggregate(dbSet.AsQueryable(), (current, includeExpression) => current.Include(includeExpression));
        }
    }
}
