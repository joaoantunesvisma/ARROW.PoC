using Rebus.Sagas;

namespace PoC.Worker.Rebus.Saga.Models
{
    public class BulkProcessingSagaData : SagaData
    {
        public Guid RequestId { get; set; }
        public int TotalBatches { get; set; }
        public int ProcessedBatches { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public DateTime LastUpdatedAtUtc { get; set; } = DateTime.UtcNow;
        public DateTime? CompletedAtUtc { get; set; }

        //// Processed messages for idempotency
        //public HashSet<string> ProcessedMessageIds { get; set; } = new();

    }
}
