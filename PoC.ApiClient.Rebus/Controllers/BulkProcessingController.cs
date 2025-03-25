using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PoC.Common.Models;
using Rebus.Bus;

namespace PoC.ApiClient.Rebus.Controllers
{
    [ApiController]
    [Route("api/bulk")]
    public class BulkProcessingController : ControllerBase
    {
        private readonly IBus _bus;

        public BulkProcessingController(IBus bus) => _bus = bus;

        [HttpPost("process")]
        public async Task<IActionResult> ProcessBulkRequest([FromBody] BulkProcessingRequest request)
        {
            if (request.Items == null || request.Items.Count == 0)
            {
                return BadRequest("Request must contain data.");
            }

            Console.WriteLine($"Received Bulk Request {request.RequestId}, sending to RabbitMQ...");

            await _bus.Send(request);

            return Accepted(new { Message = "Bulk request received!", RequestId = request.RequestId });
        }
    }
}
