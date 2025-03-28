﻿using PoC.Common.Models;
using PoC.Worker.Rebus.Saga.Models;
using PoC.Worker.Rebus.WorkerHandler;
using Rebus.Bus;
using Rebus.Handlers;
using Rebus.Sagas;

namespace PoC.Worker.Rebus.Saga
{
    public class BulkProcessingSaga : Saga<BulkProcessingSagaData>,
        IAmInitiatedBy<BulkProcessingRequest>,
        IHandleMessages<BatchProcessed>
    {
        private readonly IBus _bus;

        public BulkProcessingSaga(IBus bus) => _bus = bus;

        protected override void CorrelateMessages(ICorrelationConfig<BulkProcessingSagaData> config)
        {
            config.Correlate<BulkProcessingRequest>(msg => msg.RequestId, saga => saga.RequestId);
            config.Correlate<BatchProcessed>(msg => msg.RequestId, saga => saga.RequestId);
        }

        // 🔹 Step 1: When BulkProcessingRequest is received, split into batches and send ProcessBatch messages
        public async Task Handle(BulkProcessingRequest message)
        {
            Data.RequestId = message.RequestId;
            Data.TotalBatches = (message.Items.Count + 2) / 3; // Ex: If 10 items, we get 4 batches of size ≈3
            Data.ProcessedBatches = 0;
            Data.IsCompleted = false;
            Data.LastUpdatedAtUtc = DateTime.UtcNow;

            Console.WriteLine($"[SAGA] Splitting RequestId {message.RequestId} into {Data.TotalBatches} batches.");

            int batchSize = 3;
            int batchNumber = 0; // Track batch number

            foreach (var batch in message.Items.Chunk(batchSize))
            {
                var processBatch = new ProcessBatch
                {
                    RequestId = message.RequestId,
                    BatchNumber = ++batchNumber,
                    Items = batch.ToList(),
                };

                await _bus.Send(processBatch);  // 🔹 Send a ProcessBatch message to be processed
                Console.WriteLine($"[SAGA] Sent Batch {batchNumber} for RequestId {message.RequestId}.");
            }
        }

        // 🔹 Step 2: Track completion of each batch
        public async Task Handle(BatchProcessed message)
        {
            Data.ProcessedBatches++;
            Data.LastUpdatedAtUtc = DateTime.UtcNow;
            //Data.ProcessedMessageIds.Add(message.MessageId);

            Console.WriteLine($"[SAGA] Processed {Data.ProcessedBatches}/{Data.TotalBatches} batches for RequestId {message.RequestId}.");

            if (Data.ProcessedBatches >= Data.TotalBatches)
            {
                Data.IsCompleted = true;
                Data.LastUpdatedAtUtc = DateTime.UtcNow;
                Data.CompletedAtUtc = DateTime.UtcNow;

                await _bus.Publish(new BulkProcessingCompleted
                {
                    RequestId = message.RequestId,
                    TotalBatches = Data.TotalBatches
                });

                // Problem: I stopped RabbitMQ in this step and:
                // SAGA was updated and messaged was not ACK and BulkProcessingCompleted was not published
                // I then enabled Rabbitmq
                // so message was retried.
                // it duplicated the SAGA process, it did wrongly by incrementing twice the ProcessedBatches :(
                // was expecting if i enable o.EnableIdempotentSagas() it will not process the message again, but it will not publish the BulkProcessingCompleted
                // but saw no changes while using EnableIdempotentSagas, was hoping to some Ids went to mongo but saw no changes

                Console.WriteLine($"[SAGA] All batches are processed for RequestId {message.RequestId}. Marking as completed.");
                //MarkAsComplete();
            }
        }
    }
}