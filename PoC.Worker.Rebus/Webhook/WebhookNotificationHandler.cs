using PoC.Worker.Rebus.Saga.Models;
using Rebus.Handlers;

namespace PoC.Worker.Rebus.Webhook
{
    public class WebhookNotificationHandler : IHandleMessages<BulkProcessingCompleted>
    {

        public Task Handle(BulkProcessingCompleted message)
        {
            Console.WriteLine($"Notify Webhook! Processing BulkProcessingCompleted " +
                $"RequestId: {message.RequestId} TotalBatches: {message.TotalBatches})");
            return Task.CompletedTask;
        }
    }
}
