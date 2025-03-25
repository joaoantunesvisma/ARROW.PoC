using Microsoft.Extensions.Hosting;
using PoC.Worker.Rebus.Models;
using Rebus.Bus;

namespace PoC.Worker.Rebus
{
    public class StartupService : BackgroundService
    {
        private readonly IBus _bus;

        public StartupService(IBus bus)
        {
            _bus = bus;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _bus.Subscribe<BatchProcessed>(); // 🔹 Ensure this worker listens for published events
            await _bus.Subscribe<BulkProcessingCompleted>();
        }
    }
}
