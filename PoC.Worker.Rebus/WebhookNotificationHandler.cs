using PoC.Worker.Rebus.Models;
using Rebus.Handlers;

namespace PoC.Worker.Rebus
{
    public class WebhookNotificationHandler : IHandleMessages<BulkProcessingCompleted>
    {

        public Task Handle(BulkProcessingCompleted message)
        {
            Console.WriteLine($"Notify Webhook! Processing BulkProcessingCompleted " +
                $"RequestId: {message.RequestId} TotalBatches: { message.TotalBatches})");
            return Task.CompletedTask;
        }
    }
}
