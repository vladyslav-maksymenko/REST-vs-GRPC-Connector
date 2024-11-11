using DataConnectorLibraryProject.DataAccess.Data;

namespace DataConnectorLibraryProject.DatabaseStrategy
{
    public class SqlDatabaseStrategy : DatabaseStrategyBase
    {
        public SqlDatabaseStrategy(SqlDataConnectorDbContext dbContext) : base(dbContext)
        {
        }
    }
}