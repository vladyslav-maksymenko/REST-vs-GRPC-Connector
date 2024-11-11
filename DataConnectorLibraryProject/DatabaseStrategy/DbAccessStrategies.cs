using DataConnectorLibraryProject.DataAccess.Data;

namespace DataConnectorLibraryProject.DatabaseStrategy
{
    public class DbAccessStrategies
    {
        private SqlDataConnectorDbContext sqlDataConnectorDbContext;
        private MongoDataConnectorDbContext mongoDataConnectorDbContext;
        private SqlDatabaseStrategy sqlInstance;
        private MongoDatabaseStrategy mongoInstance;

        public DbAccessStrategies(
            SqlDataConnectorDbContext sqlDataConnectorDbContext,
            MongoDataConnectorDbContext mongoDataConnectorDbContext)
        {
            this.sqlDataConnectorDbContext = sqlDataConnectorDbContext;
            this.mongoDataConnectorDbContext = mongoDataConnectorDbContext;
        }

        public SqlDatabaseStrategy Sql() => sqlInstance ?? (sqlInstance = new(sqlDataConnectorDbContext));

        public MongoDatabaseStrategy Mongo() => mongoInstance ?? (mongoInstance = new(mongoDataConnectorDbContext));
        
        
    }
}