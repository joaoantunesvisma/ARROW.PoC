using PoC.Common;
using PoC.Common.Models;
using Rebus.Config;
using Rebus.Routing.TypeBased;
using Rebus.Serialization.Json;

var builder = WebApplication.CreateBuilder(args);

// Configure Rebus to use RabbitMQ
builder.Services.AddRebus(config => config
    .Transport(t => t.UseRabbitMqAsOneWayClient(Constants.RabbitMqConnectionString))
    .Routing(r => r.TypeBased().Map<BulkProcessingRequest>(Constants.BulkQueueName)) // Explicit mapping
    .Serialization(s => s.UseSystemTextJson()) // Ensure JSON Serialization
);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapGet("/", () => "PoC.ApiClient.Rebus is running...");

app.Run();
