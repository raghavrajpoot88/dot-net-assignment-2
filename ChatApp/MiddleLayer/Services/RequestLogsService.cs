using ChatApp.DomainModel.Models;
using ChatApp.DomainModel.Repo.Interfaces;

namespace ChatApp.MiddleLayer.Services
{
    public class RequestLogsService : IRequestLogsService
    {
        private readonly IRequestLog _requestLog;

        public RequestLogsService(IRequestLog requestLogs)
        {
            _requestLog = requestLogs;
        }

        public async Task<ICollection<RequestLog>> GetLogs(int timeInterval)
        {
           return await _requestLog.GetLogList(timeInterval);
        }

    }
}
