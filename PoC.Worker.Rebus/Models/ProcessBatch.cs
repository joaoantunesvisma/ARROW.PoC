namespace PoC.Worker.Rebus.Models
{
    public class ProcessBatch
    {
        public Guid? RequestId { get; set; }
        public int BatchNumber { get; set; }
        public List<string> Items { get; set; } = new();
    }
}
