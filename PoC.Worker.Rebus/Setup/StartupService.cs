using Microsoft.Extensions.Hosting;
using MongoDB.Driver;
using PoC.Common;
using PoC.Worker.Rebus.Saga.Models;
using Rebus.Bus;
using System.Linq.Expressions;

namespace PoC.Worker.Rebus.Setup
{
    public class StartupService : BackgroundService
    {
        private readonly IBus _bus;
        private readonly IMongoDatabase _mongoDatabase;
        private readonly string _collectionName = "Sagas_BulkProcessingSagaData";

        public StartupService(IBus bus)
        {
            _bus = bus;
            var mongoClient = new MongoClient(Constants.MongoDbConnectionString);
            _mongoDatabase = mongoClient.GetDatabase(Constants.MongoDbName);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await CreateSagaTTLIndexes();
            await _bus.Subscribe<BatchProcessed>(); // 🔹 Ensure this worker listens for published events
            await _bus.Subscribe<BulkProcessingCompleted>();
        }

        private async Task CreateSagaTTLIndexes()
        {
            var collection = _mongoDatabase.GetCollection<BulkProcessingSagaData>(_collectionName);

            Console.WriteLine("[MongoDB] Checking existing TTL indexes...");

            // Define index details
            var ttlIndexes = new List<(string IndexName, Expression<Func<BulkProcessingSagaData, object>> Field, TimeSpan Expiry)>
            {
                ("CompletedAtUtc_TTL", saga => saga.CompletedAtUtc, TimeSpan.FromMinutes(30)),
                ("CreatedAtUtc_TTL", saga => saga.CreatedAtUtc, TimeSpan.FromMinutes(3600))
            };

            // Process each TTL index
            foreach (var (indexName, field, expiry) in ttlIndexes)
            {
                if (!await IndexExistsAsync(collection, indexName))
                {
                    await CreateTTLIndexAsync(collection, indexName, field, expiry);
                }
                else
                {
                    Console.WriteLine($"[MongoDB] TTL index '{indexName}' already exists. Skipping...");
                }
            }

            Console.WriteLine("[MongoDB] TTL Index setup complete!");
        }

        // 🔹 Reusable Method: Check if an index exists
        private async Task<bool> IndexExistsAsync(IMongoCollection<BulkProcessingSagaData> collection, string indexName)
        {
            var indexCursor = await collection.Indexes.ListAsync();
            var indexes = await indexCursor.ToListAsync();
            return indexes.Any(index => index["name"] == indexName);
        }

        // 🔹 Reusable Method: Create TTL Index
        private async Task CreateTTLIndexAsync(IMongoCollection<BulkProcessingSagaData> collection, string indexName,
            Expression<Func<BulkProcessingSagaData, object>> field, TimeSpan expiry)
        {
            Console.WriteLine($"[MongoDB] Creating TTL index '{indexName}'...");

            var indexModel = new CreateIndexModel<BulkProcessingSagaData>(
                Builders<BulkProcessingSagaData>.IndexKeys.Ascending(field),
                new CreateIndexOptions { ExpireAfter = expiry, Name = indexName }
            );

            await collection.Indexes.CreateOneAsync(indexModel);
        }

    }
}
