namespace PoC.Worker.Rebus.Models
{
    public class BatchProcessed
    {
        public Guid? RequestId { get; set; } // Unique Request ID of the bulk operation
        public int ProcessedItemCount { get; set; } // Number of items processed in this batch
    }
}
