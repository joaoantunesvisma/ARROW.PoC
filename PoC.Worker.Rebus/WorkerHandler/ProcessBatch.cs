namespace PoC.Worker.Rebus.WorkerHandler
{
    public class ProcessBatch
    {
        public Guid? RequestId { get; set; }
        public int BatchNumber { get; set; }
        public List<string> Items { get; set; } = new();
    }
}
