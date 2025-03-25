namespace PoC.Common.Models
{
    public class BulkProcessingRequest
    {
        public Guid RequestId { get; set; } = Guid.NewGuid();
        public List<string> Items { get; set; } = new();
    }
}
