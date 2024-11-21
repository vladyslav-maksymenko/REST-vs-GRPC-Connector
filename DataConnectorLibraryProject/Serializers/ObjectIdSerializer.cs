using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace DataConnectorLibraryProject.Serializers
{
    public class ObjectIdSerializer : SerializerBase<string>
    {
        public override string Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            var objectId = context.Reader.ReadObjectId();
            return objectId.ToString();
        }

        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, string value)
        {
            var objectId = new ObjectId(value);
            context.Writer.WriteObjectId(objectId);
        }
    }
}