// See https://aka.ms/new-console-template for more information

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDB.Driver;
using PoC.Common;
using PoC.Worker.Rebus.Setup;
using PoC.Worker.Rebus.WorkerHandler;
using Rebus.Config;
using Rebus.Routing.TypeBased;
using Rebus.Serialization.Json;

var host = Host.CreateDefaultBuilder()
    .ConfigureServices((_, services) =>
    {

        MongoDbConfig.ConfigureMongoDBSerialization();
        var mongoClient = new MongoClient(Constants.MongoDbConnectionString);
        var mongoDatabase = mongoClient.GetDatabase(Constants.MongoDbName);

        services.AddRebus(config => config
            .Options(o => o.SetNumberOfWorkers(1)) // example setting workers to 5.
            .Transport(t => t.UseRabbitMq(Constants.RabbitMqConnectionString, Constants.BulkQueueName))
            //.Routing(r => r.TypeBased().Map<BulkProcessingRequest>(Constants.BulkQueueName)) // Explicit mapping
            .Routing(r => r.TypeBased().MapFallback(Constants.BulkQueueName)) // Automatically routes all messages to this queue
                                                                              //.Sagas(s => s.StoreInMemory())
        //.Sagas(s => s.StoreInMongoDb(mongoDatabase))
        //.Sagas(s => s.StoreInMemory())
        .Sagas(s => s.StoreInMongoDb(mongoDatabase, 
                // Optional: Custom collection name resolver
                sagaType => $"Sagas_{sagaType.Name}", 
                automaticallyCreateIndexes: true))
        .Serialization(s => s.UseSystemTextJson())
        );

        services.AutoRegisterHandlersFromAssemblyOf<ProcessBatchHandler>();
        services.AddHostedService<StartupService>();
    })
    .Build();


await host.RunAsync();


Console.WriteLine("Hello, World!");
