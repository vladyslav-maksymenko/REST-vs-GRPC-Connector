using DataConnectorLibraryProject.DataAccess.Data;

namespace DataConnectorLibraryProject.DatabaseStrategy
{
    public class MongoDatabaseStrategy : DatabaseStrategyBase
    {
        public MongoDatabaseStrategy(MongoDataConnectorDbContext dbContext) : base(dbContext)
        {
        }
    }
}