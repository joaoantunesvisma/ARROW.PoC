namespace PoC.Common
{
    public class Constants
    {
        public const string BulkQueueName = "bulk_processing_queue";
        public const string RabbitMqConnectionString = "amqp://guest:guest@localhost:5672";
        public const string MongoDbConnectionString = "mongodb://guest:guest@localhost:27017";
        public const string MongoDbName = "ARROW_Saga";
    }
}
