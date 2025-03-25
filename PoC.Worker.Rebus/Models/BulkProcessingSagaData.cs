using Rebus.Sagas;

namespace PoC.Worker.Rebus.Models
{
    public class BulkProcessingSagaData : SagaData
    {
        public Guid RequestId { get; set; }
        public int TotalBatches { get; set; }
        public int ProcessedBatches { get; set; }
        public bool IsCompleted { get; set; }
    }
}
