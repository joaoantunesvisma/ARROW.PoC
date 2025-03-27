namespace PoC.Worker.Rebus.Saga.Models
{
    public class BatchProcessed
    {
        public Guid? RequestId { get; set; } // Unique Request ID of the bulk operation
        public int ProcessedItemCount { get; set; } // Number of items processed in this batch

        //public string MessageId { get; set; } = Guid.NewGuid().ToString(); // Unique message ID for idempotency
    }
}
