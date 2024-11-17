using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DataConnectorLibraryProject.Extensions
{
    internal static class DbSetExtensions
    {
        public static IQueryable<TEntity> IncludeIncludes<TEntity>(this IQueryable<TEntity> query, List<Expression<Func<TEntity, object>>> includeExpressions)
            where TEntity : class
        {
            if (includeExpressions == null || !includeExpressions.Any())
            {
                return query;
            }

            return includeExpressions.Aggregate(query, (current, includeExpression) => current.Include(includeExpression));
        }
    }
}
