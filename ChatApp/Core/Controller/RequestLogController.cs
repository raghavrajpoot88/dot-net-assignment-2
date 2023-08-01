using ChatApp.MiddleLayer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.Core.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class RequestLogController : ControllerBase
    {
        //private readonly ILogger _logger;ILogger logger, ILogModelCreator logCreator,
        //private readonly ILogModelCreator _logCreator;
        private readonly IRequestLogsService _requestLogsService;

        public RequestLogController( IRequestLogsService requestLogsService)
        {
            //_logger = logger;
            //_logCreator = logCreator;
            _requestLogsService = requestLogsService;
        }
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> RequestLogsOutput()
        {
            var result = await _requestLogsService.GetLogs();
            return Ok(result);
        }
    }
}
