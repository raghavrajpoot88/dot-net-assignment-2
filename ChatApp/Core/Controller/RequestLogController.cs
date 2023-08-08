using ChatApp.MiddleLayer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.Core.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class RequestLogController : ControllerBase
    {
        private readonly IRequestLogsService _requestLogsService;

        public RequestLogController( IRequestLogsService requestLogsService)
        {
            _requestLogsService = requestLogsService;
        }
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> RequestLogsOutput(int timeInterval)
        {

            var result = await _requestLogsService.GetLogs(timeInterval);
            if(result == null) return NotFound();
            return Ok(result);
        }
    }
}
