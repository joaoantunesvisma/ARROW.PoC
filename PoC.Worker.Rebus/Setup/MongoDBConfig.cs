using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using PoC.Worker.Rebus.Saga.Models;

namespace PoC.Worker.Rebus.Setup
{
    public static class MongoDbConfig
    {
        public static void ConfigureMongoDBSerialization()
        {
            BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));

            var objectSerializer = new ObjectSerializer(type => type == typeof(BulkProcessingSagaData));
            BsonSerializer.RegisterSerializer(objectSerializer);
        }
    }
}
