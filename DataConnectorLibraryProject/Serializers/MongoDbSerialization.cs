using DataConnectorLibraryProject.Models.ServerSideModels;
using MongoDB.Bson.Serialization;

namespace DataConnectorLibraryProject.Serializers
{
    public class MongoDbSerialization
    {
        public static void AddCustomMongoDbSerialization()
        {
            BsonClassMap.RegisterClassMap<Customer>(cm =>
            {
                cm.AutoMap();
                cm.MapIdField(g => g.Id).SetSerializer(new ObjectIdSerializer());
            });
        }
    }
}