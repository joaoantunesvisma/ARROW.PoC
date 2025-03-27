namespace PoC.Worker.Rebus.Saga.Models
{
    public class BulkProcessingCompleted
    {
        public Guid? RequestId { get; set; }
        public int TotalBatches { get; set; }
    }
}
