using PoC.Worker.Rebus.Saga.Models;
using Rebus.Bus;
using Rebus.Handlers;

namespace PoC.Worker.Rebus.WorkerHandler
{
    public class ProcessBatchHandler : IHandleMessages<ProcessBatch>
    {
        private readonly IBus _bus;

        public ProcessBatchHandler(IBus bus) => _bus = bus;

        public async Task Handle(ProcessBatch message)
        {
            Console.WriteLine($"[WORKER] Processing Batch {message.BatchNumber} for RequestId {message.RequestId}.");

            foreach (var item in message.Items)
            {
                Console.WriteLine($"Processing Item: {item}");
                await Task.Delay(100); // Simulate processing
            }

            // Broadcast BatchProcessed event
            await _bus.Publish(new BatchProcessed
            {
                RequestId = message.RequestId,
                ProcessedItemCount = message.Items.Count
            });

            Console.WriteLine($"[WORKER] Completed Batch {message.BatchNumber} for RequestId {message.RequestId}.");
        }
    }

}
